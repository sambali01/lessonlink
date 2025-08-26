namespace LessonLink.BusinessLogic.Models;

public class Booking
{
    public int Id { get; set; }

    // The booker
    public string StudentId { get; set; }
    public User Student { get; set; }

    // A teacher's available slot that the booker books
    public int AvailableSlotId { get; set; }
    public AvailableSlot AvailableSlot { get; set; }

    public BookingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
