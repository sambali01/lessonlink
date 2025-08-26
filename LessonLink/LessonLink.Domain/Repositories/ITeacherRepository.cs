using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories;

public interface ITeacherRepository
{
    Task<IReadOnlyCollection<Teacher>> GetAllAsync();
    Task<IReadOnlyCollection<Teacher>> GetFeaturedAsync();
    Task<Teacher?> GetByIdAsync(string id);
    Task<(List<Teacher>, int)> SearchAsync(
        string? searchQuery,
        List<string>? subjects,
        int? minPrice,
        int? maxPrice,
        double? minRating,
        bool? acceptsOnline,
        bool? acceptsInPerson,
        int page,
        int pageSize);
    Task<Teacher> CreateAsync(Teacher teacher);
    Task UpdateAsync(string id, Teacher updatedTeacher);
    Task DeleteAsync(Teacher teacher);
}
