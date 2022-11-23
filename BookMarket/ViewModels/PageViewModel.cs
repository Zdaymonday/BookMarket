namespace BookMarket.ViewModels
{
    public class PageViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string Context { get; set; }
        public bool HasNextPage => CurrentPage < TotalPages;
        public bool HasPreviousPage => CurrentPage > 1;

        public PageViewModel(int currentPage, int totalItems, int itemsPerPage, string context = "")
        {
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
            Context = context;
        }
    }
}
