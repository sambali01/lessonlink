namespace LessonLink.BusinessLogic.Models;

/// <summary>
/// Defines the available user roles in the lesson booking system.
/// Users can have multiple roles simultaneously (e.g., a teacher can also be a student).
/// </summary>
public enum Role
{
    /// <summary>
    /// Student role - can browse teachers and book available lesson slots.
    /// This is the default role assigned upon registration.
    /// </summary>
    Student = 0,

    /// <summary>
    /// Teacher role - can create available time slots, manage incoming bookings, and also book lessons from other teachers.
    /// Teachers have all student capabilities plus additional teaching-related features.
    /// </summary>
    Teacher = 1,

    /// <summary>
    /// Administrator role - has elevated privileges for system management.
    /// </summary>
    Admin = 2
}
