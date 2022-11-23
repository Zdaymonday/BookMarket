using BookMarket.DataLayer.Context;
using BookMarket.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.DataLayer.Repository.AdminRepository
{
    public class AdminRepository
    {
        private readonly BookContext ctx;

        public AdminRepository()
        {
            ctx = new BookContext();
        }

        public async Task<IEnumerable<PageInStore>> GetBooksByAuthorName(string name, int skip, int take)
        {
            return await GetBooksByAuthorName(name).Skip(skip).Take(take).ToArrayAsync();
        }

        public async Task<int> GetNumOfBooksByAuthorName(string name)
        {
            return await GetBooksByAuthorName(name).CountAsync();
        }

        public async Task<IEnumerable<PageInStore>> GetBooksByTitle(string name, int skip, int take)
        {
            return await GetBookByTitle(name).Skip(skip).Take(take).ToArrayAsync();
        }

        public async Task<int> GetNumOfBooksByTitle(string name)
        {
            return await GetBookByTitle(name).CountAsync();
        }

        public async Task<int> RemoveBook(int id)
        {
            ctx.PageInStore.Attach(new PageInStore() { Id = id }).State = EntityState.Deleted;
            return await ctx.SaveChangesAsync();            
        }

        public async Task<PageInStore?> GetBookById(int id)
        {
            return await ctx.PageInStore.Include(p => p.Book.LiteraryWorks).ThenInclude(w => w.Author).Include(p => p.Book.Publisher)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> UpdatePage(PageInStore page)
        {
            ctx.PageInStore.Update(page);
            return await ctx.SaveChangesAsync();
        }

        public async Task<int> UpdatePagesAsync(IEnumerable<PageInStore> pages)
        {
            ctx.PageInStore.AttachRange(pages);
            return await ctx.SaveChangesAsync();
        }

        private IQueryable<PageInStore> GetBooksByAuthorName(string name)
        {
            return ctx.PageInStore.Include(p => p.Book.LiteraryWorks).ThenInclude(w => w.Author)
                .Where(p => p.Book.LiteraryWorks.Select(w => w.Author.Name).Any(a => a.Contains(name)));
        }

        private IQueryable<PageInStore> GetBookByTitle(string name)
        {
            return ctx.PageInStore.Include(p => p.Book.LiteraryWorks).ThenInclude(w => w.Author)
                .Where(p => p.Book.LiteraryWorks.Select(w => w.Title).Any(a => a.Contains(name)));
        }
    }
}
