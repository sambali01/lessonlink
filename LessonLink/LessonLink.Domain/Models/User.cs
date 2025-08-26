using Microsoft.AspNetCore.Identity;

namespace LessonLink.BusinessLogic.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string SurName { get; set; }
    public string NickName { get; set; }
    public byte[]? ProfilePicture { get; set; }

    public List<Booking> Bookings { get; set; } = new List<Booking>();

}
