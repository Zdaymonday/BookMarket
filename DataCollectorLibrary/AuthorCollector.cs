using BookMarket.DataLayer.Models;
using BookMarket.DataLayer.Context;
using BookMarket.DataLayer.Repository;
using System.Collections.Generic;
using AngleSharp.Dom;
using System.Text;

namespace BookMarket.DataCollectorLibrary
{
    public class AuthorCollector : PageCollector
    {
        private readonly AuthorRepository repository;

        public AuthorCollector() : base(new HttpClient())
        {
            repository = new AuthorRepository();
        }

        public async Task CollectFromFantlab(int skip = 0)
        {
            var doc = await GetHtmlDoc(@"https://fantlab.ru/ratings?all=1");
            string main = @"https://fantlab.ru";
            List<Author> authors = new();

            var table = doc
                .GetElementsByTagName("main")
                .First().GetElementsByTagName("table")
                .Skip(2)
                .First();

            var rows = table.GetElementsByTagName("tr").Skip(skip);
            //сделать асинхронным?
            foreach (var row in rows)            
            {
                //у строки с автором есть параметр valign="top"
                var valign = row.GetAttribute("valign");
                if (string.IsNullOrEmpty(valign) || !valign.Equals("top")) continue;

                Author author = new Author();
                try
                {
                    var data = row
                        .GetElementsByTagName("td")
                        .Skip(1)
                        .First()
                        .GetElementsByTagName("a")
                        .First();

                    author.Name = data.TextContent;

                    string page = data.GetAttribute("href")!;

                    author.FantlabPage = main + page;

                    var personalPage = await GetHtmlDoc(author.FantlabPage);

                    var infoRows = personalPage
                        .GetElementById("autor_info")!
                        .GetElementsByTagName("table")
                        .Skip(2)
                        .First()
                        .GetElementsByTagName("tr");

                    author.Country = infoRows[0]
                        .GetElementsByTagName("td")
                        .Skip(1)
                        .First()
                        .GetElementsByTagName("span")
                        .First()
                        .TextContent;

                    author.BirthDate = infoRows[1]
                        .GetElementsByTagName("td")
                        .Skip(1)
                        .First()
                        .GetElementsByTagName("meta")
                        .First()
                        .GetAttribute("content")!;

                    if (infoRows.Count() > 2)
                    {
                        author.DeathDate = infoRows[2]
                        .GetElementsByTagName("td")
                        .Skip(1)
                        .First()
                        .GetElementsByTagName("meta")
                        .First()
                        .GetAttribute("content")!;
                    }
                }
                catch
                {
                    author = null;
                }
                if (author is not null)
                    authors.Add(author);
                if (authors.Count > 100)
                {
                    await repository.AddRangeAsync(authors);
                    //логировать
                    Console.WriteLine("Сотня авторов добавлена в базу");
                    authors.Clear();
                }
                //позднее убрать или поток вывсести в другое место
                Console.WriteLine($"Обработано {rows.Index(row)} из {rows.Count()}");

            }

            await repository.AddRangeAsync(authors);

        }

        public async Task AddBiographyAndFoto()
        {
            string fantlabMainPage = @"https://fantlab.ru";
            var authors = await repository.GetAll();
            authors = authors.Where(a => String.IsNullOrEmpty(a.Biography));

            int current = 1;
            var total = authors.Count();

            //получаем страницу автора
            foreach (var a in authors)
            {
                var page = await GetHtmlDoc(a.FantlabPage);

                //биография
                var bio = page.GetElementsByClassName("person-info-bio").First();
                if (bio is not null && String.IsNullOrEmpty(a.Biography))
                {
                    StringBuilder sb = new();
                    var p_blocks = bio.GetElementsByTagName("p");
                    foreach (var p in p_blocks)
                    {
                        sb.AppendLine(p.TextContent);
                    }

                    a.Biography = sb.ToString();
                }

                //фото
                string? src = page.GetElementById("autor_info")?.GetElementsByTagName("img")?.First()?.GetAttribute("src");
                if (src is not null)
                {
                    var response = await _httpClient.GetAsync($"{fantlabMainPage}{src}");
                    if (response.IsSuccessStatusCode)
                    {
                        string filePath = $"E:\\ImageData\\{a.AuthorImageGuid}";
                        try
                        {
                            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                            using (var fs = File.Create($"{filePath}\\foto.jpg"))
                            {
                                await response.Content.CopyToAsync(fs);
                            }
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine($"Не удалось сохранить фото {a.Name}");
                            Console.WriteLine(exc.Message);
                        }
                    }
                }
                Console.WriteLine($"Обработано {current++} from {total}");
            }

            await repository.UpdateRangeAsync(authors);
        }

    }

}
