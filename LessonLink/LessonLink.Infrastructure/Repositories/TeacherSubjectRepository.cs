using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LessonLink.Infrastructure.Repositories;

/// <summary>
/// Implementation of ITeacherSubjectRepository using Entity Framework Core.
/// Manages the many-to-many relationship between teachers and subjects.
/// </summary>
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
        // Remove all subject associations for a teacher
        // This is used before adding new subjects when updating a teacher's profile
        var teacherSubjects = context.TeacherSubjects
            .Where(ts => ts.TeacherId == teacherId)
            .ToList();

        context.TeacherSubjects.RemoveRange(teacherSubjects);
    }
}
