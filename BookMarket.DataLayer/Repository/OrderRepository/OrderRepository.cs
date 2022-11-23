using BookMarket.DataLayer.Context;
using BookMarket.DataLayer.Models.Cart;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.DataLayer.Repository.OrderRepository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CartContext ctx;

        public OrderRepository()
        {
            this.ctx = new CartContext();
        }

        public async Task CreateNewOrderAsync(Order order)
        {;
            await ctx.Orders.AddAsync(order);
            var user_cart = await ctx.Carts.Include(c => c.Items).FirstAsync(c => c.OwnerId == order.OwnerId);
            ctx.Carts.Remove(user_cart);
            await ctx.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await ctx.Orders.Include(o => o.Items).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Order>> GetOrderList(int skip, int take)
        {
            return await ctx.Orders.Include(o => o.Items).Skip(skip).Take(take).ToArrayAsync();
        }

        public async Task<int> GetNumOfOrders() => await ctx.Orders.CountAsync();

        public async Task<IEnumerable<Order>> GetOrdersByUserId(string id)
        {
            return await ctx.Orders.Include(o => o.Items).Where(o => o.OwnerId == id).ToArrayAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            ctx.Orders.Update(order);
            await ctx.SaveChangesAsync();
        }
    }
}
