using System.ComponentModel.DataAnnotations;

namespace LessonLink.BusinessLogic.DTOs.User;

public class RegisterDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string SurName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; }
}

