using BookMarket.DataLayer.Models.Cart;

namespace BookMarket.DataLayer.Repository.OrderRepository
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetOrdersByUserId(string id);
        Task<int> GetNumOfOrders();
        Task<IEnumerable<Order>> GetOrderList(int skip, int take);
        Task UpdateOrderAsync(Order order);
        Task CreateNewOrderAsync(Order order);
    }
}
