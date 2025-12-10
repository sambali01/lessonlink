using Microsoft.AspNetCore.Http;

namespace LessonLink.BusinessLogic.DTOs.User;

public class StudentUpdateRequest
{
    public string? NickName { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}
