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
            .OrderByDescending(t => t.Rating)
            .Take(4)
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
        string? searchQuery,
        List<string>? subjects,
        int? minPrice,
        int? maxPrice,
        double? minRating,
        bool? acceptsOnline,
        bool? acceptsInPerson,
        int page,
        int pageSize)
    {
        var query = _dbContext.Teachers
            .Include(t => t.User)
            .Include(t => t.TeacherSubjects)
                .ThenInclude(ts => ts.Subject)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(t =>
                t.User.FirstName.Contains(searchQuery) ||
                t.User.SurName.Contains(searchQuery) ||
                t.User.NickName.Contains(searchQuery) ||
                (t.Description != null && t.Description.Contains(searchQuery)));
        }

        if (subjects != null && subjects.Count != 0)
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

        if (minRating.HasValue)
        {
            query = query.Where(t => t.Rating >= minRating);
        }

        if (acceptsOnline.HasValue)
        {
            query = query.Where(t => t.AcceptsOnline == acceptsOnline);
        }

        if (acceptsInPerson.HasValue)
        {
            query = query.Where(t => t.AcceptsInPerson == acceptsInPerson);
        }

        var totalCount = await query.CountAsync();

        var teachers = await query
            .OrderByDescending(t => t.Rating)
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
