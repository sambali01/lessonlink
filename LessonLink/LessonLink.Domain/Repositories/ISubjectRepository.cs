using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories;

public interface ISubjectRepository
{
    Task<IReadOnlyCollection<Subject>> GetAllAsync();
    Task<Subject?> GetByIdAsync(int id);
    Task<Subject?> GetByNameAsync(string name);
    Task<Subject> CreateAsync(Subject subject);
}
