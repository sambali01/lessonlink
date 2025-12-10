using LessonLink.BusinessLogic.DTOs.Booking;

namespace LessonLink.BusinessLogic.DTOs.AvailableSlot;

public class AvailableSlotResponse
{
    public int Id { get; set; }
    public string TeacherId { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<BookingResponse> Bookings { get; set; } = [];
}
