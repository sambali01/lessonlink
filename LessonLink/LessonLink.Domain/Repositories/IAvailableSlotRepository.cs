using LessonLink.BusinessLogic.DTOs.Common;
using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories
{
    /// <summary>
    /// Repository interface for managing AvailableSlot entities.
    /// Provides data access operations for teacher-created time slots including
    /// retrieval, pagination, overlap detection, and CRUD operations.
    /// </summary>
    public interface IAvailableSlotRepository
    {
        /// <summary>
        /// Retrieves an available slot by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the slot.</param>
        /// <returns>The available slot if found, null otherwise.</returns>
        Task<AvailableSlot?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves an available slot by its unique identifier, including related booking and teacher information.
        /// Eagerly loads: Teacher, Teacher.User, Bookings, and Bookings.Student.
        /// </summary>
        /// <param name="id">The unique identifier of the slot.</param>
        /// <returns>The available slot with related entities if found, null otherwise.</returns>
        Task<AvailableSlot?> GetByIdWithBookingAsync(int id);

        /// <summary>
        /// Retrieves all available slots for a specific teacher, ordered by start time.
        /// </summary>
        /// <param name="teacherId">The teacher's unique identifier.</param>
        /// <returns>A read-only collection of available slots.</returns>
        Task<IReadOnlyCollection<AvailableSlot>> GetByTeacherIdAsync(string teacherId);

        /// <summary>
        /// Retrieves available slots for a specific teacher with date-based pagination.
        /// Groups slots by date and paginates based on optimal days per page calculation.
        /// </summary>
        /// <param name="teacherId">The teacher's unique identifier.</param>
        /// <param name="page">The page number (1-based).</param>
        /// <param name="pageSize">The target number of items per page.</param>
        /// <returns>A paginated response containing slots and pagination metadata.</returns>
        Task<PaginatedResponse<AvailableSlot>> GetByTeacherIdPaginatedAsync(string teacherId, int page, int pageSize);

        /// <summary>
        /// Retrieves all available slots for a specific teacher with related booking and user information.
        /// Eagerly loads: Teacher, Teacher.User, Bookings, and Bookings.Student.
        /// </summary>
        /// <param name="teacherId">The teacher's unique identifier.</param>
        /// <returns>A read-only collection of available slots with related entities.</returns>
        Task<IReadOnlyCollection<AvailableSlot>> GetByTeacherIdWithBookingsAsync(string teacherId);

        /// <summary>
        /// Retrieves available slots for a specific teacher with related entities, using date-based pagination.
        /// Includes Teacher, User, Bookings, and Student information.
        /// </summary>
        /// <param name="teacherId">The teacher's unique identifier.</param>
        /// <param name="page">The page number (1-based).</param>
        /// <param name="pageSize">The target number of items per page.</param>
        /// <returns>A paginated response with slots and related entities.</returns>
        Task<PaginatedResponse<AvailableSlot>> GetByTeacherIdWithBookingsPaginatedAsync(string teacherId, int page, int pageSize);

        /// <summary>
        /// Retrieves current and future available slots for a specific teacher with pagination.
        /// Filters out past slots (StartTime < UtcNow). Includes related booking and user entities.
        /// </summary>
        /// <param name="teacherId">The teacher's unique identifier.</param>
        /// <param name="page">The page number (1-based).</param>
        /// <param name="pageSize">The target number of items per page.</param>
        /// <returns>A paginated response containing only current/future slots.</returns>
        Task<PaginatedResponse<AvailableSlot>> GetCurrentSlotsByTeacherIdWithBookingsPaginatedAsync(string teacherId, int page, int pageSize);

        /// <summary>
        /// Retrieves past available slots for a specific teacher with pagination.
        /// Filters slots where StartTime < UtcNow. Includes related booking and user entities.
        /// </summary>
        /// <param name="teacherId">The teacher's unique identifier.</param>
        /// <param name="page">The page number (1-based).</param>
        /// <param name="pageSize">The target number of items per page.</param>
        /// <returns>A paginated response containing only past slots.</returns>
        Task<PaginatedResponse<AvailableSlot>> GetPastSlotsByTeacherIdWithBookingsPaginatedAsync(string teacherId, int page, int pageSize);

        /// <summary>
        /// Retrieves current and future available slots for a teacher that have no active bookings.
        /// Used to show teachers which slots are still available for students to book.
        /// </summary>
        /// <param name="teacherId">The teacher's unique identifier.</param>
        /// <param name="page">The page number (1-based).</param>
        /// <param name="pageSize">The target number of items per page.</param>
        /// <returns>A paginated response containing unbooked current/future slots.</returns>
        Task<PaginatedResponse<AvailableSlot>> GetCurrentNotBookedSlotsByTeacherIdPaginatedAsync(string teacherId, int page, int pageSize);

        /// <summary>
        /// Checks if a teacher has any existing available slot that overlaps with the specified time range.
        /// Used to prevent teachers from creating conflicting time slots.
        /// </summary>
        /// <param name="teacherId">The teacher's unique identifier.</param>
        /// <param name="startTime">The start time of the time range to check.</param>
        /// <param name="endTime">The end time of the time range to check.</param>
        /// <param name="excludeSlotId">Optional slot ID to exclude from the check (useful when updating an existing slot).</param>
        /// <returns>True if an overlapping slot exists, false otherwise.</returns>
        Task<bool> HasOverlappingSlotAsync(string teacherId, DateTime startTime, DateTime endTime, int? excludeSlotId = null);

        /// <summary>
        /// Checks if a specific slot has any associated bookings (of any status).
        /// Used to determine if a slot can be edited or deleted.
        /// </summary>
        /// <param name="slotId">The slot's unique identifier.</param>
        /// <returns>True if the slot has bookings, false otherwise.</returns>
        Task<bool> HasBookingAsync(int slotId);

        /// <summary>
        /// Creates a new available slot.
        /// Does not save changes to the database - call UnitOfWork.CompleteAsync() to persist.
        /// </summary>
        /// <param name="slot">The available slot to create.</param>
        /// <returns>The created slot entity (tracked by EF Core).</returns>
        AvailableSlot CreateAsync(AvailableSlot slot);

        /// <summary>
        /// Marks an available slot as modified for update.
        /// Does not save changes to the database - call UnitOfWork.CompleteAsync() to persist.
        /// </summary>
        /// <param name="slot">The available slot with updated properties.</param>
        void UpdateAsync(AvailableSlot slot);

        /// <summary>
        /// Marks an available slot for deletion.
        /// Does not save changes to the database - call UnitOfWork.CompleteAsync() to persist.
        /// </summary>
        /// <param name="slot">The available slot to delete.</param>
        void DeleteAsync(AvailableSlot slot);
    }
}
