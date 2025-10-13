using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories
{
    public interface ITeacherSubjectRepository
    {
        Task<IReadOnlyCollection<TeacherSubject>> GetByTeacherIdAsync(string teacherId);
        Task<TeacherSubject> CreateAsync(TeacherSubject teacherSubject);
        Task DeleteByTeacherIdAsync(string teacherId);
    }
}
