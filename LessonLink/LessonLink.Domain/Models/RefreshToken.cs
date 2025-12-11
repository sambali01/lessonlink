namespace LessonLink.BusinessLogic.Models;

/// <summary>
/// Represents a refresh token used for JWT authentication.
/// Refresh tokens allow users to obtain new access tokens without re-entering credentials.
/// They have a longer lifespan than access tokens and are stored in the database.
/// When a user logs out or is inactive for too long, refresh tokens are invalidated.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// The refresh token string value.
    /// This is a cryptographically secure random string used to identify and validate the token.
    /// Serves as the primary key in the database.
    /// </summary>
    public required string Value { get; set; }

    /// <summary>
    /// The UTC timestamp when this refresh token was created.
    /// Used for token management and auditing purposes.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The UTC timestamp when this refresh token expires.
    /// After this time, the token can no longer be used to obtain new access tokens.
    /// Users must re-authenticate when their refresh token expires.
    /// </summary>
    public required DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Foreign key to the User who owns this refresh token.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Navigation property to the User associated with this refresh token.
    /// </summary>
    public User User { get; set; } = null!;
}
