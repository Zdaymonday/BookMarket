using BookMarket.DataLayer.Models.Cart;
using BookMarket.DataLayer.Repository.CartRepository;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace BookMarket.Infrastracture
{
    public class CartService
    {
        #region Constuctor
        private readonly IMemoryCache memoryCache;
        private readonly ICartRepository cartRepository;
        private readonly UserManager<IdentityUser> userManager;
        private readonly HttpContext httpContext;

        public CartService(
            IMemoryCache memoryCache, 
            ICartRepository cartRepository, 
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            this.memoryCache = memoryCache;
            this.cartRepository = cartRepository;
            this.userManager = userManager;
            this.httpContext = httpContextAccessor.HttpContext!;
        }
        #endregion

        public async Task<int> GetNumOfItemsInCart()
        {
            var user_claim = httpContext.User;
            
            Cart? cart = null;
            if(httpContext.User?.Identity?.IsAuthenticated ?? false)
            {
                var user = await userManager.GetUserAsync(user_claim);
                cart = await cartRepository.GetCartByUserIdAsync(user.Id);
                if (cart is null)
                {
                    cart = new Cart() { OwnerId = user.Id };
                    await cartRepository.UpdateCartAsync(cart);
                }                
            }
            else
            {
                var session_guid = httpContext.Session.GetString("GUID")!;
                cart = (Cart)memoryCache.Get(session_guid);
                if (cart is null)
                {
                    cart = new Cart() { OwnerId = session_guid };
                    memoryCache.Set(session_guid, cart);
                }
            }

            return cart?.Items.Count() ?? 0;
        }

        public async Task<Cart> GetCart()
        {
            Cart cart = null!;
            if (await isCurrentUserAuthenticated())
            {
                var user = await userManager.GetUserAsync(httpContext.User);
                cart = (await cartRepository.GetCartByUserIdAsync(user.Id))!;
                if (cart is null)
                {
                    cart = new Cart() { OwnerId = user.Id };
                    await cartRepository.UpdateCartAsync(cart);
                }
            }
            else
            {
                var session_guid = httpContext.Session.GetString("GUID")!;
                cart = (Cart)memoryCache.Get(session_guid);
                if (cart is null)
                {
                    cart = new Cart() { OwnerId = session_guid };
                    memoryCache.Set(session_guid, cart);
                }
            }

            return cart;
        }

        public async Task AddToCart(int page_id)
        {
            var cart = await GetCart();
            cart.Items.Add(new CartItem() { ItemQuantity = 1, PageId = page_id });
            await cartRepository.UpdateCartAsync(cart);
        }

        public async Task UpdateCart(Cart cart)
        {
            await cartRepository.UpdateCartAsync(cart);
        }

        public async Task RemoveFromCart(int pageId)
        {
            var cart = await GetCart();
            cart.Items.RemoveAll(i => i.PageId == pageId);
            if (httpContext.User?.Identity?.IsAuthenticated ?? false)
            {
                await cartRepository.UpdateCartAsync(cart);
            }
        }

        private async Task<bool> isCurrentUserAuthenticated()
        {
            return await userManager.GetUserAsync(httpContext.User) is not null;
        }
    }
}
