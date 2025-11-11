namespace LessonLink.BusinessLogic.Models;

public class AvailableSlot
{
    public int Id { get; set; }

    public required string TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    // Navigation property
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
