using BookMarket.IdentityServer.Context;
using BookMarket.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Xml.Linq;
using BookMarket.DataLayer.Repository.AdminRepository;
using BookMarket.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using BookMarket.ExcelHandler.Interfaces;
using DocumentFormat.OpenXml.InkML;
using BookMarket.DataLayer.Repository.OrderRepository;
using BookMarket.ViewModels.Account;
using BookMarket.DataLayer.Repository.UserRepository;
using BookMarket.DataLayer.Models.Cart;

namespace BookMarket.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        #region Header
        private readonly BookMarketIdentityContext ctx;
        private readonly AdminRepository adminRepository;
        private readonly IUserRepository userRepository;
        private readonly IOrderRepository orderRepository;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IExcelFileReader excelReader;

        public AdminController(
            AdminRepository adminRepository,
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            BookMarketIdentityContext context, 
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            IExcelFileReader excelReader)
        {
            ctx = context;
            this.adminRepository = adminRepository;
            this.orderRepository = orderRepository;
            this.userRepository = userRepository;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.excelReader = excelReader;
        }

        #endregion

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var user_view_models = await ctx.Users
                .Select(u => new UserViewModel(u.Id, u.UserName, u.Email))
                .ToArrayAsync();

            var all_roles = await ctx.Roles.ToArrayAsync();

            foreach (var user in user_view_models)
            {
                var roles_id = await ctx.UserRoles
                    .Where(ur => ur.UserId == user.UserId)
                    .Select(ur => ur.RoleId)
                    .ToArrayAsync();

                foreach (var role_id in roles_id)
                {
                    user.UserRoles.Add(all_roles.First(role => role.Id == role_id).Name);
                }
            }

            return View(user_view_models);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var identity_user = await userManager.FindByIdAsync(id);
            var view_model = new EditUserViewModel(id, identity_user!.UserName, identity_user.Email);

            var user_roles = await GetUserRoles(id);
            var all_roles = await roleManager.Roles.Select(r => r.Name).ToArrayAsync();

            view_model.UserRoles.AddRange(user_roles);
            view_model.AvailableRoles.AddRange(all_roles);

            return View(view_model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user is not null)
            {
                user.UserName = model.UserName;
                user.Email = model.UserEmail;
                var current_roles = await GetUserRoles(user.Id);
                var new_role_list = model.UserRoles;
                var roles_to_add = new_role_list.Except(current_roles);
                var roles_to_remove = current_roles.Except(new_role_list);
               
                await userManager.AddToRolesAsync(user, roles_to_add);
                await userManager.RemoveFromRolesAsync(user, roles_to_remove);

                await userManager.UpdateAsync(user);
            }
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> Roles()
        {
            var role_models = await roleManager.Roles.Select(r => new RoleViewModel() { Id = r.Id, Name = r.Name }).ToArrayAsync();
            return View(role_models);       
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if(role is not null)
            {
                var model = new RoleViewModel() { Id = role.Id, Name = role.Name };
                return View(model);
            }
            return RedirectToAction(nameof(Roles));
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(RoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (ModelState.IsValid && role is not null)
            {
                var op_result = await roleManager.SetRoleNameAsync(role, model.Name);
                if (op_result.Succeeded)
                {
                    return RedirectToAction(nameof(Roles));
                }
                ModelState.AddModelError("", "The role name cannot be set");
            }
            ModelState.AddModelError("", "Role is not exist");
            return View(model);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            if (!String.IsNullOrWhiteSpace(model.Name) && !(await roleManager.RoleExistsAsync(model.Name)))
            {
                var role = new IdentityRole(model.Name);
                var op_res = await roleManager.CreateAsync(role);
                if (op_res.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Something wrong");
            }
            else
            {
                ModelState.AddModelError("", "Incorrect name");
            }
            return View(model); 
        }

        [HttpGet]
        public IActionResult Books(string redirectUrl)
        {
            if (!String.IsNullOrWhiteSpace(redirectUrl) && Url.IsLocalUrl(redirectUrl))
                return Redirect(redirectUrl);
            return View();
        }

        public async Task<IActionResult> SearchByAuthorName(int page = 1, string context = "")
        {
            int items_per_page = 10;
            int count = await adminRepository.GetNumOfBooksByAuthorName(context);
            int to_skip = (page - 1) * items_per_page;

            var page_model = new PageViewModel(page, count, items_per_page, context);            
            var pages = await adminRepository.GetBooksByAuthorName(context, to_skip, items_per_page);

            var books_model = new BooksViewModel();
            books_model.PageViewModel = page_model;
            books_model.Books = pages.Select(p => new BookSlimViewModel(
                p.Id, p.GetBookName(), p.GetBookAuthorName(), $"{p.Price} руб."));

            return View("BookSearchResult", books_model);
        }

        public async Task<IActionResult> SearchBookByTitle(int page = 1, string context = "")
        {
            int items_per_page = 10;
            int count = await adminRepository.GetNumOfBooksByTitle(context);
            int to_skip = (page - 1) * items_per_page;

            var page_model = new PageViewModel(page, count, items_per_page, context);            
            var pages = await adminRepository.GetBooksByTitle(context, to_skip, items_per_page);

            var books_model = new BooksViewModel();
            books_model.PageViewModel = page_model;
            books_model.Books = pages.Select(p => new BookSlimViewModel(
                p.Id, p.GetBookName(), p.GetBookAuthorName(), $"{p.Price} руб."));

            return View("BookSearchResult", books_model);
        }

        [HttpGet]
        public async Task<IActionResult> EditBook(int id, string retunrUrl)
        {
            var page = await adminRepository.GetBookById(id);
            if (page is null) return View("Not Found");
            return View(new BookFullViewModel(page, retunrUrl));
        }

        [HttpPost]
        public async Task<IActionResult> EditBook(BookFullViewModel model)
        {
            var page = await adminRepository.GetBookById(model.PageId);
            if (ModelState.IsValid && page is not null)
            {
                page.ShopName = model.ShopName;
                page.Price = decimal.TryParse(model.Price.Split().First(), out decimal val) ? val : 0M;
                page.BookPage = model.BookPage;

                page.Book.ISBN = model.ISBN;
                page.Book.Year = model.PublicationDate;
                page.Book.PageCount = model.PageCount;
                page.Book.BookImageGuid = model.BookImageGuid;

                page.Book.Publisher.Name = model.PublisherName;
                page.Book.Publisher.FullName = model.PublisherFullName;
                page.Book.Publisher.Country = model.Country;
                page.Book.Publisher.City = model.City;
                page.Book.Publisher.Year = model.FoundationDate;

                try
                {
                    await adminRepository.UpdatePage(page);
                    return LocalRedirect(model.ReturnUrl);
                }
                catch(Exception exc)
                {
                    ModelState.AddModelError("", "Cant Save Changes");
                    ModelState.AddModelError("", exc.Message.Substring(0,100));
                }

            }
            
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            string path = await SaveFileAsync(file);
            var pages = await excelReader.GetPagesFromFileAsync(path);

            var changes = await adminRepository.UpdatePagesAsync(pages);

            if (changes == 0) return StatusCode(403);
            return RedirectToAction(nameof(Books));            
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBook(int id, string returnUrl)
        {
            await adminRepository.RemoveBook(id);
            if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> UserOrderHistory(string user_id)
        {
            var orders = await orderRepository.GetOrdersByUserId(user_id);
            return View(new UserOrdersViewModel(orders, userRepository));
        }

        [HttpGet]
        public async Task<IActionResult> Orders(int page = 1, string context="")
        {
            int orders_per_page = 15;
            int total_orders = await orderRepository.GetNumOfOrders();
            int order_to_skip = (page - 1) * orders_per_page;

            var pagination = new PageViewModel(page, total_orders, orders_per_page, context);
            var orders = await orderRepository.GetOrderList(order_to_skip, orders_per_page);
            var pages = await userRepository.GetPagesByIdAsync(orders.SelectMany(o => o.Items.Select(i => i.PageId)));

            return View(new OrderListViewModel(orders, pages, pagination));
        }

        [HttpGet]
        public async Task<IActionResult> EditUserOrder(int id) 
        {
            var order = await orderRepository.GetOrderByIdAsync(id);
            var p_list = new SelectList(Enum.GetValues<PaymentStatus>().Select(v => v.ToString()));
            var o_list = new SelectList(Enum.GetValues<OrderStatus>().Select(v => v.ToString()));
            return View(new EditUserOrderByAdminViewModel() 
            { 
                Order = order, 
                OrderStatusList = o_list,
                PaymentStatusList = p_list,
            }) ;
        }

        [HttpPost]
        public async Task<IActionResult> EditUserOrder(int id, string payment_status,string order_status)
        {
            var order = await orderRepository.GetOrderByIdAsync(id);
            try
            { 
                order.PaymentStatus = (PaymentStatus)Enum.Parse(typeof(PaymentStatus), payment_status);
                order.OrderStatus = (OrderStatus)Enum.Parse(typeof(OrderStatus), order_status);

                await orderRepository.UpdateOrderAsync(order);                
            }
            catch
            {
                return StatusCode(500, "Не удалось установить обновить заказ");
            }
            return RedirectToAction("UserOrderHistory", new { user_id = order.OwnerId });
        }

        public async Task<bool> IsRoleNameExist(string name)
        {
            return !await roleManager.RoleExistsAsync(name);
        }

        private async Task<IEnumerable<string>> GetUserRoles(string user_id)
        {
            var identity_user = await userManager.FindByIdAsync(user_id);
            var role_names = await userManager.GetRolesAsync(identity_user);

            return role_names;
            //var all_identity_roles = await roleManager.Roles.ToArrayAsync();
           //return all_identity_roles.Where(r => role_names.Contains(r.Name));
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            //TODO: try catch
            string dir_path = $"E:\\TempData\\{Guid.NewGuid().ToString()}";
            string file_path = $"{dir_path}\\{file.FileName}";
            Directory.CreateDirectory(dir_path);
            using (var fs = System.IO.File.Create(file_path))
            {
                await file.CopyToAsync(fs);
            }

            return file_path;
        }
    }
}
