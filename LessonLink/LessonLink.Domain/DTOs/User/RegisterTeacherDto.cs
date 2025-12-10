using System.ComponentModel.DataAnnotations;

namespace LessonLink.BusinessLogic.DTOs.User;

public class RegisterTeacherDto
{
    public required string FirstName { get; set; }
    public required string SurName { get; set; }

    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }

    public required bool AcceptsOnline { get; set; }
    public required bool AcceptsInPerson { get; set; }
    public required string? Location { get; set; }
    public required int HourlyRate { get; set; }
    public required string Contact { get; set; }
    public required List<string> SubjectNames { get; set; }
}
