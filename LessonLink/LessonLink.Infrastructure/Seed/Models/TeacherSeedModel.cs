namespace LessonLink.Infrastructure.Seed.Models;

public class TeacherSeedModel
{
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string SurName { get; set; }
    public required string NickName { get; set; }
    public string? ImageUrl { get; set; }

    // Teacher specific properties
    public bool AcceptsOnline { get; set; }
    public bool AcceptsInPerson { get; set; }
    public string? Location { get; set; }
    public int HourlyRate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
}
