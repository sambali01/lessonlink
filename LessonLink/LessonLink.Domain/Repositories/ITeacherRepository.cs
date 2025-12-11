using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories;

/// <summary>
/// Repository interface for managing Teacher entities.
/// Provides data access operations for teacher profiles including
/// search, filtering, and CRUD operations.
/// </summary>
public interface ITeacherRepository
{
    /// <summary>
    /// Retrieves all teachers in the system with their related User and Subject information.
    /// Includes: User, TeacherSubjects, and TeacherSubjects.Subject.
    /// </summary>
    /// <returns>A read-only collection of all teachers with related entities.</returns>
    Task<IReadOnlyCollection<Teacher>> GetAllAsync();

    /// <summary>
    /// Retrieves a curated list of featured teachers to display on the home page.
    /// Typically returns a subset of teachers (e.g., most popular or recently active).
    /// Includes: User, TeacherSubjects, and TeacherSubjects.Subject.
    /// </summary>
    /// <returns>A read-only collection of featured teachers.</returns>
    Task<IReadOnlyCollection<Teacher>> GetFeaturedAsync();

    /// <summary>
    /// Retrieves a specific teacher by their unique identifier.
    /// Includes: User, TeacherSubjects, and TeacherSubjects.Subject.
    /// </summary>
    /// <param name="id">The teacher's unique identifier (same as their user ID).</param>
    /// <returns>The teacher with related entities if found, null otherwise.</returns>
    Task<Teacher?> GetByIdAsync(string id);

    /// <summary>
    /// Searches for teachers based on multiple filter criteria with pagination.
    /// Allows filtering by text search, subjects, price range, teaching methods, and location.
    /// Includes: User, TeacherSubjects, and TeacherSubjects.Subject.
    /// </summary>
    /// <param name="searchText">Optional text to search in teacher name or description.</param>
    /// <param name="subjects">List of subject names to filter by (empty list means no subject filter).</param>
    /// <param name="minPrice">Optional minimum hourly rate filter.</param>
    /// <param name="maxPrice">Optional maximum hourly rate filter.</param>
    /// <param name="acceptsOnline">Optional filter for teachers offering online lessons.</param>
    /// <param name="acceptsInPerson">Optional filter for teachers offering in-person lessons.</param>
    /// <param name="location">Optional city/location filter for in-person lessons.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of teachers per page.</param>
    /// <returns>A tuple containing the list of matching teachers and the total count of matches.</returns>
    Task<(List<Teacher>, int)> SearchAsync(
        string? searchText,
        List<string> subjects,
        int? minPrice,
        int? maxPrice,
        bool? acceptsOnline,
        bool? acceptsInPerson,
        string? location,
        int page,
        int pageSize);

    /// <summary>
    /// Creates a new teacher profile.
    /// Does not save changes to the database - call UnitOfWork.CompleteAsync() to persist.
    /// </summary>
    /// <param name="teacher">The teacher to create.</param>
    /// <returns>The created teacher entity (tracked by EF Core).</returns>
    Teacher CreateAsync(Teacher teacher);

    /// <summary>
    /// Marks a teacher as modified for update.
    /// Does not save changes to the database - call UnitOfWork.CompleteAsync() to persist.
    /// </summary>
    /// <param name="updatedTeacher">The teacher with updated properties.</param>
    void UpdateAsync(Teacher updatedTeacher);

    /// <summary>
    /// Marks a teacher for deletion.
    /// Does not save changes to the database - call UnitOfWork.CompleteAsync() to persist.
    /// </summary>
    /// <param name="teacher">The teacher to delete.</param>
    void DeleteAsync(Teacher teacher);
}
