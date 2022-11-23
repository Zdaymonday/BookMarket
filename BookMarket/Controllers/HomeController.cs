using BookMarket.DataLayer.Models;
using BookMarket.DataLayer.Repository.UserRepository;
using BookMarket.ViewModels.Index;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository repository;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public HomeController(IUserRepository repository, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            this.repository = repository;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var top_fantasy = await repository.GetTopInJenre("Фэнтези", 10);
                var top_fantastic = await repository.GetTopInJenre("Фантастика", 10);
                var top_mystic = await repository.GetTopInJenre("Мистика", 10);
                return View(new IndexViewModel(new Dictionary<string, IEnumerable<PageInStore>>()
                {
                    ["Фэнтези"] = top_fantasy,
                    ["Фантастика"] = top_fantastic,
                    ["Мистика"] = top_mystic,
                }));
            }
            catch
            {
                return NotFound();
            }
        }

        
    }
}
