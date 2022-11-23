using BookMarket.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMarket.DataLayer.Repository.UserRepository
{
    public interface IUserRepository
    {
        public Task<int> GetCountOfBooksInJenres(IEnumerable<string> jenreNames);
        public Task<IEnumerable<PageInStore>> GetBooksByJenre(IEnumerable<string> jenreNames, int skip, int take);

        public Task<int> GetCountOfMatches(string pattern);
        public Task<IEnumerable<PageInStore>> Search(string pattern, int skip, int take);

        public Task<IEnumerable<Jenre>> GetAllJenresAsync();
        
        public Task<IEnumerable<Book>> GetPopularBooks(int count);
        public Task<IEnumerable<PageInStore>> GetTopInJenre(string jenre, int count);
        
        public Task<IEnumerable<Review>> GetReviews(int workId, int count);

        public Task<PageInStore?> GetPageByIdAsync(int id);
        public Task<IEnumerable<PageInStore>> GetPagesByIdAsync(IEnumerable<int> idSet);
    }
}
