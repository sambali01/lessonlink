using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace LessonLink.Infrastructure.Repositories;

public class TeacherRepository : ITeacherRepository
{
    private readonly LessonLinkDbContext _dbContext;

    public TeacherRepository(LessonLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Teacher>> GetAllAsync()
    {
        return await _dbContext.Teachers
            .Include(t => t.User)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<Teacher>> GetFeaturedAsync()
    {
        return await _dbContext.Teachers
            .Include(t => t.User)
            .Include(t => t.TeacherSubjects)
                .ThenInclude(ts => ts.Subject)
            .Include(t => t.AvailableSlots)
                .ThenInclude(slot => slot.Bookings)
            .Select(t => new
            {
                Teacher = t,
                BookingCount = t.AvailableSlots
                    .SelectMany(slot => slot.Bookings)
                    .Count(booking => booking.Status != BookingStatus.Cancelled)
            })
            .OrderByDescending(x => x.BookingCount)
            .Take(4)
            .Select(x => x.Teacher)
            .ToListAsync();
    }

    public async Task<Teacher?> GetByIdAsync(string id)
    {
        return await _dbContext.Teachers
            .Include(t => t.User)
            .Include(t => t.TeacherSubjects)
                .ThenInclude(ts => ts.Subject)
            .FirstOrDefaultAsync(t => t.UserId == id);
    }

    public async Task<(List<Teacher>, int)> SearchAsync(
        string? searchText,
        string[]? subjects,
        int? minPrice,
        int? maxPrice,
        bool? acceptsOnline,
        bool? acceptsInPerson,
        string? location,
        int page,
        int pageSize)
    {
        var query = _dbContext.Teachers
            .Include(t => t.User)
            .Include(t => t.TeacherSubjects)
                .ThenInclude(ts => ts.Subject)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchText))
        {
            query = query.Where(t =>
                t.User.FirstName.Contains(searchText) ||
                t.User.SurName.Contains(searchText) ||
                t.User.NickName.Contains(searchText) ||
                (t.Description != null && t.Description.Contains(searchText)));
        }

        if (subjects != null && subjects.Length != 0)
        {
            query = query.Where(t => t.TeacherSubjects
                .Any(ts => subjects.Contains(ts.Subject.Name)));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(t => t.HourlyRate >= minPrice);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(t => t.HourlyRate <= maxPrice);
        }

        if (acceptsOnline.HasValue && acceptsOnline.Value)
        {
            query = query.Where(t => t.AcceptsOnline == true);
        }

        if (acceptsInPerson.HasValue && acceptsInPerson.Value)
        {
            query = query.Where(t => t.AcceptsInPerson == true);

            if (location != null)
            {
                query = query.Where(t => t.Location != null && t.Location.Contains(location));
            }
        }

        var totalCount = await query.CountAsync();

        var teachers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (teachers, totalCount);
    }

    public async Task<Teacher> CreateAsync(Teacher teacher)
    {
        _dbContext.Teachers.Add(teacher);
        await _dbContext.SaveChangesAsync();
        return teacher;
    }

    public async Task UpdateAsync(Teacher updatedTeacher)
    {
        _dbContext.Teachers.Update(updatedTeacher);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Teacher teacher)
    {
        _dbContext.Teachers.Remove(teacher);
        await _dbContext.SaveChangesAsync();
    }
}
