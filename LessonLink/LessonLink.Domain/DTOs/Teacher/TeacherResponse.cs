using LessonLink.BusinessLogic.DTOs.Subject;

namespace LessonLink.BusinessLogic.DTOs.Teacher;

public class TeacherResponse
{
    public required string UserId { get; set; }
    public required string FirstName { get; set; }
    public required string SurName { get; set; }
    public required string NickName { get; set; }
    public string? ImageUrl { get; set; }
    public bool AcceptsOnline { get; set; }
    public bool AcceptsInPerson { get; set; }
    public string? Location { get; set; }
    public int HourlyRate { get; set; }
    public string Description { get; set; } = string.Empty;
    public required string Contact { get; set; }
    public List<SubjectResponse> Subjects { get; set; } = [];
}
