using Microsoft.AspNetCore.Http;

namespace LessonLink.BusinessLogic.DTOs.User;

public class TeacherUpdateDto
{
    public string? NickName { get; set; }
    public IFormFile? ProfilePicture { get; set; }
    public bool? AcceptsOnline { get; set; }
    public bool? AcceptsInPerson { get; set; }
    public string? Location { get; set; }
    public int? HourlyRate { get; set; }
    public string? Description { get; set; }
    public string? Contact { get; set; }
    public List<string>? SubjectNames { get; set; }
}
