using BookMarket.DataLayer.Models;

namespace BookMarket.ExcelHandler.Interfaces
{
    public interface IExcelFileReader
    {
        Task<IEnumerable<PageInStore>> GetPagesFromFileAsync(string filePath);
    }
}
