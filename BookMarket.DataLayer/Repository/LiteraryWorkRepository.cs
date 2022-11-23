using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookMarket.DataLayer.Models;
using BookMarket.DataLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.DataLayer.Repository
{
    public class LiteraryWorkRepository : IRepository<LiteraryWork>
    {
        private readonly BookContext ctx;

        public LiteraryWorkRepository()
        {
            ctx = new BookContext();
        }

        public async Task AddAsync(LiteraryWork item)
        {
            var authorExist = await ctx.Authors.AnyAsync(a => a.Id == item.AuthorId);
            if(authorExist)
            {
                var authorWorks = await ctx.Works
                    .Include(w => w.Author)
                    .Where(w => w.Author.Id == item.AuthorId)
                    .Select(w => w.Title)
                    .ToArrayAsync();

                if(!authorWorks.Contains(item.Title))
                {
                    await ctx.Works.AddAsync(item);
                    await ctx.SaveChangesAsync();
                }                
            }            
        }

        public async Task AddRangeAsync(IEnumerable<LiteraryWork> items)
        {

            await ctx.Works.AddRangeAsync(items);
            await ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<LiteraryWork>> GetAll()
        {
            return await ctx.Works.Include(w => w.Author).ToArrayAsync();
        }

        public async Task<IEnumerable<LiteraryWork>> GetAsync(int count)
        {
            return await ctx.Works.Take(count).Include(w => w.Author).ToArrayAsync();
        }

        public async Task<LiteraryWork?> GetById(int id)
        {
            return await ctx.Works.FindAsync(id);
        }

        public async Task<LiteraryWork?> GetByName(string title)
        {
            return await ctx.Works.FirstOrDefaultAsync(i => i.Title == title);           
        }

        public async Task<LiteraryWork?> RemoveAsync(LiteraryWork item)
        {
            var res = await ctx.Works.FirstOrDefaultAsync(i => i.Id == item.Id);
            if(res is not null) ctx.Remove(res);
            await ctx.SaveChangesAsync();
            return res;
        }

        public async Task UpdateAsync(LiteraryWork item)
        {
            var res = await ctx.Works.FirstOrDefaultAsync(i => i.Id == item.Id);
            //или логика обновления полей
            if (res is not null) ctx.Update(item);
            await ctx.SaveChangesAsync();            
        }

        public async Task UpdateRangeAsync(IEnumerable<LiteraryWork> items)
        {
            ctx.Works.UpdateRange(items);
            await ctx.SaveChangesAsync();
        }
    }
}
