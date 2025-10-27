using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LessonLink.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly LessonLinkDbContext _dbContext;

    public BookingRepository(LessonLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Booking>> GetByStudentIdAsync(string studentId)
    {
        return await _dbContext.Bookings
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
        return await _dbContext.Bookings
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
        return await _dbContext.Bookings
            .Include(b => b.Student)
            .Include(b => b.AvailableSlot)
                .ThenInclude(a => a.Teacher)
                    .ThenInclude(t => t.User)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Booking> CreateAsync(Booking booking)
    {
        _dbContext.Bookings.Add(booking);
        await _dbContext.SaveChangesAsync();
        return booking;
    }

    public async Task UpdateAsync(Booking booking)
    {
        _dbContext.Bookings.Update(booking);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Booking booking)
    {
        _dbContext.Bookings.Remove(booking);
        await _dbContext.SaveChangesAsync();
    }
}