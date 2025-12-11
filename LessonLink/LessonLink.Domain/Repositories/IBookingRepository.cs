using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories;

/// <summary>
/// Repository interface for managing Booking entities.
/// Provides data access operations for lesson bookings including
/// retrieval, overlap detection, and CRUD operations.
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Retrieves all bookings made by a specific student.
    /// Includes related Student, AvailableSlot, Teacher, and User entities.
    /// Results are ordered by creation date (most recent first).
    /// </summary>
    /// <param name="studentId">The student's (user's) unique identifier.</param>
    /// <returns>A read-only collection of bookings with related entities.</returns>
    Task<IReadOnlyCollection<Booking>> GetByStudentIdAsync(string studentId);

    /// <summary>
    /// Retrieves all bookings for lessons offered by a specific teacher.
    /// Includes related Student, AvailableSlot, Teacher, and User entities.
    /// Results are ordered by creation date (most recent first).
    /// </summary>
    /// <param name="teacherId">The teacher's unique identifier.</param>
    /// <returns>A read-only collection of bookings for the teacher's slots.</returns>
    Task<IReadOnlyCollection<Booking>> GetByTeacherIdAsync(string teacherId);

    /// <summary>
    /// Retrieves a specific booking by its unique identifier.
    /// Includes related Student, AvailableSlot, Teacher, and User entities.
    /// </summary>
    /// <param name="id">The booking's unique identifier.</param>
    /// <returns>The booking with related entities if found, null otherwise.</returns>
    Task<Booking?> GetByIdAsync(int id);

    /// <summary>
    /// Checks if a student has any active (non-cancelled) booking that overlaps with the specified time range.
    /// Used to prevent students from booking multiple lessons at the same time.
    /// Since teachers can also be students, this prevents them from booking lessons during their own teaching slots.
    /// </summary>
    /// <param name="studentId">The student's (user's) unique identifier.</param>
    /// <param name="startTime">The start time of the time range to check.</param>
    /// <param name="endTime">The end time of the time range to check.</param>
    /// <returns>True if an overlapping active booking exists, false otherwise.</returns>
    Task<bool> HasOverlappingActiveBookingForStudentAsync(string studentId, DateTime startTime, DateTime endTime);

    /// <summary>
    /// Checks if a teacher has any active (non-cancelled) booking as a student that overlaps with the specified time range.
    /// Used when a teacher (who is also a student) tries to create an available slot.
    /// Prevents teachers from creating teaching slots during times they have booked lessons with other teachers.
    /// </summary>
    /// <param name="teacherId">The teacher's unique identifier (same as their user ID).</param>
    /// <param name="startTime">The start time of the time range to check.</param>
    /// <param name="endTime">The end time of the time range to check.</param>
    /// <returns>True if the teacher has an overlapping booking as a student, false otherwise.</returns>
    Task<bool> HasOverlappingActiveBookingForTeacherAsync(string teacherId, DateTime startTime, DateTime endTime);

    /// <summary>
    /// Creates a new booking.
    /// Does not save changes to the database - call UnitOfWork.CompleteAsync() to persist.
    /// </summary>
    /// <param name="booking">The booking to create.</param>
    /// <returns>The created booking entity (tracked by EF Core).</returns>
    Booking CreateAsync(Booking booking);

    /// <summary>
    /// Marks a booking as modified for update.
    /// Does not save changes to the database - call UnitOfWork.CompleteAsync() to persist.
    /// </summary>
    /// <param name="booking">The booking with updated properties.</param>
    void UpdateAsync(Booking booking);

    /// <summary>
    /// Marks a booking for deletion.
    /// Does not save changes to the database - call UnitOfWork.CompleteAsync() to persist.
    /// </summary>
    /// <param name="booking">The booking to delete.</param>
    void DeleteAsync(Booking booking);

    /// <summary>
    /// Marks multiple bookings for deletion in a single operation.
    /// Used when deleting an available slot to remove all associated bookings.
    /// Does not save changes to the database - call UnitOfWork.CompleteAsync() to persist.
    /// </summary>
    /// <param name="bookings">The collection of bookings to delete.</param>
    void DeleteRangeAsync(IEnumerable<Booking> bookings);
}
