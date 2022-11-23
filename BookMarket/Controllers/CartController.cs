using BookMarket.DataLayer.Models.Cart;
using BookMarket.DataLayer.Repository.OrderRepository;
using BookMarket.DataLayer.Repository.UserRepository;
using BookMarket.Infrastracture;
using BookMarket.ViewModels.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookMarket.Controllers
{
    public class CartController : Controller
    {
        #region Constructor
        private readonly UserManager<IdentityUser> userManager;
        private readonly IUserRepository userRepository;
        private readonly CartService cartService;
        private readonly IOrderRepository orderRepository;

        public CartController(
            IUserRepository userRepository,
            IOrderRepository orderRepository,
            UserManager<IdentityUser> userManager,
            CartService cartService)
        {
            this.userManager = userManager;
            this.userRepository = userRepository;
            this.cartService = cartService;
            this.orderRepository = orderRepository;
        }
        #endregion

        public async Task<IActionResult> Index()
        {
            Cart cart = await cartService.GetCart();
            var pages = await userRepository.GetPagesByIdAsync(cart!.Items.Select(i => i.PageId));
            return View(new CartViewModel(cart, pages));
        }

        public async Task<IActionResult> AddToCart(int page_id, string? returnUrl = null)
        {
            await cartService.AddToCart(page_id);
            if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Remove(int pageId)
        {
            await cartService.RemoveFromCart(pageId);
            return RedirectToAction("Index", "Cart");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PlaceAnOrder()
        {
            var user = await userManager.GetUserAsync(User);
            return View(new OrderViewModel() {UserId=user.Id, UserEmail = user.Email, UserPhone = user.PhoneNumber });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PlaceAnOrder(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var cart = await cartService.GetCart();
                var items = cart.Items
                    .Select(i => new OrderPosition() { PageId = i.PageId, ItemQuantity = i.ItemQuantity })
                    .ToList();
                var order = new Order()
                {
                    OwnerId = model.UserId,
                    Name = model.Name,
                    Address = model.Address,
                    Items = items,
                    PaymentStatus = PaymentStatus.Unpaid,
                    OrderStatus = OrderStatus.Accepted,
                };

                await orderRepository.CreateNewOrderAsync(order);
                return RedirectToAction("Index", "Account");
            }
            return View(model);
        }
    }
}
