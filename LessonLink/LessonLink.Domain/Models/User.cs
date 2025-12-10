using Microsoft.AspNetCore.Identity;

namespace LessonLink.BusinessLogic.Models;

public class User : IdentityUser
{
    public required string FirstName { get; set; }
    public required string SurName { get; set; }
    public required string NickName { get; set; }

    public string? ImageUrl { get; set; }

    public List<Booking> Bookings { get; set; } = [];

}
