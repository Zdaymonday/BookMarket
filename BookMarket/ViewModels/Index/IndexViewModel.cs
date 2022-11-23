using BookMarket.DataLayer.Context;
using BookMarket.DataLayer.Models;

namespace BookMarket.ViewModels.Index
{
    public class IndexViewModel
    {
        public IDictionary<string, IEnumerable<PageInStore>> pages_for_slider { get; }

        public IndexViewModel(IDictionary<string, IEnumerable<PageInStore>> pages)
        {
            pages_for_slider = pages;
        }
    }
}
