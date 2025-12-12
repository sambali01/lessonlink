namespace LessonLink.BusinessLogic.Models;

/// <summary>
/// Join table entity representing the many-to-many relationship between Teachers and Subjects.
/// Each record indicates that a specific teacher is qualified to teach a specific subject.
/// This allows teachers to teach multiple subjects and subjects to be taught by multiple teachers.
/// </summary>
public class TeacherSubject
{
    /// <summary>
    /// Foreign key to the Teacher entity.
    /// Part of the composite primary key.
    /// </summary>
    public required string TeacherId { get; set; }

    /// <summary>
    /// Navigation property to the associated Teacher entity.
    /// </summary>
    public Teacher Teacher { get; set; } = null!;

    /// <summary>
    /// Foreign key to the Subject entity.
    /// Part of the composite primary key.
    /// </summary>
    public required int SubjectId { get; set; }

    /// <summary>
    /// Navigation property to the associated Subject entity.
    /// </summary>
    public Subject Subject { get; set; } = null!;
}
