using Microsoft.AspNetCore.Identity;

namespace LessonLink.BusinessLogic.Models;

/// <summary>
/// Represents a user account in the lesson booking system.
/// Extends ASP.NET Core Identity's IdentityUser to include additional profile information.
/// Users can have Student, Teacher, or Admin roles (or multiple roles simultaneously).
/// </summary>
public class User : IdentityUser
{
    /// <summary>
    /// The user's first (given) name.
    /// Required during registration.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// The user's surname (family name).
    /// Required during registration.
    /// </summary>
    public required string SurName { get; set; }

    /// <summary>
    /// The user's display name shown throughout the application.
    /// Defaults to the user's first name upon registration but can be customized.
    /// </summary>
    public required string NickName { get; set; }

    /// <summary>
    /// URL to the user's profile image stored in Cloudinary.
    /// Optional - null if no profile picture has been uploaded.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Collection of bookings made by this user as a student.
    /// Represents all lesson slots this user has booked with teachers.
    /// </summary>
    public List<Booking> Bookings { get; set; } = [];

}
