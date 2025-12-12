using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories;

/// <summary>
/// Repository interface for managing Subject entities.
/// Provides data access operations for academic subjects that teachers can teach.
/// </summary>
public interface ISubjectRepository
{
    /// <summary>
    /// Retrieves all subjects in the system.
    /// Used for populating subject selection dropdowns and filters.
    /// </summary>
    /// <returns>A read-only collection of all subjects.</returns>
    Task<IReadOnlyCollection<Subject>> GetAllAsync();

    /// <summary>
    /// Retrieves a specific subject by its unique identifier.
    /// </summary>
    /// <param name="id">The subject's unique identifier.</param>
    /// <returns>The subject if found, null otherwise.</returns>
    Task<Subject?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a subject by its name.
    /// Used for checking if a subject already exists before creating a new one.
    /// </summary>
    /// <param name="name">The name of the subject (case-sensitive).</param>
    /// <returns>The subject if found, null otherwise.</returns>
    Task<Subject?> GetByNameAsync(string name);

    /// <summary>
    /// Creates a new subject.
    /// Does not save changes to the database - call UnitOfWork.CompleteAsync() to persist.
    /// </summary>
    /// <param name="subject">The subject to create.</param>
    /// <returns>The created subject entity (tracked by EF Core).</returns>
    Subject CreateAsync(Subject subject);
}
