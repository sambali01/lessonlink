using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories;

public interface ISubjectRepository
{
    Task<IReadOnlyCollection<Subject>> GetAllAsync();
    Task<Subject?> GetByIdAsync(int id);
}
