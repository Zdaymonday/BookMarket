using BookMarket.DataLayer.Models;
using BookMarket.DataLayer.Models.Cart;
using BookMarket.DataLayer.Repository;

namespace BookMarket.ViewModels.Account
{
    public class OrderListViewModel
    {
        public PageViewModel PaginationModel { get; }

        private IEnumerable<PageInStore> pages { get; }
        private IEnumerable<Order> orders { get; }

        public OrderListViewModel(IEnumerable<Order> orders, IEnumerable<PageInStore> pages, PageViewModel paginationModel)
        {
            this.PaginationModel = paginationModel;
            this.pages = pages;
            this.orders = orders;
        }

        public IEnumerable<OrderViewModel> GetOrders()
        {
            var list = new List<OrderViewModel>();
            foreach (var order in orders)
            {
                var model = new OrderViewModel();
                model.Id = order.Id;
                model.Items = pages.Select(p => $"{p.Book.BookName} - {p.Book.LiteraryWorks.First().Author.Name}");
                model.Quantities = order.Items.Select(i => i.ItemQuantity);
                model.OrderStatus = order.OrderStatus.ToString();
                model.PaymentStatus = order.PaymentStatus.ToString();
                list.Add(model);
            }
            return list;
        }
    }
}
