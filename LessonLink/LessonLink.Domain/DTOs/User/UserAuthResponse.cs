namespace LessonLink.BusinessLogic.DTOs.User;

public class UserAuthResponse
{
    public required string Id { get; set; }
    public required string Token { get; set; }
    public required IList<string> Roles { get; set; }
}
