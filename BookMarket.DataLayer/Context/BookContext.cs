using BookMarket.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.DataLayer.Context
{
    public class BookContext : DbContext
    {
        public BookContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var connString = Config.GetConnectionStringFromJson("book_data");
            builder.UseSqlServer(connString);
        }

        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Jenre> Jenres { get; set; } = null!;
        public DbSet<LiteraryWork> Works { get; set; } = null!;
        public DbSet<Publisher> Publishers { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<PageInStore> PageInStore { get; set; } = null!;

    }
}
