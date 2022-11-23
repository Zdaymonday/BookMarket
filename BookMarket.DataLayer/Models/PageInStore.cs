namespace BookMarket.DataLayer.Models
{
    public class PageInStore
    {
        public int Id { get; set; }

        public string ShopName { get; set; } = null!;
        public string BookPage { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public decimal Price { get; set; }
        public bool? InStock { get; set; }

        public string GetBookName()
        {
            return Book.LiteraryWorks.First().Title;
        }

        public string GetBookAuthorName()
        {
            return Book.LiteraryWorks.First().Author.Name;
        }
    }
}
