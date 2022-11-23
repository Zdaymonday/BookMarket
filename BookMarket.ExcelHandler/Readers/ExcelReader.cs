using BookMarket.DataLayer.Models;
using BookMarket.DataLayer.Repository.AdminRepository;
using BookMarket.ExcelHandler.Interfaces;
using ClosedXML.Excel;

namespace BookMarket.ExcelHandler.Readers
{
    public class ExcelHandler : IExcelFileReader
    {
        private readonly UploadRepository repository;

        public ExcelHandler(UploadRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<PageInStore>> GetPagesFromFileAsync(string path)
        {
            IXLRows rows = null!;
            XLWorkbook wb = null!;
            IXLWorksheet ws = null!;
            try
            {
                wb = new XLWorkbook(path);
                ws = wb.Worksheet(1);
                rows = ws.RowsUsed();
            }
            catch
            {
                throw new Exception("Cant create table from file");
            }

            var page_list = new List<PageInStore>();

            foreach (var r in rows)
            {
                if (r.Cell(1).GetString().Contains("ShopName")) continue;

                var page = new PageInStore();

                page.ShopName = r.Cell(1).GetString();
                page.BookPage = r.Cell(2).GetString();
                page.Price = decimal.TryParse(r.Cell(3).GetString(), out decimal val) ? val : 0M;
                page.InStock = page.Price > 0;

                string isbn = r.Cell(4).GetString();

                if (await repository.IsBookExist(isbn))
                {
                    page.Book = await repository.GetBookAsync(isbn);
                    page_list.Add(page);
                    continue;
                }

                string[] titles = r.Cell(5).GetString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                string[] authors_names = r.Cell(6).GetString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                string[] years = r.Cell(7).GetString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                string[] jenres_names = r.Cell(11).GetString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                string bookname = r.Cell(10).GetString();

                IDictionary<string, LiteraryWork> works;
                IDictionary<string, Author> authors;
                IDictionary<string, Jenre> jenres;

                try
                {
                    works = await repository.GetWorksAsync(titles);
                    authors = await repository.GetAuthorsAsync(authors_names);
                    jenres = await repository.GetJenresAsync(jenres_names);
                }
                catch
                {
                    //TODO: логгер
                    continue;
                }

                var book = new Book();

                for (int i = 0; i < titles.Count(); i++)
                {
                    var work = works[titles[i]];
                    var author = authors[authors_names[i]];
                    var jenre = jenres[jenres_names[i]];

                    work.Jenre = jenre;
                    if (work.Author is null) work.Author = author;
                    if (string.IsNullOrWhiteSpace(work.Year)) work.Year = years[i];

                    book.LiteraryWorks.Add(work);
                }

                book.ISBN = isbn;
                book.Publisher = await repository.GetPublisherAsync(r.Cell(8).GetString());
                book.Year = r.Cell(9).GetString();
                book.BookName = String.IsNullOrEmpty(bookname) ? bookname : book.LiteraryWorks.First().Title;
                

                page.Book = book;

                page_list.Add(page);
            }

            return page_list;
        }
    }
}