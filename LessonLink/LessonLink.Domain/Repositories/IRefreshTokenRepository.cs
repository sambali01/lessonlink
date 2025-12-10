using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByHashedValueAsync(string token);
    void CreateAsync(RefreshToken token);
    void DeleteAsync(RefreshToken token);
    Task DeleteAllForUserAsync(string userId);
}
