using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LessonLink.Infrastructure.Repositories;

public class BookingRepository(LessonLinkDbContext dbContext) : IBookingRepository
{
    public async Task<IReadOnlyCollection<Booking>> GetByStudentIdAsync(string studentId)
    {
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
        return await dbContext.Bookings
            .Include(b => b.AvailableSlot)
            .AnyAsync(b =>
                b.AvailableSlot.TeacherId == teacherId &&
                b.Status != BookingStatus.Cancelled &&
                b.AvailableSlot.StartTime < endTime &&
                b.AvailableSlot.EndTime > startTime);
    }
}
