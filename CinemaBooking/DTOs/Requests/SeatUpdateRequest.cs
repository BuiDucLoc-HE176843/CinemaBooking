using CinemaBooking.Enums;
using System.ComponentModel.DataAnnotations;

namespace CinemaBooking.DTOs.Requests
{
    public class SeatUpdateRequest
    {
        public SeatType Type { get; set; }

        public bool IsDisabled { get; set; }
    }
}
