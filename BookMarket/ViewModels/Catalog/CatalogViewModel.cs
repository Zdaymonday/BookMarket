using BookMarket.DataLayer.Models;
using BookMarket.DataLayer.Repository.UserRepository;

namespace BookMarket.ViewModels.Catalog
{
    public class CatalogViewModel
    {
        private readonly IEnumerable<PageInStore> pages;
        private readonly PageViewModel pageModel;

        public CatalogViewModel(PageViewModel pageModel, IEnumerable<PageInStore> pages)
        {
            this.pages = pages;
            this.pageModel = pageModel;
        }

        public IEnumerable<BookCardViewModel> BookCards
        {
            get
            {
                foreach (var book in pages) yield return new(book);
            }
        }

        public PageViewModel PageViewModel => pageModel;
    }
}
