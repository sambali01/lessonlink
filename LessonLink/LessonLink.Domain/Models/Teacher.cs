namespace LessonLink.BusinessLogic.Models;

public class Teacher
{
    public required string UserId { get; set; }  // Primary key and foreign key
    public User User { get; set; } = null!;

    public bool AcceptsOnline { get; set; }
    public bool AcceptsInPerson { get; set; }

    public string? Location { get; set; }

    public int HourlyRate { get; set; }

    public string Description { get; set; } = string.Empty;

    public required string Contact { get; set; }

    public List<TeacherSubject> TeacherSubjects { get; set; } = [];
    public List<AvailableSlot> AvailableSlots { get; set; } = [];
}
