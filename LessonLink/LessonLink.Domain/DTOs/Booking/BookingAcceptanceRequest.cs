using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.DTOs.Booking;

public class BookingAcceptanceRequest
{
    public BookingStatus Status { get; set; }
}
