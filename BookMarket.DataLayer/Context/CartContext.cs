using BookMarket.DataLayer.Models.Cart;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.DataLayer.Context
{
    public class CartContext : DbContext
    {
        public CartContext()
        {

        }

        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItem { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var connString = Config.GetConnectionStringFromJson("cart_data");
            builder.UseSqlServer(connString);
        }
    }
}
