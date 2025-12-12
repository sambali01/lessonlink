namespace LessonLink.BusinessLogic.Models;

/// <summary>
/// Represents a time slot that a teacher has made available for lesson bookings.
/// Students can browse available slots and create bookings for specific slots.
/// Once booked, the slot remains in the system but should not appear to other students.
/// Teachers can edit or delete slots that haven't been booked yet.
/// The system prevents creating overlapping slots for the same teacher and checks for conflicts with the teacher's own bookings.
/// </summary>
public class AvailableSlot
{
    /// <summary>
    /// Unique identifier for the available time slot.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to the Teacher who created this available slot.
    /// </summary>
    public required string TeacherId { get; set; }

    /// <summary>
    /// Navigation property to the Teacher who owns this time slot.
    /// </summary>
    public Teacher Teacher { get; set; } = null!;

    /// <summary>
    /// The date and time when the lesson slot begins.
    /// Stored in UTC format.
    /// </summary>
    public required DateTime StartTime { get; set; }

    /// <summary>
    /// The date and time when the lesson slot ends.
    /// Stored in UTC format.
    /// Must be after StartTime.
    /// </summary>
    public required DateTime EndTime { get; set; }

    /// <summary>
    /// Collection of bookings made for this time slot.
    /// Typically contains 0 bookings (unbooked) or 1 booking (since a slot can only be booked once).
    /// Multiple cancelled bookings may exist if students repeatedly cancelled.
    /// </summary>
    public List<Booking> Bookings { get; set; } = [];
}
