using BookMarket.DataLayer.Context;
using BookMarket.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.DataLayer.Repository.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly BookContext ctx;

        public UserRepository()
        {
            ctx = new BookContext();
        }

        public async Task<IEnumerable<PageInStore>> GetBooksByJenre(IEnumerable<string> jenreNames, int skip, int take)
        {
            return await ctx.PageInStore.Include(p => p.Book.LiteraryWorks).ThenInclude(w => w.Author)
                .Where(p => jenreNames.Contains(p.Book.LiteraryWorks.First().Jenre.JenreName))
                .Skip(skip)
                .Take(take)
                .ToArrayAsync();
        }

        public async Task<int> GetCountOfBooksInJenres(IEnumerable<string> jenreNames)
        {
            return await ctx.PageInStore.Where(w => jenreNames.Contains(w.Book.LiteraryWorks.First().Jenre.JenreName)).CountAsync();           
        }

        public Task<IEnumerable<Book>> GetPopularBooks(int count)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PageInStore>> GetTopInJenre(string jenre, int count)
        {
            var Jenre = await ctx.Jenres.FirstOrDefaultAsync(j => j.JenreName == jenre);
            var pre_pages = await ctx.PageInStore.Include(p => p.Book.LiteraryWorks).ThenInclude(w => w.Author)
                .Where(p => p.Book.LiteraryWorks.First().JenreId == Jenre!.Id)
                .Where(p => p.Book.LiteraryWorks.First().isForKids != true)
                .OrderByDescending(p => p.Book.LiteraryWorks.First().Raiting)
                .Take(count * 10)
                .ToArrayAsync();

            var pages = pre_pages.GroupBy(p => p.Book.LiteraryWorks.First()).Select(g => g.First()).Take(10);

            return pages;
        }

        public async Task<PageInStore?> GetPageByIdAsync(int id)
        {
            return await ctx.PageInStore
                .Include(b => b.Book.LiteraryWorks)
                .ThenInclude(w => w.Author)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PageInStore>> GetPagesByIdAsync(IEnumerable<int> idSet)
        {
            return await ctx.PageInStore
                .Include(b => b.Book.LiteraryWorks)
                .ThenInclude(w => w.Author)
                .Where(p => idSet.Contains(p.Id))
                .ToArrayAsync();
        }

        public async Task<IEnumerable<Review>> GetReviews(int workId, int count)
        {
            return await ctx.Reviews.Where(r => r.WorkID == workId).Take(count).ToArrayAsync();
        }

        public async Task<IEnumerable<PageInStore>> Search(string pattern, int skip, int take)
        {
            //поиск подстроки в названии произведения или имени автора
            //TODO Защита от внедрения sql
            var booksId = await ctx.PageInStore.FromSqlRaw("SELECT Id FROM PageInStore WHERE BookId in " +
                "(SELECT BooksID FROM BookLiteraryWork WHERE LiteraryWorksId in " +
                $"(SELECT Id FROM Works WHERE Title Like '%{pattern}%'))").Select(p => p.Id).ToArrayAsync();

            var authorsId = await ctx.PageInStore.FromSqlRaw("SELECT Id FROM PageInStore WHERE BookId IN " +
                "(SELECT BooksId FROM BookLiteraryWork WHERE LiteraryWorksId IN " +
                "(SELECT Id FROM Works WHERE AuthorId IN " +
                $"(SELECT Id From Authors WHERE Name Like '%{pattern}%')))").Select(p => p.Id).ToArrayAsync();

            var resultIdSet = booksId.Union(authorsId).Skip(skip).Take(take);

            return await ctx.PageInStore
                .Where(p => resultIdSet.Contains(p.Id))
                .Include(p => p.Book.LiteraryWorks).ThenInclude(w => w.Author)
                .ToArrayAsync();
        }

        public async Task<int> GetCountOfMatches(string pattern)
        {
            //поиск подстроки в названии произведения или имени автора
            return await ctx.PageInStore.Include(p => p.Book.LiteraryWorks).ThenInclude(w => w.Author)
                            .Where(p => p.Book.LiteraryWorks.First().Title.Contains(pattern)
                            || p.Book.LiteraryWorks.First().Author.Name.Contains(pattern))
                            .CountAsync();
        }

        public async Task<IEnumerable<Jenre>> GetAllJenresAsync()
        {
            return await ctx.Jenres.ToArrayAsync();
        }
    }
}
