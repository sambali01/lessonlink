namespace LessonLink.BusinessLogic.Models;

public class Teacher
{
    public string UserId { get; set; }  // Primary key and foreign key
    public User User { get; set; }

    public bool? AcceptsOnline { get; set; }
    public bool? AcceptsInPerson { get; set; }
    public string? Location { get; set; }
    public int? HourlyRate { get; set; }
    public string? Description { get; set; }
    public double? Rating { get; set; }

    public List<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
    public List<AvailableSlot> AvailableSlots { get; set; } = new List<AvailableSlot>();
}
