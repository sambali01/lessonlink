using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories;

public interface ITeacherRepository
{
    Task<IReadOnlyCollection<Teacher>> GetAllAsync();
    Task<IReadOnlyCollection<Teacher>> GetFeaturedAsync();
    Task<Teacher?> GetByIdAsync(string id);
    Task<(List<Teacher>, int)> SearchAsync(
        string? searchText,
        List<string> subjects,
        int? minPrice,
        int? maxPrice,
        bool? acceptsOnline,
        bool? acceptsInPerson,
        string? location,
        int page,
        int pageSize);
    Teacher CreateAsync(Teacher teacher);
    void UpdateAsync(Teacher updatedTeacher);
    void DeleteAsync(Teacher teacher);
}
