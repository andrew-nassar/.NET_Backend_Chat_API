namespace SignalR.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }  // Total items in database matching the filter
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // Helper to let frontend know if they should show a "Load More" button
        public bool HasNextPage => (PageNumber * PageSize) < TotalCount;
    }
}
