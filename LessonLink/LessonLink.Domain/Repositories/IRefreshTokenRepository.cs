using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories;

/// <summary>
/// Repository interface for managing RefreshToken entities.
/// Handles refresh token storage and validation for JWT authentication.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Retrieves a refresh token by its hashed value.
    /// Used during token refresh operations to validate and find the token.
    /// Includes the related User entity.
    /// </summary>
    /// <param name="token">The hashed refresh token value to search for.</param>
    /// <returns>The refresh token with related User if found, null otherwise.</returns>
    Task<RefreshToken?> GetByHashedValueAsync(string token);

    /// <summary>
    /// Creates a new refresh token.
    /// Used when a user logs in to generate a new refresh token.
    /// Does not save changes to the database - call UnitOfWork.CompleteAsync() to persist.
    /// </summary>
    /// <param name="token">The refresh token to create.</param>
    void CreateAsync(RefreshToken token);

    /// <summary>
    /// Marks a refresh token for deletion.
    /// Used during logout or when a refresh token is no longer valid.
    /// Does not save changes to the database - call UnitOfWork.CompleteAsync() to persist.
    /// </summary>
    /// <param name="token">The refresh token to delete.</param>
    void DeleteAsync(RefreshToken token);

    /// <summary>
    /// Deletes all refresh tokens associated with a specific user.
    /// Used during logout from all devices or when invalidating all sessions for a user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    Task DeleteAllForUserAsync(string userId);
}
