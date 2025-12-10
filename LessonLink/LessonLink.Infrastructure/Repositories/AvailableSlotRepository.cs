using LessonLink.BusinessLogic.DTOs.Common;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LessonLink.Infrastructure.Repositories;

public class AvailableSlotRepository(LessonLinkDbContext dbContext) : IAvailableSlotRepository
{
    public async Task<AvailableSlot?> GetByIdAsync(int id)
    {
        return await dbContext.AvailableSlots
            .FirstOrDefaultAsync(slot => slot.Id == id);
    }

    public async Task<AvailableSlot?> GetByIdWithBookingAsync(int id)
    {
        return await dbContext.AvailableSlots
            .Include(slot => slot.Teacher)
            .Include(slot => slot.Bookings)
                .ThenInclude(booking => booking.Student)
            .FirstOrDefaultAsync(slot => slot.Id == id);
    }

    public async Task<IReadOnlyCollection<AvailableSlot>> GetByTeacherIdAsync(string teacherId)
    {
        return await dbContext.AvailableSlots
            .Where(slot => slot.TeacherId == teacherId)
            .OrderBy(slot => slot.StartTime)
            .ToListAsync();
    }

    public async Task<PaginatedResponse<AvailableSlot>> GetByTeacherIdPaginatedAsync(string teacherId, int page, int pageSize)
    {
        var query = dbContext.AvailableSlots
            .Where(slot => slot.TeacherId == teacherId)
            .OrderBy(slot => slot.StartTime);

        var totalCount = await query.CountAsync();

        var uniqueDates = await query
            .Select(slot => slot.StartTime.Date)
            .Distinct()
            .OrderByDescending(date => date)
            .ToListAsync();

        if (uniqueDates.Count == 0)
        {
            return new PaginatedResponse<AvailableSlot>
            {
                Items = [],
                TotalCount = 0,
                Page = page,
                PageSize = 0,
                TotalPages = 0
            };
        }

        var daysPerPage = await CalculateOptimalDaysPerPage(teacherId, uniqueDates, pageSize);
        var totalPages = (int)Math.Ceiling((double)uniqueDates.Count / daysPerPage);

        var startIndex = (page - 1) * daysPerPage;
        var selectedDates = uniqueDates
            .Skip(startIndex)
            .Take(daysPerPage)
            .ToList();

        if (selectedDates.Count == 0)
        {
            return new PaginatedResponse<AvailableSlot>
            {
                Items = [],
                TotalCount = totalCount,
                Page = page,
                PageSize = 0,
                TotalPages = totalPages
            };
        }

        var startDate = selectedDates.First();
        var endDate = selectedDates.Last().AddDays(1);

        var items = await query
            .Where(slot => slot.StartTime.Date >= startDate && slot.StartTime.Date < endDate)
            .ToListAsync();

        return new PaginatedResponse<AvailableSlot>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = items.Count,
            TotalPages = totalPages
        };
    }

    public async Task<IReadOnlyCollection<AvailableSlot>> GetByTeacherIdWithBookingsAsync(string teacherId)
    {
        return await dbContext.AvailableSlots
            .Include(slot => slot.Bookings)
            .ThenInclude(booking => booking.Student)
            .Where(slot => slot.TeacherId == teacherId)
            .OrderBy(slot => slot.StartTime)
            .ToListAsync();
    }

    public async Task<PaginatedResponse<AvailableSlot>> GetByTeacherIdWithBookingsPaginatedAsync(string teacherId, int page, int pageSize)
    {
        var query = dbContext.AvailableSlots
            .Include(slot => slot.Bookings)
            .ThenInclude(booking => booking.Student)
            .Where(slot => slot.TeacherId == teacherId)
            .OrderBy(slot => slot.StartTime);

        var totalCount = await query.CountAsync();

        var uniqueDates = await dbContext.AvailableSlots
            .Where(slot => slot.TeacherId == teacherId)
            .Select(slot => slot.StartTime.Date)
            .Distinct()
            .OrderBy(date => date)
            .ToListAsync();

        if (!uniqueDates.Any())
        {
            return new PaginatedResponse<AvailableSlot>
            {
                Items = new List<AvailableSlot>(),
                TotalCount = 0,
                Page = page,
                PageSize = 0,
                TotalPages = 0
            };
        }

        var daysPerPage = await CalculateOptimalDaysPerPage(teacherId, uniqueDates, pageSize);
        var totalPages = (int)Math.Ceiling((double)uniqueDates.Count / daysPerPage);

        var startIndex = (page - 1) * daysPerPage;
        var selectedDates = uniqueDates
            .Skip(startIndex)
            .Take(daysPerPage)
            .ToList();

        if (!selectedDates.Any())
        {
            return new PaginatedResponse<AvailableSlot>
            {
                Items = new List<AvailableSlot>(),
                TotalCount = totalCount,
                Page = page,
                PageSize = 0,
                TotalPages = totalPages
            };
        }

        var startDate = selectedDates.First();
        var endDate = selectedDates.Last().AddDays(1); // Start of the day after the last selected date

        var items = await query
            .Where(slot => slot.StartTime.Date >= startDate && slot.StartTime.Date < endDate)
            .ToListAsync();

        return new PaginatedResponse<AvailableSlot>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = items.Count,
            TotalPages = totalPages
        };
    }

    public async Task<IReadOnlyCollection<AvailableSlot>> GetNotBookedByTeacherIdAsync(string teacherId)
    {
        return await dbContext.AvailableSlots
            .Where(slot => slot.TeacherId == teacherId &&
                !dbContext.Bookings.Any(booking => booking.AvailableSlotId == slot.Id && (booking.Status == BookingStatus.Pending || booking.Status == BookingStatus.Confirmed))
            )
            .OrderBy(slot => slot.StartTime)
            .ToListAsync();
    }

    public async Task<bool> HasOverlappingSlotAsync(string teacherId, DateTime startTime, DateTime endTime)
    {
        return await dbContext.AvailableSlots
            .AnyAsync(slot => slot.TeacherId == teacherId &&
                ((startTime >= slot.StartTime && startTime < slot.EndTime) ||
                 (endTime > slot.StartTime && endTime <= slot.EndTime) ||
                 (startTime <= slot.StartTime && endTime >= slot.EndTime)));
    }
    public async Task<bool> HasBookingAsync(int slotId)
    {
        return await dbContext.Bookings
            .AnyAsync(booking => booking.AvailableSlotId == slotId && booking.Status != BookingStatus.Cancelled);
    }

    public AvailableSlot CreateAsync(AvailableSlot slot)
    {
        dbContext.AvailableSlots.Add(slot);
        return slot;
    }

    public void UpdateAsync(AvailableSlot slot)
    {
        dbContext.AvailableSlots.Update(slot);
    }

    public void DeleteAsync(AvailableSlot slot)
    {
        dbContext.AvailableSlots.Remove(slot);
    }

    private async Task<int> CalculateOptimalDaysPerPage(string teacherId, List<DateTime> uniqueDates, int targetPageSize)
    {
        if (uniqueDates.Count == 0) return 1;

        var sampleSize = Math.Min(10, uniqueDates.Count);
        var sampleDates = uniqueDates.Take(sampleSize).ToList();

        var totalSlotsInSample = 0;
        foreach (var date in sampleDates)
        {
            var slotsOnDate = await dbContext.AvailableSlots
                .Where(slot => slot.TeacherId == teacherId && slot.StartTime.Date == date)
                .CountAsync();
            totalSlotsInSample += slotsOnDate;
        }

        var avgSlotsPerDay = (double)totalSlotsInSample / sampleDates.Count;

        if (avgSlotsPerDay == 0) return 1;

        var estimatedDays = (int)Math.Ceiling(targetPageSize / avgSlotsPerDay);

        return Math.Max(1, Math.Min(estimatedDays, uniqueDates.Count));
    }
}
