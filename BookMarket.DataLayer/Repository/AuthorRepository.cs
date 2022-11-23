using BookMarket.DataLayer.Context;
using BookMarket.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.DataLayer.Repository
{
    public class AuthorRepository : IRepository<Author>
    {
        private readonly BookContext ctx;

        public AuthorRepository()
        {
            ctx = new();
        }

        public async Task AddAsync(Author item)
        {
            var exist = ctx.Authors.Any(a => a.Name.Equals(item.Name));
            if (exist)
            {
                await ctx.Authors.AddAsync(item);
            }
            await ctx.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Author> items)
        {
            var current = ctx.Authors.Select(a => a.Name);
            var result = items.DistinctBy(i => i.Name).ExceptBy(current, i => i.Name);
            await ctx.AddRangeAsync(result);
            await ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<Author>> GetAll()
        {
            return await ctx.Authors.ToArrayAsync();
        }

        public Task<IEnumerable<Author>> GetAsync(int count)
        {
            throw new NotImplementedException();
        }

        public async Task<Author?> GetById(int id)
        {
            return await ctx.FindAsync<Author>(id);
        }

        public async Task<Author?> GetByName(string name)
        {
            return await ctx.Authors.FirstOrDefaultAsync(a => a.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task<Author?> RemoveAsync(Author item)
        {
            var res = await ctx.FindAsync<Author>(item.Id);
            if (res is not null) ctx.Authors.Remove(res);
            await ctx.SaveChangesAsync();
            return res;
        }

        public async Task UpdateAsync(Author item)
        {
            var res = await ctx.Authors.FindAsync(item.Id);
            if (res is not null) ctx.Authors.Update(item);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<Author> items)
        {
            ctx.Authors.UpdateRange(items);
            await ctx.SaveChangesAsync();
        }
    }
}
