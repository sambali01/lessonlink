using LessonLink.BusinessLogic.DTOs.Booking;

namespace LessonLink.BusinessLogic.DTOs.AvailableSlot;

public class AvailableSlotDetailsDto
{
    public int Id { get; set; }
    public string TeacherId { get; set; } = null!;
    public string TeacherName { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingGetDto? Booking { get; set; }
}