namespace CinemaBooking.Enums
{
    public enum UserRole
    {
        User,
        Admin
    }

    public enum MovieStatus
    {
        Upcoming,   // sắp chiếu
        Ongoing,    // đang chiếu
        Ended       // đã kết thúc
    }

    public enum SeatType
    {
        Regular,
        Vip
    }

    public enum SeatStatus
    {
        Available,   // còn trống
        Disabled,    // admin tắt ghế
        Booked,      // đã đặt và thanh toán xong
        Pending      // chờ thanh toán (đã giữ chỗ tạm thời)
    }

    public enum BookingStatus
    {
        Pending,    // chờ thanh toán
        Paid,       // đã thanh toán
        Cancelled   // đã hủy
    }
}
