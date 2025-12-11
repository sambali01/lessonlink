using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LessonLink.Infrastructure.Repositories;

public class TeacherSubjectRepository(LessonLinkDbContext context) : ITeacherSubjectRepository
{
    public async Task<IReadOnlyCollection<TeacherSubject>> GetByTeacherIdAsync(string teacherId)
    {
        return await context.TeacherSubjects
            .Where(ts => ts.TeacherId == teacherId)
            .ToListAsync();
    }

    public TeacherSubject CreateAsync(TeacherSubject teacherSubject)
    {
        context.TeacherSubjects.Add(teacherSubject);
        return teacherSubject;
    }

    public void DeleteByTeacherIdAsync(string teacherId)
    {
        var teacherSubjects = context.TeacherSubjects
            .Where(ts => ts.TeacherId == teacherId)
            .ToList();

        context.TeacherSubjects.RemoveRange(teacherSubjects);
    }
}
