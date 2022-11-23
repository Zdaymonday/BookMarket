using BookMarket.DataLayer.Models.Cart;
using BookMarket.DataLayer.Repository.CartRepository;
using BookMarket.DataLayer.Repository.UserRepository;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BookMarket.ViewModels.Account
{
    public class UserOrdersViewModel
    {
        private readonly IEnumerable<Order> orders;
        private readonly IUserRepository repository;
        

        public UserOrdersViewModel(IEnumerable<Order> orders, IUserRepository repository)
        {
            this.repository = repository;
            this.orders = orders;
        }


        public async Task<IEnumerable<OrderViewModel>> GetOrders()
        {
            var list = new List<OrderViewModel>();
            foreach(var order in orders)
            {
                var model = new OrderViewModel();
                model.Id = order.Id;
                var idSet = order.Items.Select(i => i.PageId);
                var pages = await repository.GetPagesByIdAsync(idSet);
                model.Items = pages.Select(p => $"{p.Book.BookName} - {p.Book.LiteraryWorks.First().Author.Name}");
                model.Quantities = order.Items.Select(i => i.ItemQuantity);
                model.OrderStatus = order.OrderStatus.ToString();
                model.PaymentStatus = order.PaymentStatus.ToString();
                list.Add(model);
            }
            return list;
        }
    }

    public class OrderViewModel
    {
        public int Id { get; set; }
        public IEnumerable<string> Items { get; set; } = null!;
        public IEnumerable<int> Quantities { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;
    }
}
