namespace LessonLink.BusinessLogic.Models;

/// <summary>
/// Represents the teacher-specific profile information in the lesson booking system.
/// This entity extends User functionality with teaching-related data.
/// A Teacher is linked one-to-one with a User who has the Teacher role.
/// Teachers can both offer lessons (via AvailableSlots) and book lessons from other teachers (as Users).
/// </summary>
public class Teacher
{
    /// <summary>
    /// The unique identifier that serves as both primary key and foreign key to the User entity.
    /// Establishes a one-to-one relationship with the User table.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Navigation property to the associated User entity.
    /// Contains basic user information like name, email, and profile image.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Indicates whether the teacher offers online lessons.
    /// Used for filtering teachers in the search functionality.
    /// </summary>
    public bool AcceptsOnline { get; set; }

    /// <summary>
    /// Indicates whether the teacher offers in-person lessons.
    /// Used for filtering teachers in the search functionality.
    /// </summary>
    public bool AcceptsInPerson { get; set; }

    /// <summary>
    /// The city or location where in-person lessons are held.
    /// Required when AcceptsInPerson is true, null otherwise.
    /// Used for location-based teacher search filtering.
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// The teacher's hourly rate in the system's currency.
    /// Used for filtering teachers by price range in the search functionality.
    /// </summary>
    public int HourlyRate { get; set; }

    /// <summary>
    /// A self-written description of the teacher.
    /// Displayed on the teacher's public profile page.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Contact information for communication between student and teacher after booking confirmation.
    /// Could be email, phone number, or other preferred contact method.
    /// </summary>
    public required string Contact { get; set; }

    /// <summary>
    /// Many-to-many relationship with Subjects through the TeacherSubject join table.
    /// Represents all subjects this teacher is qualified to teach.
    /// </summary>
    public List<TeacherSubject> TeacherSubjects { get; set; } = [];

    /// <summary>
    /// Collection of time slots this teacher has made available for booking.
    /// Students can browse and book these slots to schedule lessons.
    /// </summary>
    public List<AvailableSlot> AvailableSlots { get; set; } = [];
}
