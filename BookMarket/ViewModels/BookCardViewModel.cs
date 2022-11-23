using Microsoft.EntityFrameworkCore.ChangeTracking;
using BookMarket.DataLayer.Models;

namespace BookMarket.ViewModels
{
    public class BookCardViewModel
    {
        private readonly string baseImagePath = @"/ImageData/Books/";
        private readonly string bookImageGuid;
        private readonly Book book = null!;

        public BookCardViewModel(PageInStore page)
        {
            book = page.Book;
            bookImageGuid = book.BookImageGuid;
            Author = string.Join(' ', book.LiteraryWorks.Select(w => w.Author.Name).Distinct());
            Title = book.BookName;
            Price = page.Price;
            Year = book?.Year ?? "";
            Id = page.Id;
        }


        public string BookImagePath => $"{baseImagePath}/{bookImageGuid}/book_image.jpg";
        public int Id { get; set; }
        public string Author { get; }
        public string Title { get; }
        public string? Year { get; }        
        public decimal Price { get; }
        public double Rating {
            get
            {
                var rate = book.LiteraryWorks.FirstOrDefault()?.Raiting?.Replace('.', ',');
                if (!String.IsNullOrEmpty(rate) && Double.TryParse(rate, out double res))
                {
                    return  res;
                }
                return 1.0;
            }
        }
    }
}
