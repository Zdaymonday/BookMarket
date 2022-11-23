using BookMarket.DataLayer.Models;
using BookMarket.DataLayer.Repository;
using System.Text;

namespace BookMarket.DataCollectorLibrary
{
    public class BookCollector : PageCollector
    {
        private readonly string oz_search_query = @"https://oz.by/search/?c=1101523&q=";
        private readonly string oz_main = @"https://oz.by";
        private readonly string ImageDataPath;
        private readonly object syncObject = new object();

        public BookCollector(string ImageDataPath = "E:\\ImageData\\Books") : base(new HttpClient())
        {
            this.ImageDataPath = ImageDataPath;
        }

        public async Task CollectBooksFromOz(IEnumerable<LiteraryWork> works)
        {
            var publishers = await new PublisherRepository().GetAll();

            var books = new List<Book>();
            var notExistingWork = new List<LiteraryWork>();
            var errors = new List<string>();

            await Parallel.ForEachAsync(works, async (work, token) =>
            {
                string bookname = $"{work.Author.Name} {work.Title}".Replace(' ', '+');
                string query_string = $"{oz_search_query}{bookname}";

                var doc = await GetHtmlDoc(query_string);

                //проверяем есть ли search-info-results - если есть то ничего не найдено
                var info_results = doc.GetElementsByClassName("search-info-results").FirstOrDefault();
                if (info_results is not null)
                {
                    work.isExistToSale = false;
                    notExistingWork.Add(work);
                    return;
                }

                //как и проверка выше, перестарховываемся
                var search_results = doc.GetElementById("goods-table")?.GetElementsByTagName("li");
                if (search_results is null)
                {
                    work.isExistToSale = false;
                    notExistingWork.Add(work);
                    return;
                }

                foreach (var li in search_results)
                {
                    string? link = li.GetElementsByTagName("a")?.First().GetAttribute("href");

                    #region Проверка на корректность поиска
                    //если ссылка пустая или там нет /books значит скипаем
                    if (String.IsNullOrEmpty(link) || !link.Contains("/books")) continue;

                    //проверяем совпадает ли название автора или поиск дал расскраску для детей
                    var author_name_in_search = li
                                .GetElementsByClassName("item-type-card__info")
                                .FirstOrDefault()?
                                .TextContent;

                    if (!isAuthorNameCorrect(author_name_in_search, work))
                    {
                        Console.WriteLine("Имя в поиске {0} не совпало с целевым {1}", author_name_in_search ?? "null", work.Author.Name);
                        return;
                    }
                    #endregion

                    //цена
                    string? price_string = li.GetElementsByClassName("item-type-card__cost")?
                            .FirstOrDefault()?
                            .GetElementsByClassName("item-type-card__btn")?
                            .FirstOrDefault()?
                            .TextContent;

                    decimal price_decimal = 0M;

                    if (!String.IsNullOrEmpty(price_string))
                    {
                        //TODO: reg exp
                        char[] seps = { ' ', ';', '\u00A0' };
                        price_string = price_string.Trim().Split(seps).FirstOrDefault();
                        if (decimal.TryParse(price_string, out decimal price)) price_decimal = price;
                    }

                    string oz_query_path = $"{oz_main}{link}";

                    try
                    {
                        var oz_book_page = await GetHtmlDoc(oz_query_path);

                        var imageLink = oz_book_page
                            .GetElementsByClassName("b-product-photo__picture-self")
                            .First()
                            .GetElementsByTagName("a")
                            .First()
                            .GetAttribute("href");

                        var book = new Book();

                        PageInStore page = new()
                        {
                            ShopName = "oz.by",
                            BookPage = oz_query_path,
                            Book = book,
                            Price = price_decimal,
                            InStock = price_decimal != 0,
                        };

                        book.PagesInStores.Add(page);

                        #region SaveBookImg
                        string imagePath = $"{ImageDataPath}\\{book.BookImageGuid}";
                        if (!File.Exists(imagePath)) Directory.CreateDirectory(imagePath);

                        try
                        {
                            using (var fs = File.Create($"{imagePath}\\book_image.jpg"))
                            {
                                var content = await _httpClient.GetAsync(imageLink);
                                await content.Content.CopyToAsync(fs);
                            }
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("cant save book image");
                            Console.WriteLine(exc.Message);
                        }
                        #endregion

                        var book_title = oz_book_page.GetElementsByClassName("b-product-title__heading").First()
                            .GetElementsByTagName("h1").First()
                            .TextContent;

                        var book_data_info_rows = oz_book_page
                            .GetElementsByClassName("b-description__container-col").First()
                            .GetElementsByTagName("tbody").First()
                            .GetElementsByTagName("tr")
                            .ToArray();

                        //перебор таблицы с данными о книге
                        foreach (var row in book_data_info_rows)
                        {
                            string description = row.GetElementsByTagName("td").First().TextContent;

                            switch (description)
                            {
                                case "Издательство":
                                    {
                                        var pub_content = row
                                            .GetElementsByTagName("td").Skip(1).First()
                                            .GetElementsByTagName("a").First()
                                            .TextContent;
                                        Publisher? publisher;

                                        //блокируем возможность создать одинаковых издателей
                                        lock (syncObject)
                                        {
                                            publisher = publishers.FirstOrDefault(p => p.Name.Equals(pub_content, StringComparison.CurrentCultureIgnoreCase));
                                            if (publisher is null)
                                            {
                                                var pubRep = new PublisherRepository();
                                                publisher = new Publisher() { Name = pub_content, FullName = pub_content };

                                                pubRep.Add(publisher);
                                                publishers = publishers.Append(publisher);
                                            }
                                        }

                                        book.PublisherId = publisher.Id;
                                        book.Publisher = publisher;
                                    }; break;

                                case "Год издания":
                                    {
                                        var year = row
                                            .GetElementsByTagName("td").Skip(1).First()
                                            .TextContent;

                                        book.Year = year;
                                    }; break;

                                case "Страниц":
                                    {
                                        var pages_content = row
                                        .GetElementsByTagName("td").Skip(1).First()
                                        .TextContent;

                                        int.TryParse(pages_content, out var page_number);

                                        book.PageCount = page_number;
                                    }; break;

                                case "ISBN":
                                    {
                                        var isbn = row
                                            .GetElementsByTagName("td").Skip(1).First()
                                            .TextContent;

                                        book.ISBN = isbn;
                                    }; break;
                            }

                            if (description.Equals("ISBN")) break;
                        }

                        book.BookName = book_title ?? "unnamed";

                        book.LiteraryWorks.Add(work);

                        lock (this)
                        {
                            books.Add(book);
                        }

                        Console.WriteLine($"Добавлено {books.Count}");
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"Ошибка #{errors.Count + 1}");
                        sb.AppendLine(oz_query_path);
                        sb.AppendLine(exc.Message + "\n");
                        errors.Add(sb.ToString());
                    }
                }
            });
            var bookRep = new BookRepository();
            var workRep = new LiteraryWorkRepository();
            Console.WriteLine($"Несуществующих работ {notExistingWork.Count}");

            await workRep.UpdateRangeAsync(notExistingWork);
            await bookRep.AddRangeAsync(books);


            using (var fw = File.CreateText("C:\\add_book_loger.log"))
            {
                fw.Write(String.Join('\n', errors));
            }

            Console.WriteLine($"Completed. Added {books.Count}. Errors: " + errors.Count);
        }

        private bool isAuthorNameCorrect(string? sourceString, LiteraryWork work)
        {
            if (sourceString is null) return false;

            return sourceString.Contains(work.Author.Name) || sourceString.Contains(work.Author?.ShortName ?? "ыыы");
        }

        private async Task SaveBookImage(string imagePath, string imageLink)
        {
            if (!File.Exists(imagePath)) Directory.CreateDirectory(imagePath);

            try
            {
                using (var fs = File.Create($"{imagePath}\\book_image.jpg"))
                {
                    var content = await _httpClient.GetAsync(imageLink);
                    await content.Content.CopyToAsync(fs);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("cant save book image");
                Console.WriteLine(exc.Message);
            }
        }
        
    }




}
