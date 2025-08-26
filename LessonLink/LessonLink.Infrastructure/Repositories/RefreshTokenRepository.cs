using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LessonLink.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly LessonLinkDbContext _dbContext;

    public RefreshTokenRepository(LessonLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken?> GetByHashedValueAsync(string hashedValue)
    {
        return await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Value == hashedValue);
    }

    public async Task CreateAsync(RefreshToken token)
    {
        _dbContext.RefreshTokens.Add(token);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(RefreshToken token)
    {
        _dbContext.RefreshTokens.Remove(token);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAllForUserAsync(string userId)
    {
        var tokens = await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .ToListAsync();

        _dbContext.RefreshTokens.RemoveRange(tokens);
        await _dbContext.SaveChangesAsync();
    }
}
