namespace LessonLink.BusinessLogic.DTOs.Teacher;

public class TeacherGetDto
{
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string SurName { get; set; }
    public string? NickName { get; set; }
    public string? ImageUrl { get; set; }
    public bool? AcceptsOnline { get; set; }
    public bool? AcceptsInPerson { get; set; }
    public string? Location { get; set; }
    public int? HourlyRate { get; set; }
    public string? Description { get; set; }
    public double? Rating { get; set; }
    public List<string> Subjects { get; set; } = new List<string>();
}
