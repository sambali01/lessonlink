using Microsoft.AspNetCore.Http;

namespace LessonLink.BusinessLogic.DTOs.User;

public class UserUpdateDto
{
    public string? NickName { get; set; }
    public IFormFile? ProfilePicture { get; set; }
    public bool? AcceptsOnline { get; set; }
    public bool? AcceptsInPerson { get; set; }
    public string? Location { get; set; }
    public int? HourlyRate { get; set; }
    public List<string>? Subjects { get; set; }
}
