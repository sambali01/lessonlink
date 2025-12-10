using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LessonLink.Infrastructure.Repositories;

public class RefreshTokenRepository(LessonLinkDbContext dbContext) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByHashedValueAsync(string hashedValue)
    {
        return await dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Value == hashedValue);
    }

    public void CreateAsync(RefreshToken token)
    {
        dbContext.RefreshTokens.Add(token);
    }

    public void DeleteAsync(RefreshToken token)
    {
        dbContext.RefreshTokens.Remove(token);
    }

    public async Task DeleteAllForUserAsync(string userId)
    {
        var tokens = await dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .ToListAsync();

        dbContext.RefreshTokens.RemoveRange(tokens);
    }
}
