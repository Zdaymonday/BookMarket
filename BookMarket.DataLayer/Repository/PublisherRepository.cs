using BookMarket.DataLayer.Context;
using BookMarket.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.DataLayer.Repository
{
    public class PublisherRepository : IRepository<Publisher>
    {
        private readonly BookContext ctx;

        public PublisherRepository()
        {
            ctx = new BookContext();
        }

        public async Task AddAsync(Publisher item)
        {
            var exist = await ctx.Publishers.AnyAsync(p => p.Name == item.Name);
            if (!exist)
            {
                await ctx.Publishers.AddAsync(item);
                await ctx.SaveChangesAsync();
            }
        }

        public void Add(Publisher item)
        {
            var exist = ctx.Publishers.Any(p => p.Name == item.Name);
            if (!exist)
            {
                ctx.Publishers.Add(item);
                ctx.SaveChanges();
            }
        }

        public async Task AddRangeAsync(IEnumerable<Publisher> items)
        {
            var current = await ctx.Publishers.Select(p => p.Name).ToArrayAsync();
            var res = items.ExceptBy(current, i => i.Name);
            await ctx.Publishers.AddRangeAsync(res);
            await ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<Publisher>> GetAll()
        {
            return await ctx.Publishers.ToArrayAsync();
        }

        public Task<IEnumerable<Publisher>> GetAsync(int count)
        {
            throw new NotImplementedException();
        }

        public Task<Publisher?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Publisher?> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Publisher?> RemoveAsync(Publisher item)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Publisher item)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRangeAsync(IEnumerable<Publisher> items)
        {
            throw new NotImplementedException();
        }
    }
}
