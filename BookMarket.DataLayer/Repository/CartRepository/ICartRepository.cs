using BookMarket.DataLayer.Models.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMarket.DataLayer.Repository.CartRepository
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByIdAsync(int id);
        Task<Cart?> GetCartByUserIdAsync(string index);
        Task<int> UpdateCartAsync(Cart cart);

        Cart? GetCartById(int id);


    }
}
