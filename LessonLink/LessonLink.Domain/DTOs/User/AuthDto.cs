namespace LessonLink.BusinessLogic.DTOs.User;

public class AuthDto
{
    public string Id { get; set; }
    public string Token { get; set; }
    public IEnumerable<string> Roles { get; set; }
}
