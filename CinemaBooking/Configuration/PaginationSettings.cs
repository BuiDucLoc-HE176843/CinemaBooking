namespace CinemaBooking.Configuration
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class PaginationRequest
    {
        private const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
