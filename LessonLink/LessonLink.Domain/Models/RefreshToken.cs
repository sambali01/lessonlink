namespace LessonLink.BusinessLogic.Models;

public class RefreshToken
{
    public string Value { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
}
