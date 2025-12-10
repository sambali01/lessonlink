namespace LessonLink.BusinessLogic.Models;

public class RefreshToken
{
    public required string Value { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required DateTime ExpiresAt { get; set; }

    public required string UserId { get; set; }
    public User User { get; set; } = null!;
}
