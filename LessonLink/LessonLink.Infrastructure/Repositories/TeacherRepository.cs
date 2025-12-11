using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace LessonLink.Infrastructure.Repositories;

/// <summary>
/// Implementation of ITeacherRepository using Entity Framework Core.
/// Provides teacher profile management with advanced search and filtering capabilities.
/// Teachers are ranked by their booking count (popularity) in various queries.
/// </summary>
public class TeacherRepository(LessonLinkDbContext dbContext) : ITeacherRepository
{
    public async Task<IReadOnlyCollection<Teacher>> GetAllAsync()
    {
        return await dbContext.Teachers
            .Include(t => t.User)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<Teacher>> GetFeaturedAsync()
    {
        // Featured teachers are determined by their popularity (number of non-cancelled bookings)
        // Top 4 most popular teachers are returned for display on the home page
        return await dbContext.Teachers
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
        return await dbContext.Teachers
            .Include(t => t.User)
            .Include(t => t.TeacherSubjects)
                .ThenInclude(ts => ts.Subject)
            .FirstOrDefaultAsync(t => t.UserId == id);
    }

    public async Task<(List<Teacher>, int)> SearchAsync(
        string? searchText,
        List<string> subjects,
        int? minPrice,
        int? maxPrice,
        bool? acceptsOnline,
        bool? acceptsInPerson,
        string? location,
        int page,
        int pageSize)
    {
        var query = dbContext.Teachers
            .Include(t => t.User)
            .Include(t => t.TeacherSubjects)
                .ThenInclude(ts => ts.Subject)
            .AsQueryable();

        // Apply text search filter: search in teacher's name, nickname, and description
        if (!string.IsNullOrEmpty(searchText))
        {
            query = query.Where(t =>
                t.User.FirstName.Contains(searchText) ||
                t.User.SurName.Contains(searchText) ||
                t.User.NickName.Contains(searchText) ||
                (t.Description != null && t.Description.Contains(searchText)));
        }

        // Filter by subjects: teacher must teach at least one of the specified subjects
        if (subjects != null && subjects.Count != 0)
        {
            query = query.Where(t => t.TeacherSubjects
                .Any(ts => subjects.Contains(ts.Subject.Name)));
        }

        // Filter by price range
        if (minPrice.HasValue)
        {
            query = query.Where(t => t.HourlyRate >= minPrice);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(t => t.HourlyRate <= maxPrice);
        }

        // Filter by teaching method: online lessons
        if (acceptsOnline.HasValue && acceptsOnline.Value)
        {
            query = query.Where(t => t.AcceptsOnline == true);
        }

        // Filter by teaching method: in-person lessons and optional location
        if (acceptsInPerson.HasValue && acceptsInPerson.Value)
        {
            query = query.Where(t => t.AcceptsInPerson == true);

            if (location != null)
            {
                query = query.Where(t => t.Location != null && t.Location.Contains(location));
            }
        }

        var totalCount = await query.CountAsync();

        // Order by popularity (booking count) and apply pagination
        var teachers = await query
            .Select(t => new
            {
                Teacher = t,
                BookingCount = t.AvailableSlots
                    .SelectMany(slot => slot.Bookings)
                    .Count(booking => booking.Status != BookingStatus.Cancelled)
            })
            .OrderByDescending(x => x.BookingCount)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.Teacher)
            .ToListAsync();

        return (teachers, totalCount);
    }

    public Teacher CreateAsync(Teacher teacher)
    {
        dbContext.Teachers.Add(teacher);
        return teacher;
    }

    public void UpdateAsync(Teacher updatedTeacher)
    {
        dbContext.Teachers.Update(updatedTeacher);
    }

    public void DeleteAsync(Teacher teacher)
    {
        dbContext.Teachers.Remove(teacher);
    }
}
