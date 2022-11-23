using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BookMarket.DataLayer.Context;
using BookMarket.DataLayer.Models;

namespace BookMarket.DataLayer.Repository
{
    public class ReviewRepository : IRepository<Review>
    {
        private readonly BookContext ctx;

        public ReviewRepository()
        {
            ctx = new BookContext();
        }

        public async Task AddAsync(Review item)
        {
            await ctx.Reviews.AddAsync(item);
            await ctx.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Review> items)
        {
            await ctx.Reviews.AddRangeAsync(items);
            await ctx.SaveChangesAsync();
        }

        public Task<IEnumerable<Review>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Review>> GetAsync(int count)
        {
            throw new NotImplementedException();
        }

        public Task<Review?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Review?> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Review?> RemoveAsync(Review item)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Review item)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRangeAsync(IEnumerable<Review> items)
        {
            throw new NotImplementedException();
        }
    }
}
