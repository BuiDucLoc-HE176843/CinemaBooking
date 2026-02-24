namespace CinemaBooking.Configuration
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResponse(bool success, string message, T data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        // Helper để gọi nhanh khi thành công
        public static ApiResponse<T> Ok(T data, string message = "Success")
            => new ApiResponse<T>(true, message, data);

        public static ApiResponse<object> Ok(string message = "Success")
            => new ApiResponse<object>(true, message, null);

        // Helper để gọi nhanh khi có lỗi nghiệp vụ
        public static ApiResponse<object> Fail(string message)
            => new ApiResponse<object>(false, message, null);
    }

    // 2. Exception tùy chỉnh để bắt lỗi nghiệp vụ (Business Logic Error)
    public class AppException : Exception
    {
        public AppException(string message) : base(message) { }
    }

    // 3. Middleware xử lý lỗi tập trung
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex) // Chỉ bắt lỗi do mình chủ động throw
            {
                context.Response.ContentType = "application/json";
                // Lưu ý: Status code vẫn giữ là 200 hoặc 400 tùy bạn, 
                // nhưng ở đây tôi tập trung vào việc trả về Message chuẩn.
                var response = ApiResponse<object>.Fail(ex.Message);
                await context.Response.WriteAsJsonAsync(response);
            }
            // Các lỗi hệ thống khác sẽ để mặc định của ASP.NET xử lý (hoặc log riêng)
        }
    }
}
