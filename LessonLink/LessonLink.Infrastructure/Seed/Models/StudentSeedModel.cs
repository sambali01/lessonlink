namespace LessonLink.Infrastructure.Seed.Models;

public class StudentSeedModel
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string SurName { get; set; }
    public required string NickName { get; set; }
    public string? ImageUrl { get; set; }
}
