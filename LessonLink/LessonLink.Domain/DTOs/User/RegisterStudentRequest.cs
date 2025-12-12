using System.ComponentModel.DataAnnotations;

namespace LessonLink.BusinessLogic.DTOs.User;

public class RegisterStudentRequest
{
    public required string FirstName { get; set; }
    public required string SurName { get; set; }

    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }
}

