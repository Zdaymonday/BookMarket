using BookMarket.DataLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace BookMarket.ViewModels.Admin
{
    public class BooksViewModel
    {
        public IEnumerable<BookSlimViewModel> Books { get; set; } = null!;
        public PageViewModel PageViewModel { get; set; } = null!;
    }

    public class BookSlimViewModel
    {
        public int PageId { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string Price { get; set; } = null!;
        public BookSlimViewModel(int pageId, string title, string author, string price)
        {
            PageId = pageId;
            Title = title;
            Author = author;
            Price = price;
        }
        public BookSlimViewModel() : this(-1, "", "", "") { }
    }


    public class BookFullViewModel
    {
        public BookFullViewModel(PageInStore page, string returnUrl)
        {
            PageId = page.Id;
            ShopName = page.ShopName;
            BookPage = page.BookPage;
            Price = page.Price.ToString("##.## руб.");

            BookId = page.BookId;
            ISBN = page.Book.ISBN;
            BookName = page.Book.LiteraryWorks.First().Title;
            PublicationDate = page.Book?.Year ?? "";
            PageCount = page.Book?.PageCount ?? 0;
            BookImageGuid = page.Book!.BookImageGuid;

            PublisherId = page.Book.PublisherId;
            PublisherName = page.Book.Publisher.Name;
            PublisherFullName = page.Book.Publisher?.FullName ?? "";
            Country = page.Book.Publisher?.Country ?? "";
            City = page.Book.Publisher?.City ?? "";
            FoundationDate = page.Book.Publisher?.Year ?? "";

            ReturnUrl = returnUrl;
        }

        public BookFullViewModel() { }

        public int PageId { get; set; }
        public string ShopName { get; set; }
        public string BookPage { get; set; }
        public string Price { get; set; }

        public int BookId { get; set; }
        public string ISBN { get; set; }
        public string? BookName { get; set; }
        public string PublicationDate { get; set; }
        public int PageCount { get; set; }
        public string BookImageGuid { get; set; }
        
        public int PublisherId { get; set; }
        public string PublisherName { get; set; }
        public string? PublisherFullName { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? FoundationDate { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
