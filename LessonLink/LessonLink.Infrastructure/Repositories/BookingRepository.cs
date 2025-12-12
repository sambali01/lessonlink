using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LessonLink.Infrastructure.Repositories;

/// <summary>
/// Implementation of IBookingRepository using Entity Framework Core.
/// Manages database operations for Booking entities including retrieval,
/// conflict detection, and CRUD operations.
/// </summary>
public class BookingRepository(LessonLinkDbContext dbContext) : IBookingRepository
{
    public async Task<IReadOnlyCollection<Booking>> GetByStudentIdAsync(string studentId)
    {
        // Include all related entities needed for displaying booking details
        return await dbContext.Bookings
            .Include(b => b.Student)
            .Include(b => b.AvailableSlot)
                .ThenInclude(a => a.Teacher)
                    .ThenInclude(t => t.User)
            .Where(b => b.StudentId == studentId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<Booking>> GetByTeacherIdAsync(string teacherId)
    {
        // Retrieve bookings for a teacher's available slots
        return await dbContext.Bookings
            .Include(b => b.Student)
            .Include(b => b.AvailableSlot)
                .ThenInclude(a => a.Teacher)
                    .ThenInclude(t => t.User)
            .Where(b => b.AvailableSlot.TeacherId == teacherId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await dbContext.Bookings
            .Include(b => b.Student)
            .Include(b => b.AvailableSlot)
                .ThenInclude(a => a.Teacher)
                    .ThenInclude(t => t.User)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public Booking CreateAsync(Booking booking)
    {
        dbContext.Bookings.Add(booking);
        return booking;
    }

    public void UpdateAsync(Booking booking)
    {
        dbContext.Bookings.Update(booking);
    }

    public void DeleteAsync(Booking booking)
    {
        dbContext.Bookings.Remove(booking);
    }

    public void DeleteRangeAsync(IEnumerable<Booking> bookings)
    {
        dbContext.Bookings.RemoveRange(bookings);
    }

    public async Task<bool> HasOverlappingActiveBookingForStudentAsync(string studentId, DateTime startTime, DateTime endTime)
    {
        // Check if student has any non-cancelled booking that conflicts with the given time range
        // This prevents double-booking: a student cannot book two lessons at the same time
        return await dbContext.Bookings
            .Include(b => b.AvailableSlot)
            .AnyAsync(b =>
                b.StudentId == studentId &&
                b.Status != BookingStatus.Cancelled &&
                b.AvailableSlot.StartTime < endTime &&
                b.AvailableSlot.EndTime > startTime);
    }

    public async Task<bool> HasOverlappingActiveBookingForTeacherAsync(string teacherId, DateTime startTime, DateTime endTime)
    {
        // Check if a teacher (acting as a student) has bookings that conflict with a new teaching slot
        // Since teachers can also book lessons, we need to prevent them from creating teaching slots
        // during times they have booked to be students elsewhere
        return await dbContext.Bookings
            .Include(b => b.AvailableSlot)
            .AnyAsync(b =>
                b.AvailableSlot.TeacherId == teacherId &&
                b.Status != BookingStatus.Cancelled &&
                b.AvailableSlot.StartTime < endTime &&
                b.AvailableSlot.EndTime > startTime);
    }
}
