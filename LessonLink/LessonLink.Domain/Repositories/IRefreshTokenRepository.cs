using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByHashedValueAsync(string token);
    Task CreateAsync(RefreshToken token);
    Task DeleteAsync(RefreshToken token);
    Task DeleteAllForUserAsync(string userId);
}
