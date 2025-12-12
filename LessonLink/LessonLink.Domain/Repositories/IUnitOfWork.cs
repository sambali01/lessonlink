namespace LessonLink.BusinessLogic.Repositories;

/// <summary>
/// Unit of Work pattern implementation that coordinates multiple repository operations.
/// Ensures that multiple data modifications are treated as a single transaction.
/// Provides centralized access to all repositories and a single point for saving changes.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets the repository for refresh token operations.
    /// </summary>
    IRefreshTokenRepository RefreshTokenRepository { get; }

    /// <summary>
    /// Gets the repository for teacher operations.
    /// </summary>
    ITeacherRepository TeacherRepository { get; }

    /// <summary>
    /// Gets the repository for subject operations.
    /// </summary>
    ISubjectRepository SubjectRepository { get; }

    /// <summary>
    /// Gets the repository for teacher-subject relationship operations.
    /// </summary>
    ITeacherSubjectRepository TeacherSubjectRepository { get; }

    /// <summary>
    /// Gets the repository for available slot operations.
    /// </summary>
    IAvailableSlotRepository AvailableSlotRepository { get; }

    /// <summary>
    /// Gets the repository for booking operations.
    /// </summary>
    IBookingRepository BookingRepository { get; }

    /// <summary>
    /// Saves all pending changes made through any repository to the database as a single transaction.
    /// If any operation fails, all changes are rolled back.
    /// </summary>
    /// <returns>True if changes were successfully saved, false otherwise.</returns>
    Task<bool> CompleteAsync();
}
