using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories
{
    public interface ITeacherSubjectRepository
    {
        Task<IReadOnlyCollection<TeacherSubject>> GetByTeacherIdAsync(string teacherId);
        TeacherSubject CreateAsync(TeacherSubject teacherSubject);
        void DeleteByTeacherIdAsync(string teacherId);
    }
}
