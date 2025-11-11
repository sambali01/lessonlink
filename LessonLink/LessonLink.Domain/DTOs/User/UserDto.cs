namespace LessonLink.BusinessLogic.DTOs.User;

public class UserDto
{
    public string Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string SurName { get; set; } = default!;
    public string? NickName { get; set; } = default!;
    public string? Email { get; set; } = default!;
    public string? ImageUrl { get; set; }
}
