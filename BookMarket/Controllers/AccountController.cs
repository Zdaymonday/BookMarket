using BookMarket.DataLayer.Models.Cart;
using BookMarket.DataLayer.Repository.OrderRepository;
using BookMarket.DataLayer.Repository.UserRepository;
using BookMarket.Infrastracture;
using BookMarket.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly CartService cartService;
        private readonly IOrderRepository orderRepository;
        private readonly IUserRepository userRepository;

        public AccountController(IUserRepository userRepository, IOrderRepository orderRepository, CartService cartService, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.cartService = cartService;
            this.orderRepository = orderRepository;
            this.userRepository = userRepository;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if (await userManager.IsInRoleAsync(user, "admin")) return RedirectToAction("Index", "Admin");
            
            var orders = await orderRepository.GetOrdersByUserId(user.Id);
            return View(new UserOrdersViewModel(orders, userRepository));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);
            var profile = new UserProfileViewModel()
            {
                UserLogin = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
            return View(profile);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isLoginCorrect = await checkLogin(model.UserLogin);
                bool isEmailCorrect = await checkEmail(model.Email);
                bool isPhoneCorrect = await checkPhone(model.PhoneNumber);
                if(isEmailCorrect && isLoginCorrect && isPhoneCorrect)
                {
                    var user = await userManager.GetUserAsync(User);
                    user.Email = model.Email;
                    user.UserName = model.UserLogin;
                    user.PhoneNumber = model.PhoneNumber;
                    await userManager.UpdateAsync(user);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    if (!isEmailCorrect) ModelState.AddModelError("", "Указанный адрес уже используется");
                    if (!isLoginCorrect) ModelState.AddModelError("", "Указанное имя уже используется");
                    if (!isPhoneCorrect) ModelState.AddModelError("", "Указанный номер уже используется");
                }
                
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                await signInManager.SignOutAsync();
                var signInResult = await signInManager.PasswordSignInAsync(model.UserName, model.Password, true, false);
                if (signInResult.Succeeded)
                {
                    if (String.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неверный логин и/или пароль");
                }
            }
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = "")
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Cart? unregister_user_cart = null;
                if (!await isCurrentUserAuthenticated()) unregister_user_cart = await cartService.GetCart();

                var user = new IdentityUser() { UserName = model.Name, Email = model.Email };
                var createRes = await userManager.CreateAsync(user, model.Password);
                if (createRes.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);

                    if (unregister_user_cart is not null && unregister_user_cart.Items.Count() > 0)
                    {
                        unregister_user_cart.OwnerId = user.Id;
                        await cartService.UpdateCart(unregister_user_cart);
                    }

                    return RedirectToAction(nameof(Index), "Home");
                }
                else
                {
                    foreach (var e in createRes.Errors)
                    {
                        ModelState.AddModelError("", e.Description);
                    }
                }
            }
            return View();
        }

        public IActionResult RecoverForm()
        {
            return View();
        }

        public async Task<IActionResult> CheckEmail(string email)
        {
            return Json(await checkEmail(email));
        }

        public async Task<IActionResult> CheckLogin(string login)
        {
            return Json(await checkLogin(login));
        }

        public async Task<IActionResult> CheckPhone(string phone)
        {
            return Json(await checkPhone(phone));
        }

        private async Task<bool> isCurrentUserAuthenticated()
        {
            return await userManager.GetUserAsync(HttpContext.User) is not null;
        }

        private async Task<bool> checkEmail(string email)
        {
            var result = await userManager.FindByEmailAsync(email);
            var current = await userManager.GetUserAsync(User);
            return result is null || current.Email == email;
        }

        private async Task<bool> checkLogin(string login)
        {
            var result = await userManager.Users.Where(u => u.UserName == login).FirstOrDefaultAsync();
            var current = await userManager.GetUserAsync(User);
            return result is null || current.UserName == login;
        }

        private async Task<bool> checkPhone(string phone)
        {
            var result = await userManager.Users.Where(u => u.PhoneNumber == phone).FirstOrDefaultAsync();
            var current = await userManager.GetUserAsync(User);
            return result is null || current.PhoneNumber == phone;
        }
    }
}
