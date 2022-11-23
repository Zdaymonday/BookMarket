using BookMarket.DataLayer.Models;
using BookMarket.DataLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.DataLayer.Repository
{
    public class JenreRepository : IRepository<Jenre>
    {
        private readonly BookContext ctx;

        public JenreRepository()
        {
            ctx = new BookContext();
        }

        public async Task AddAsync(Jenre item)
        {
            var exist = await ctx.Jenres.AnyAsync(j => j.JenreName == item.JenreName);
            if(!exist)
            {
                await ctx.Jenres.AddAsync(item);
                await ctx.SaveChangesAsync();                  
            }
        }

        public Task AddRangeAsync(IEnumerable<Jenre> items)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Jenre>> GetAll()
        {
            return await ctx.Jenres.ToArrayAsync();
        }

        public async Task<IEnumerable<Jenre>> GetAsync(int count)
        {
            return await ctx.Jenres.Take(count).ToArrayAsync();
        }

        public Task<Jenre?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Jenre?> GetByName(string name)
        {
            return await ctx.Jenres.FirstOrDefaultAsync(j => j.JenreName == name);
        }

        public Task<Jenre?> RemoveAsync(Jenre item)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Jenre item)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRangeAsync(IEnumerable<Jenre> items)
        {
            throw new NotImplementedException();
        }

        public void Add(Jenre jenre)
        {
            ctx.Jenres.Add(jenre);
            ctx.SaveChanges();
        }
    }
}
