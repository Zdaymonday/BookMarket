using Microsoft.AspNetCore.Mvc.RazorPages;
using BookMarket.DataLayer.Models;

namespace BookMarket.ViewModels.Catalog
{
    public class BookViewModel : BookCardViewModel
    {
        public BookViewModel(PageInStore page, IEnumerable<Review> reviews) : base(page)
        {
            Reviews = reviews;
            Anntotation = page.Book.LiteraryWorks.First().Annotation ?? "No annotation";
        }

        public IEnumerable<Review> Reviews { get; }
        public string Anntotation { get; }
    }
}
