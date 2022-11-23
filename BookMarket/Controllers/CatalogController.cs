using BookMarket.DataLayer.Models;
using BookMarket.DataLayer.Repository.UserRepository;
using BookMarket.ViewModels;
using BookMarket.ViewModels.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace BookMarket.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IUserRepository repository;
        public CatalogController(IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IActionResult> Index(int page = 1, int itemsPerPage = 12, string context = "Фэнтези")
        {
            var jenresInBase = await repository.GetAllJenresAsync();
            if (jenresInBase.FirstOrDefault(j => j.JenreName.Equals(context, StringComparison.CurrentCultureIgnoreCase)) is null) return RedirectToAction(nameof(Search));

            var jenres = new List<string>() { context };
            switch (context)
            {
                case "Хоррор/Ужасы": jenres.AddRange(new[] { "Мистика", "Магический реализм" }); break;
            }

            int totalPages = await repository.GetCountOfBooksInJenres(jenres);
            var model = new PageViewModel(page, totalPages, itemsPerPage, context);
            int forSkip = (page - 1) * itemsPerPage;

            var booksForView = await repository.GetBooksByJenre(jenres, forSkip, itemsPerPage);

            return View(new CatalogViewModel(model, booksForView));
        }

        public async Task<IActionResult> Book(int id)
        {
            var page = await repository.GetPageByIdAsync(id);
            var work_id = page.Book.LiteraryWorks.First().Id;
            var reviews = await repository.GetReviews(work_id, 10);
            return View(new BookViewModel(page, reviews));
        }

        public async Task<IActionResult> Search(int page = 1, int itemsPerPage = 12, string context = "")
        {
            var count = await repository.GetCountOfMatches(context);
            if(count == 0 ) return View("Not Found", context);

            var model = new PageViewModel(page, count, itemsPerPage, context);
            int forSkip = (page - 1) * itemsPerPage;

            var booksForView = await repository.Search(context, forSkip, itemsPerPage);

            return View("Index", new CatalogViewModel(model, booksForView));
        }
    }
}
