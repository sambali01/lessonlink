using LessonLink.BusinessLogic.Helpers;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LessonLink.BusinessLogic.Services;

public class TokenService(
    JwtSettings jwtSettings,
    IUnitOfWork unitOfWork,
    IHttpContextAccessor httpContextAccessor)
{
    private const string REFRESH_TOKEN_COOKIE = "refreshToken";

    public async Task<ServiceResult<RefreshToken>> GetRefreshTokenByHashedValueAsync(string hashedValue)
    {
        var refreshToken = await unitOfWork.RefreshTokenRepository.GetByHashedValueAsync(hashedValue);
        if (refreshToken == null)
            return ServiceResult<RefreshToken>.Failure("Érvénytelen frissítési token.", 401);
        if (refreshToken.ExpiresAt < DateTime.UtcNow)
            return ServiceResult<RefreshToken>.Failure("A megadott frissítési token lejárt.", 401);

        return ServiceResult<RefreshToken>.Success(refreshToken);
    }

    public async Task<ServiceResult<object>> DeleteRefreshTokenAsync(RefreshToken refreshToken)
    {
        unitOfWork.RefreshTokenRepository.DeleteAsync(refreshToken);
        if (await unitOfWork.CompleteAsync())
        {
            return ServiceResult<object>.Success(null);
        }
        return ServiceResult<object>.Failure("Hiba történt a frissítési token törlése során.", 500);
    }

    public ServiceResult<string> GenerateAccessToken(User user, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey));
        var claims = new List<Claim>
            {
                new (ClaimTypes.Email, user.Email!),
                new(ClaimTypes.NameIdentifier, user.Id)
            };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var expiration = DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwtSettings.AccessTokenValidityInMinutes));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            SigningCredentials = credentials,
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenValue = tokenHandler.WriteToken(token);

        return ServiceResult<string>.Success(tokenValue);
    }

    public async Task<ServiceResult<RefreshToken>> GenerateRefreshTokenAsync(User user)
    {
        // Generate random string
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var tokenValue = Convert.ToBase64String(randomNumber);

        // Hash refresh token value
        var hashedTokenValueBytes = SHA256.HashData(Encoding.UTF8.GetBytes(tokenValue));
        var hashedTokenValue = Convert.ToBase64String(hashedTokenValueBytes);

        var refreshToken = new RefreshToken
        {
            Value = hashedTokenValue,
            ExpiresAt = DateTime.UtcNow.AddMinutes(jwtSettings.RefreshTokenValidityInMinutes),
            UserId = user.Id,
            User = user
        };

        // Save the token with the hashed value in the database
        await unitOfWork.RefreshTokenRepository.DeleteAllForUserAsync(user.Id);
        unitOfWork.RefreshTokenRepository.CreateAsync(refreshToken);

        if (await unitOfWork.CompleteAsync())
        {
            // Give the token with unhashed value
            refreshToken.Value = tokenValue;
            return ServiceResult<RefreshToken>.Success(refreshToken);
        }

        return ServiceResult<RefreshToken>.Failure("Hiba történt a frissítési token létrehozása során.", 500);
    }

    public ServiceResult<object> SetRefreshTokenResponseCookie(RefreshToken refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = refreshToken.ExpiresAt,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };
        httpContextAccessor.HttpContext.Response.Cookies.Append(
            REFRESH_TOKEN_COOKIE,
            refreshToken.Value,
            cookieOptions
        );

        return ServiceResult<object>.Success(null);
    }

    public ServiceResult<string> GetRefreshTokenFromRequestCookie()
    {
        var refreshTokenValue = httpContextAccessor.HttpContext.Request.Cookies[REFRESH_TOKEN_COOKIE];
        if (refreshTokenValue == null)
        {
            return ServiceResult<string>.Failure("Nincs frissítési token megadva.", 401);
        }
        return ServiceResult<string>.Success(refreshTokenValue);
    }

    public ServiceResult<object> DeleteRefreshTokenFromResponseCookie()
    {
        httpContextAccessor.HttpContext.Response.Cookies.Delete(REFRESH_TOKEN_COOKIE);
        return ServiceResult<object>.Success(null);
    }
}
