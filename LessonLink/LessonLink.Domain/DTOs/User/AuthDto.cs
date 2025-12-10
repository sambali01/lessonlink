namespace LessonLink.BusinessLogic.DTOs.User;

public class AuthDto
{
    public required string Id { get; set; }
    public required string Token { get; set; }
    public required IList<string> Roles { get; set; }
}
