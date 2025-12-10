namespace LessonLink.BusinessLogic.Models;

public class TeacherSubject
{
    public required string TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    public required int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
}
