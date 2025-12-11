namespace LessonLink.BusinessLogic.Models;

/// <summary>
/// Represents a lesson booking created when a student reserves a teacher's available time slot.
/// Bookings have a lifecycle: they start as Pending, then can be Confirmed by the teacher or Cancelled (by rejection, student cancellation, or slot deletion).
/// Students can cancel bookings up to a certain time before the lesson starts.
/// Teachers can approve or reject pending bookings.
/// </summary>
public class Booking
{
    /// <summary>
    /// Unique identifier for the booking.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to the User who made this booking (the student).
    /// Note: Even teachers can book lessons from other teachers, acting as students.
    /// </summary>
    public required string StudentId { get; set; }

    /// <summary>
    /// Navigation property to the User (student) who created this booking.
    /// </summary>
    public User Student { get; set; } = null!;

    /// <summary>
    /// Foreign key to the AvailableSlot being booked.
    /// Links this booking to a specific time slot created by a teacher.
    /// </summary>
    public required int AvailableSlotId { get; set; }

    /// <summary>
    /// Navigation property to the AvailableSlot that was booked.
    /// Contains the time slot details and teacher information.
    /// </summary>
    public AvailableSlot AvailableSlot { get; set; } = null!;

    /// <summary>
    /// The current status of the booking: Pending, Confirmed, or Cancelled.
    /// Initially set to Pending when created.
    /// </summary>
    public required BookingStatus Status { get; set; }

    /// <summary>
    /// The UTC timestamp when the booking was created.
    /// Used for sorting bookings and tracking booking history.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
