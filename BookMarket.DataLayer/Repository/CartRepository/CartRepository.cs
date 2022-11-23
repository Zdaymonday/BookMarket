using BookMarket.DataLayer.Context;
using BookMarket.DataLayer.Models.Cart;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMarket.DataLayer.Repository.CartRepository
{
    public class CartRepository : ICartRepository
    {
        #region Head
        private readonly CartContext ctx;

        public CartRepository()
        {
            ctx = new CartContext();
        }
        #endregion

        public async Task<Cart?> GetCartByIdAsync(int id)
        {
            return await ctx.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> UpdateCartAsync(Cart cart)
        {
            ctx.Carts.Update(cart);
            return await ctx.SaveChangesAsync();
        }

        public async Task<Cart?> GetCartByUserIdAsync(string index)
        {
            return await ctx.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.OwnerId == index);
        }

        public Cart? GetCartById(int id)
        {
            throw new NotImplementedException();
        }

        
    }
}
