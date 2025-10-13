using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LessonLink.Infrastructure.Repositories;

public class AvailableSlotRepository : IAvailableSlotRepository
{
    private readonly LessonLinkDbContext _dbContext;

    public AvailableSlotRepository(LessonLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<AvailableSlot>> GetByTeacherIdAsync(string teacherId)
    {
        return await _dbContext.AvailableSlots
            .Where(slot => slot.TeacherId == teacherId)
            .OrderBy(slot => slot.StartTime)
            .ToListAsync();
    }

    public async Task<AvailableSlot?> GetByIdAsync(int id)
    {
        return await _dbContext.AvailableSlots
            .FirstOrDefaultAsync(slot => slot.Id == id);
    }

    public async Task<AvailableSlot> CreateAsync(AvailableSlot slot)
    {
        _dbContext.AvailableSlots.Add(slot);
        await _dbContext.SaveChangesAsync();
        return slot;
    }

    public async Task UpdateAsync(AvailableSlot slot)
    {
        _dbContext.AvailableSlots.Update(slot);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(AvailableSlot slot)
    {
        _dbContext.AvailableSlots.Remove(slot);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> HasOverlappingSlotAsync(string teacherId, DateTime startTime, DateTime endTime)
    {
        return await _dbContext.AvailableSlots
            .AnyAsync(slot => slot.TeacherId == teacherId &&
                             ((startTime >= slot.StartTime && startTime < slot.EndTime) ||
                              (endTime > slot.StartTime && endTime <= slot.EndTime) ||
                              (startTime <= slot.StartTime && endTime >= slot.EndTime)));
    }
    public async Task<bool> HasBookingAsync(int slotId)
    {
        return await _dbContext.Bookings
            .AnyAsync(booking => booking.AvailableSlotId == slotId && booking.Status != BookingStatus.Cancelled);
    }
}
