using AngleSharp.Dom;
using BookMarket.DataLayer.Models;
using BookMarket.DataLayer.Repository;
using System.Text;

namespace BookMarket.DataCollectorLibrary
{
    public class WorkCollector : PageCollector
    {
        IEnumerable<Author> authors = null!;
        IEnumerable<string> workTypes = null!;

        private readonly AuthorRepository authorRepository;
        private readonly LiteraryWorkRepository litRepository;
        private readonly JenreRepository jenreRepository;
        private readonly object syncObject = new object();

        private readonly string fantlabMainPage = @"https://fantlab.ru";

        public WorkCollector(IEnumerable<Author> authors = null, IEnumerable<string> workTypes = null) : base(new HttpClient())
        {
            authorRepository = new AuthorRepository();
            litRepository = new LiteraryWorkRepository();
            jenreRepository = new JenreRepository();
            if (authors is not null) this.authors = authors.ToArray();

            this.workTypes = workTypes ?? new[] { "novel", "story", "shortstory" };
        }

        public async Task CollectWorksFromFantlab()
        {
            if (authors is null) authors = (await authorRepository.GetAll()).ToArray();

            var list = new List<LiteraryWork>();

            //сделать нормальное логирование
            var errors = new List<string>();

            int current = 1;
            int total = authors.Count();

            //await Parallel.ForEachAsync(authors, async (a, token) => { });

            foreach (var a in authors)
            {
                try
                {
                    //получаем страницу автора
                    var page = await GetHtmlDoc(a.FantlabPage);
                    foreach (string type in workTypes)
                    {
                        //получаем таблицу со списком произведений
                        var tbody = page.GetElementById($"{type}_info");
                        if (tbody is null) continue;

                        //получаем строки с данными о произведении и перебираем их
                        var works = tbody.GetElementsByTagName("tr").SkipLast(1).Select(r =>
                        {
                            LiteraryWork work = new() { AuthorId = a.Id, WorkType = type };
                            try
                            {
                                //перебираем ячейки в строке
                                var cells = r.GetElementsByTagName("td");

                                //в первой ячейке дата издания
                                work.Year = cells[0].GetElementsByTagName("font").First().TextContent;

                                //дальше ряд ссылок, в первой может быть ссылка на серию
                                var links = cells[0].GetElementsByTagName("a").ToArray();
                                var isItCycleLinkTitle = links[0].GetAttribute("title");
                                if (isItCycleLinkTitle?.Contains("входит в цикл") ?? false) links = links.Skip(1).ToArray();

                                //название и ссылка
                                work.Title = links.First().TextContent;
                                work.FantlabLink = links.First().GetAttribute("href");

                                //третья ячейка это оценка и колическо голосово. Берет только оценку - 4 знака
                                work.Raiting = cells[2].GetElementsByTagName("span").First().TextContent.Substring(0, 4);

                                //если во 2ой нет описания, то это что то левое
                                var description = cells[1].GetElementsByTagName("nobr").First().InnerHtml;
                                if (String.IsNullOrWhiteSpace(description)) work = null;
                            }
                            catch
                            {
                                return null;
                            }
                            return work;
                        }).ToList();

                        //убираем лишнее
                        works.RemoveAll(w => w == null || String.IsNullOrWhiteSpace(w.Title));
                        //добавляем произведения типа в выходной лист
                        list.AddRange(works);

                        if (list.Count > 100)
                        {
                            await litRepository.AddRangeAsync(list);
                            Console.WriteLine("Сотка внесена в базу");
                            list.Clear();
                        }

                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Проблема с автором {a.Name}");
                    Console.WriteLine(exc.Message);
                    Console.WriteLine(exc.InnerException?.Message);
                    Console.WriteLine();
                    errors.Add(a.Name);
                }

                Console.WriteLine($"Обработано авторов {current++} из {total}");
            };

            Console.WriteLine($"Ended with {errors.Count()}");
            using (var fs = File.CreateText("C:\\author_log.txt"))
            {
                fs.WriteLine(String.Join('\n', errors));
            }


        }

        public async Task SetJenreInfo()
        {
            var reviewRep = new ReviewRepository();

            var works = await litRepository.GetAll();

            int current = 1;
            int total = works.Count();

            var jenres = await jenreRepository.GetAll();
            var worksToUpdate = new List<LiteraryWork>();

            await Parallel.ForEachAsync(works, async (work, token) =>
            {
                Console.WriteLine($"Взято в обработку {current++} из {total}");

                AngleSharp.Html.Dom.IHtmlDocument doc;

                try
                {
                    doc = await GetHtmlDoc($"{fantlabMainPage}{work.FantlabLink}");
                    if (doc is null) throw new Exception("Пустой ответ");
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Исключение по адресу {fantlabMainPage}{work.FantlabLink}");
                    Console.WriteLine(exc.Message);
                    Console.WriteLine(exc.InnerException?.Message);
                    return;
                }

                try
                {
                    #region Jenre section
                    var jenreString = doc
                        .GetElementById("workclassif")?
                        .GetElementsByTagName("li").First()
                        .GetElementsByTagName("a").First()
                        .TextContent;

                    //в качестве заглушки чтобы не разбираться с каждой страницей
                    if (jenreString is null) jenreString = "Повесть";

                    var jenre = jenres.FirstOrDefault(j => j.JenreName.Equals(jenreString, StringComparison.CurrentCultureIgnoreCase));

                    if (jenre is null)
                    {
                        jenre = new Jenre() { JenreName = jenreString };
                        var jenRep = new JenreRepository();
                        await jenRep.AddAsync(jenre);
                        jenres = jenres.Append(jenre);
                    }


                    work.JenreId = jenre.Id;
                    worksToUpdate.Add(work);

                    //в цивилизованном мире так не делают
                    //работает в многопотоке и почти наверняка удалит лишнее
                    if (worksToUpdate.Count() > 200)
                    {
                        var lirRep = new LiteraryWorkRepository();
                        await lirRep.UpdateRangeAsync(worksToUpdate);
                        Console.WriteLine("Проверяй хозяин");
                        worksToUpdate.Clear();
                    }

                    #endregion
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Не удалось установить жанр {fantlabMainPage}{work.FantlabLink}");
                    Console.WriteLine(exc);
                }

                try
                {
                    #region Reviews
                    var reviews = doc.GetElementsByClassName("response-item");
                    if (reviews is not null && reviews.Count() > 0)
                    {
                        List<Review> reviewsList = new List<Review>();
                        foreach (var r in reviews)
                        {
                            Review review = new Review() { WorkID = work.Id };
                            var authorInfo = r.GetElementsByClassName("response-autor-info")?.FirstOrDefault();

                            //name
                            if (authorInfo is not null)
                                review.ReviewerName = authorInfo
                                        .GetElementsByTagName("b").First()
                                        .GetElementsByTagName("a").First()
                                        .TextContent;

                            //text??
                            var review_body = r.GetElementsByClassName("response-body-home").First().GetElementsByTagName("p");
                            StringBuilder sb = new();
                            foreach (var p in review_body)
                            {
                                sb.AppendLine(p.TextContent);
                            }
                            review.Text = sb.ToString();

                            //mark
                            var mark = r.GetElementsByClassName("response-autor-mark").First()
                                        .GetElementsByTagName("span").First().TextContent;
                            Int32.TryParse(mark, out int stars);
                            review.Stars = stars;

                            reviewsList.Add(review);
                        }

                        try
                        {
                            var revRep = new ReviewRepository();
                            await revRep.AddRangeAsync(reviewsList);
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("Cant add reviews");
                            Console.WriteLine(exc.Message);
                            Console.WriteLine(exc.InnerException?.Message);
                        }

                    }

                    #endregion
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Ошибка с отзывом {work.FantlabLink}");
                    Console.WriteLine(exc.Message);
                    Console.WriteLine(exc.InnerException?.Message);
                }
            });
        }

        public async Task SetAnnotationFromFL(IEnumerable<LiteraryWork> works)
        {
            await Parallel.ForEachAsync(works, async (work, token) =>
            {
                try
                {
                    var pagePath = $"{fantlabMainPage}{work.FantlabLink}";
                    var page = await GetHtmlDoc(pagePath);
                    var annotationElement = page.GetElementById("annotation-unit");

                    if (annotationElement is null) return;

                    string annotation = GetTextFromBlock(annotationElement);
                    work.Annotation = annotation;
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Cant add annotation");
                    Console.WriteLine(exc.Message);
                    Console.WriteLine(exc.InnerException?.Message);
                }

            });

            var wRep = new LiteraryWorkRepository();
            await wRep.UpdateRangeAsync(works);
        }

        public async Task SetJenreAnnotationReviews(IEnumerable<LiteraryWork> works = null)
        {
            if (works is null) works = (await new LiteraryWorkRepository().GetAll()).Where(w => String.IsNullOrEmpty(w.Annotation));

            var reviewRep = new ReviewRepository();            

            int current = 1;
            int total = works.Count();

            var jenres = await jenreRepository.GetAll();
            var worksToUpdate = new List<LiteraryWork>();

            await Parallel.ForEachAsync(works, async (work, token) =>
            {
                Console.WriteLine($"Взято в обработку {current++} из {total}");

                AngleSharp.Html.Dom.IHtmlDocument doc;

                try
                {
                    doc = await GetHtmlDoc($"{fantlabMainPage}{work.FantlabLink}");
                    if (doc is null) throw new Exception("Пустой ответ");
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Исключение по адресу {fantlabMainPage}{work.FantlabLink}");
                    Console.WriteLine(exc.Message);
                    Console.WriteLine(exc.InnerException?.Message);
                    return;
                }

                try
                {
                    #region Jenre section
                    var jenreString = doc
                        .GetElementById("workclassif")?
                        .GetElementsByTagName("li").First()
                        .GetElementsByTagName("a").First()
                        .TextContent;

                    //в качестве заглушки чтобы не разбираться с каждой страницей
                    if (jenreString is null) jenreString = "Повесть";

                    var jenre = jenres.FirstOrDefault(j => j.JenreName.Equals(jenreString, StringComparison.CurrentCultureIgnoreCase));

                    if (jenre is null)
                    {
                        //блокируем
                        lock (syncObject)
                        {
                            //проверяем не добавил ли другой поток жанр
                            jenre = jenres.FirstOrDefault(j => j.JenreName.Equals(jenreString, StringComparison.CurrentCultureIgnoreCase));
                            if (jenre is null)
                            {
                                jenre = new Jenre() { JenreName = jenreString };
                                var jenRep = new JenreRepository();
                                jenRep.Add(jenre);
                                jenres = jenres.Append(jenre);
                            }
                        }
                        
                    }


                    work.JenreId = jenre.Id;
                    worksToUpdate.Add(work);

                    //в цивилизованном мире так не делают
                    //работает в многопотоке и почти наверняка удалит лишнее
                    if (worksToUpdate.Count() > 200)
                    {
                        var lirRep = new LiteraryWorkRepository();
                        await lirRep.UpdateRangeAsync(worksToUpdate);
                        Console.WriteLine("Проверяй хозяин");
                        worksToUpdate.Clear();
                    }

                    #endregion
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Не удалось установить жанр {fantlabMainPage}{work.FantlabLink}");
                    Console.WriteLine(exc);
                }

                try
                {
                    #region Reviews
                    var reviews = doc.GetElementsByClassName("response-item");
                    if (reviews is not null && reviews.Count() > 0)
                    {
                        List<Review> reviewsList = new List<Review>();
                        foreach (var r in reviews)
                        {
                            Review review = new Review() { WorkID = work.Id };
                            var authorInfo = r.GetElementsByClassName("response-autor-info")?.FirstOrDefault();

                            //name
                            if (authorInfo is not null)
                                review.ReviewerName = authorInfo
                                        .GetElementsByTagName("b").First()
                                        .GetElementsByTagName("a").First()
                                        .TextContent;

                            //text??
                            var review_body = r.GetElementsByClassName("response-body-home").First().GetElementsByTagName("p");
                            StringBuilder sb = new();
                            foreach (var p in review_body)
                            {
                                sb.AppendLine(p.TextContent);
                            }
                            review.Text = sb.ToString();

                            //mark
                            var mark = r.GetElementsByClassName("response-autor-mark").First()
                                        .GetElementsByTagName("span").First().TextContent;
                            Int32.TryParse(mark, out int stars);
                            review.Stars = stars;

                            reviewsList.Add(review);
                        }

                        try
                        {
                            var revRep = new ReviewRepository();
                            await revRep.AddRangeAsync(reviewsList);
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("Cant add reviews");
                            Console.WriteLine(exc.Message);
                            Console.WriteLine(exc.InnerException?.Message);
                        }

                    }

                    #endregion
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Ошибка с отзывом {work.FantlabLink}");
                    Console.WriteLine(exc.Message);
                    Console.WriteLine(exc.InnerException?.Message);
                }

                try
                {
                    #region Annotation
                    var annotationElement = doc.GetElementById("annotation-unit");

                    if (annotationElement is null) return;

                    string annotation = GetTextFromBlock(annotationElement);
                    work.Annotation = annotation;
                    #endregion
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Cant add annotation");
                    Console.WriteLine(exc.Message);
                    Console.WriteLine(exc.InnerException?.Message);
                }

            });
        }

        private string GetTextFromBlock(IElement element)
        {
            lock (this)
            {
                var responseElement = element
                    .GetElementsByClassName("responses-list")?
                    .FirstOrDefault()?
                    .GetElementsByTagName("p");

                if (responseElement is null) return "";

                var sb = new StringBuilder();

                foreach (var p in responseElement)
                {
                    sb.AppendLine(p.TextContent.Trim());
                }

                return sb.ToString();
            }
        }
    }
}
