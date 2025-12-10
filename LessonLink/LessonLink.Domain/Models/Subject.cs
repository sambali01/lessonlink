namespace LessonLink.BusinessLogic.Models;

public class Subject
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public List<TeacherSubject> TeacherSubjects { get; set; } = [];
}
