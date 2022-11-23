using BookMarket.DataLayer.Context;
using BookMarket.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.DataLayer.Repository
{
    public class BookRepository : IRepository<Book>
    {
        private readonly BookContext ctx;

        public BookRepository()
        {
            this.ctx = new BookContext();
        }

        public async Task AddAsync(Book item)
        {
            var exist = await ctx.Books.AnyAsync(b => b.ISBN == item.ISBN);
            if (!exist)
            {
                await ctx.Books.AddAsync(item);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task AddRangeAsync(IEnumerable<Book> items)
        {
            var current = await ctx.Books.Select(b => b.ISBN).ToArrayAsync();

            var res = items.DistinctBy(i => i.ISBN).Where(i => !current.Contains(i.ISBN) && !String.IsNullOrEmpty(i.ISBN));
            var publishers = items.Select(b => b.Publisher).DistinctBy(p => p.Name);
            var works = res.SelectMany(r => r.LiteraryWorks);

            ctx.Works.AttachRange(works);
            ctx.Authors.AttachRange(works.Select(w => w.Author));
            ctx.Publishers.AttachRange(publishers);

            await ctx.Books.AddRangeAsync(res);
            await ctx.SaveChangesAsync();
        }

        public Task<IEnumerable<Book>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Book>> GetAsync(int count)
        {
            throw new NotImplementedException();
        }

        public Task<Book?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Book?> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Book?> RemoveAsync(Book item)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Book item)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRangeAsync(IEnumerable<Book> items)
        {
            throw new NotImplementedException();
        }
    }
}
