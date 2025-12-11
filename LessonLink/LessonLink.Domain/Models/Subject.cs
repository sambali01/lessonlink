namespace LessonLink.BusinessLogic.Models;

/// <summary>
/// Represents an academic subject or topic that can be taught in the lesson booking system.
/// Examples: Mathematics, English, Physics, Programming, etc.
/// Subjects have a many-to-many relationship with Teachers.
/// </summary>
public class Subject
{
    /// <summary>
    /// Unique identifier for the subject.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the subject (e.g., "Mathematics", "English Literature").
    /// Displayed in teacher profiles and used in search filters.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Many-to-many relationship with Teachers through the TeacherSubject join table.
    /// Represents all teachers who are qualified to teach this subject.
    /// </summary>
    public List<TeacherSubject> TeacherSubjects { get; set; } = [];
}
