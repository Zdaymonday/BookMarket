using BookMarket.DataLayer.Context;
using BookMarket.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.DataLayer.Repository.AdminRepository
{
    public class UploadRepository
    {
        private readonly BookContext ctx;

        public UploadRepository()
        {
            ctx = new();
        }

        public async Task<IDictionary<string,Author>> GetAuthorsAsync(IEnumerable<string> names)
        {
            var authors = await ctx.Authors.Where(a => names.Contains(a.Name) || names.Contains(a.ShortName)).ToDictionaryAsync(a => a.Name);
            if (authors.Count() == names.Count()) return authors;

            var exist_authors = authors.Values;

            foreach(var name in names)
            {
                if(!exist_authors.Any(a => a.Name == name || a.ShortName == name))
                {
                    var new_author = new Author() { Name = name };
                    authors.Add(name, new_author);
                }
            }

            if (authors.Count() != names.Count()) throw new Exception("Some shit with authors");

            return authors;
        }

        public async Task<IDictionary<string, LiteraryWork>> GetWorksAsync(IEnumerable<string> titles)
        {
            var works = await ctx.Works.Include(w => w.Author).Where(a => titles.Contains(a.Title)).ToDictionaryAsync(a => a.Title);
            if (works.Count() == titles.Count()) return works;

            var exist_works = works.Values;

            foreach (var title in titles)
            {
                if (!exist_works.Any(a => a.Title == title))
                {
                    var new_work = new LiteraryWork() { Title = title };
                    works.Add(title, new_work);
                }
            }

            if (works.Count() != titles.Count()) throw new Exception("Some shit with works");

            return works;
        }

        public async Task<IDictionary<string, Jenre>> GetJenresAsync(IEnumerable<string> names)
        {
            var jenres = await ctx.Jenres.Where(j => names.Contains(j.JenreName)).ToDictionaryAsync(j => j.JenreName);
            if (jenres.Count() == names.Count()) return jenres;

            var exist_jenres = jenres.Values;

            foreach (var name in names)
            {
                if (!exist_jenres.Any(a => a.JenreName == name))
                {
                    var new_jenre = new Jenre() { JenreName = name };
                    jenres.Add(name, new_jenre);
                }
            }

            if (jenres.Count() != names.Count()) throw new Exception("Some shit with jenres");

            return jenres;
        }
            

        public async Task<Book> GetBookAsync(string ISBN)
        {
            return await ctx.Books.FirstAsync(b => b.ISBN == ISBN);
        }

        public async Task<bool> IsBookExist(string ISBN)
        {
            return await ctx.Books.AnyAsync(b => b.ISBN == ISBN);
        }

        public async Task<Publisher> GetPublisherAsync(string name)
        {
            var publisher = await ctx.Publishers.FirstOrDefaultAsync(p => p.Name == name);
            if (publisher is null)
            {
                publisher = new Publisher() { Name = name, FullName = name };
            }
            return publisher;
        }
    }
}
