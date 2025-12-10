namespace LessonLink.BusinessLogic.Models;

public class AvailableSlot
{
    public int Id { get; set; }

    public required string TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }

    // Navigation property
    public List<Booking> Bookings { get; set; } = [];
}
