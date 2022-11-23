using Microsoft.Extensions.Configuration;

namespace BookMarket.DataLayer.Context
{
    internal class Config
    {
        internal static string GetConnectionStringFromJson(string connectionName)
        {
            var confBuilder = new ConfigurationBuilder();
            confBuilder.SetBasePath("C:\\Users\\Администратор\\source\\repos\\BookMarket\\BookMarket.DataLayer");
            confBuilder.AddJsonFile("connections.json");
            var config = confBuilder.Build();
            return config.GetConnectionString(connectionName);
        }
    }
}
