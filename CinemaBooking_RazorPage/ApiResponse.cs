namespace CinemaBooking_RazorPage
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class PagedData<T>
    {
        public List<T> Items { get; set; }
    }
}
