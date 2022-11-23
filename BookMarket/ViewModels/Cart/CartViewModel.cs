using BookMarket.DataLayer.Models;
using BookMarket.DataLayer.Repository.UserRepository;
using mc = BookMarket.DataLayer.Models.Cart;

namespace BookMarket.ViewModels.Cart
{
    public class CartViewModel
    {        
        private readonly mc.Cart cart;
        private readonly PageInStore[] pages;

        public CartViewModel(mc.Cart cart, IEnumerable<PageInStore> pages)
        {
            this.cart = cart;
            this.pages = pages.ToArray();
        }

        public IEnumerable<CartItemViewModel> GetCartItemViewModels()
        {
            var result_list = new List<CartItemViewModel>();
            for (int i = 0, j = pages.Count(); i < j; i++)
            {
                result_list.Add(new CartItemViewModel(new BookCardViewModel(pages[i]))
                {
                    Quantity = cart.Items[i].ItemQuantity
                });
            }
            return result_list;
        }

        public string CartPrice => pages.Sum(p => p.Price) + " руб.";
    }
}
