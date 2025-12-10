namespace LessonLink.BusinessLogic.Models;

public class Booking
{
    public int Id { get; set; }

    // The booker
    public required string StudentId { get; set; }
    public User Student { get; set; } = null!;

    // A teacher's available slot that the booker books
    public required int AvailableSlotId { get; set; }
    public AvailableSlot AvailableSlot { get; set; } = null!;

    public required BookingStatus Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
