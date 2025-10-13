using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class TeacherSubjectRepository : ITeacherSubjectRepository
{
    private readonly LessonLinkDbContext _dbContext;

    public TeacherSubjectRepository(LessonLinkDbContext context)
    {
        _dbContext = context;
    }

    public async Task<IReadOnlyCollection<TeacherSubject>> GetByTeacherIdAsync(string teacherId)
    {
        return await _dbContext.TeacherSubjects
            .Where(ts => ts.TeacherId == teacherId)
            .ToListAsync();
    }

    public async Task<TeacherSubject> CreateAsync(TeacherSubject teacherSubject)
    {
        _dbContext.TeacherSubjects.Add(teacherSubject);
        await _dbContext.SaveChangesAsync();
        return teacherSubject;
    }

    public async Task DeleteByTeacherIdAsync(string teacherId)
    {
        var teacherSubjects = _dbContext.TeacherSubjects
            .Where(ts => ts.TeacherId == teacherId)
            .ToList();

        _dbContext.TeacherSubjects.RemoveRange(teacherSubjects);
        await _dbContext.SaveChangesAsync();
    }
}
