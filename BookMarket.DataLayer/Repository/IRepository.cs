using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookMarket.DataLayer.Models;

namespace BookMarket.DataLayer.Repository
{
    public interface IRepository<T>
    {
        public Task<IEnumerable<T>> GetAll();
        public Task<IEnumerable<T>> GetAsync(int count);
        public Task<T?> GetById(int id);
        public Task<T?> GetByName(string name);

        public Task AddAsync(T item);
        public Task AddRangeAsync(IEnumerable<T> items); 
        public Task UpdateAsync(T item);
        public Task<T?> RemoveAsync(T item);
        public Task UpdateRangeAsync(IEnumerable<T> items);
    }
}
