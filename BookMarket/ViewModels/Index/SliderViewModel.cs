using BookMarket.DataLayer.Models;

namespace BookMarket.ViewModels.Index
{
    public class SliderViewModel
    {
        private readonly IEnumerable<PageInStore> pages;
        

        public SliderViewModel(IEnumerable<PageInStore> pages, string title)
        {
            this.pages = pages;
            this.Title = title;
        }

        public string Title { get; }

        public IEnumerable<BookCardViewModel> BookCards => pages.Select(b => new BookCardViewModel(b));

        
    }
}
