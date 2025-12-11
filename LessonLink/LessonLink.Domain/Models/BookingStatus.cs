namespace LessonLink.BusinessLogic.Models;

/// <summary>
/// Represents the lifecycle status of a lesson booking in the system.
/// A booking progresses from Pending to either Confirmed (when approved by the teacher) or Cancelled (when rejected or cancelled).
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// The booking has been created by a student but is awaiting teacher approval.
    /// This is the initial state when a student books an available time slot.
    /// </summary>
    Pending,

    /// <summary>
    /// The booking has been approved by the teacher.
    /// The lesson is scheduled and confirmed for both parties.
    /// </summary>
    Confirmed,

    /// <summary>
    /// The booking has been cancelled.
    /// This can occur when: the teacher rejects the booking, the student cancels before the deadline,
    /// or the associated available slot is deleted.
    /// </summary>
    Cancelled
}
