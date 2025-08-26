using System.ComponentModel.DataAnnotations;

namespace LessonLink.BusinessLogic.DTOs.User;

public class LoginDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
