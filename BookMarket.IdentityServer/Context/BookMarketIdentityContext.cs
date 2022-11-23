using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace BookMarket.IdentityServer.Context
{
    public class BookMarketIdentityContext : IdentityDbContext
    {
        public BookMarketIdentityContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(GetConnectionStringFromJson("identity"));
        }

        private string GetConnectionStringFromJson(string connectionName)
        {
            var confBuilder = new ConfigurationBuilder();
            confBuilder.SetBasePath("C:\\Users\\Администратор\\source\\repos\\BookMarket\\BookMarket.IdentityServer");
            confBuilder.AddJsonFile("connections.json");
            var config = confBuilder.Build();
            return config.GetConnectionString(connectionName);
        }
    }
}
