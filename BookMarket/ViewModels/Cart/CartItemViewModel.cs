using BookMarket.DataLayer.Models;

namespace BookMarket.ViewModels.Cart
{
    public class CartItemViewModel
    {
        private BookCardViewModel bookCardViewModel { get; set; } = null!;

        public CartItemViewModel(BookCardViewModel bookCardViewModel)
        {
            this.bookCardViewModel = bookCardViewModel;
        }

        public int PageId => bookCardViewModel.Id;
        public string ImagePath => bookCardViewModel.BookImagePath;
        public string Title => bookCardViewModel.Title;
        public int Quantity { get; set; }
        public string Price => bookCardViewModel.Price + " руб.";
        public string TotalPrice => bookCardViewModel.Price * Quantity + " руб.";
        public string Author => bookCardViewModel.Author;
        public string Year => bookCardViewModel.Year ?? "";
    }
}
