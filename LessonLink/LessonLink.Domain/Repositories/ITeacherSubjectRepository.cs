using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories
{
    /// <summary>
    /// Repository interface for managing TeacherSubject join table entities.
    /// Handles the many-to-many relationship between Teachers and Subjects.
    /// </summary>
    public interface ITeacherSubjectRepository
    {
        /// <summary>
        /// Retrieves all subject associations for a specific teacher.
        /// Includes the related Subject entity for each association.
        /// </summary>
        /// <param name="teacherId">The teacher's unique identifier.</param>
        /// <returns>A read-only collection of teacher-subject associations with related Subject entities.</returns>
        Task<IReadOnlyCollection<TeacherSubject>> GetByTeacherIdAsync(string teacherId);

        /// <summary>
        /// Creates a new teacher-subject association.
        /// Used when a teacher adds a subject they can teach.
        /// Does not save changes to the database - call UnitOfWork.CompleteAsync() to persist.
        /// </summary>
        /// <param name="teacherSubject">The teacher-subject association to create.</param>
        /// <returns>The created association entity (tracked by EF Core).</returns>
        TeacherSubject CreateAsync(TeacherSubject teacherSubject);

        /// <summary>
        /// Deletes all subject associations for a specific teacher.
        /// Used when updating a teacher's subjects - old associations are removed and new ones are created.
        /// Does not save changes to the database - call UnitOfWork.CompleteAsync() to persist.
        /// </summary>
        /// <param name="teacherId">The teacher's unique identifier.</param>
        void DeleteByTeacherIdAsync(string teacherId);
    }
}
