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

public class JwtTokenService
{
    private const string REFRESH_TOKEN_COOKIE = "refreshToken";

    private readonly JwtSettings _jwtSettings;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtTokenService(
        JwtSettings jwtSettings,
        IRefreshTokenRepository refreshTokenRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _jwtSettings = jwtSettings;
        _refreshTokenRepository = refreshTokenRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ServiceResult<RefreshToken>> GetRefreshTokenByHashedValueAsync(string hashedValue)
    {
        try
        {
            var refreshToken = await _refreshTokenRepository.GetByHashedValueAsync(hashedValue);
            if (refreshToken == null)
                return ServiceResult<RefreshToken>.Failure("Invalid refresh token.", 401);
            if (refreshToken.ExpiresAt < DateTime.UtcNow)
                return ServiceResult<RefreshToken>.Failure("The provided refresh token is expired.", 401);

            return ServiceResult<RefreshToken>.Success(refreshToken);
        }
        catch (Exception ex)
        {
            return ServiceResult<RefreshToken>.Failure("Getting stored refresh token failed:\n" + ex.Message, 500);
        }
    }

    public async Task<ServiceResult<object>> DeleteRefreshTokenAsync(RefreshToken refreshToken)
    {
        try
        {
            await _refreshTokenRepository.DeleteAsync(refreshToken);
            return ServiceResult<object>.Success(new object());
        }
        catch (Exception ex)
        {
            return ServiceResult<object>.Failure("Deleting stored refresh token failed:\n" + ex.Message, 500);
        }
    }

    public ServiceResult<string> GenerateAccessToken(User user, IList<string> roles)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey));
            var claims = new List<Claim>
                {
                    new (ClaimTypes.Email, user.Email!),
                    new(ClaimTypes.NameIdentifier, user.Id)
                };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var expiration = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_jwtSettings.AccessTokenValidityInMinutes));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = credentials,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenValue = tokenHandler.WriteToken(token);

            return ServiceResult<string>.Success(tokenValue);
        }
        catch (Exception ex)
        {
            return ServiceResult<string>.Failure("Generating access token failed:\n" + ex.Message, 500);
        }
    }

    public async Task<ServiceResult<RefreshToken>> GenerateRefreshTokenAsync(User user)
    {
        try
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
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenValidityInMinutes),
                UserId = user.Id,
                User = user
            };

            // Save the token with the hashed value in the databasee
            await _refreshTokenRepository.DeleteAllForUserAsync(user.Id);
            await _refreshTokenRepository.CreateAsync(refreshToken);

            // Send the token with unhashed value to the client
            refreshToken.Value = tokenValue;
            return ServiceResult<RefreshToken>.Success(refreshToken);
        }
        catch (Exception ex)
        {
            return ServiceResult<RefreshToken>.Failure("Generating refresh token failed:\n" + ex.Message, 500);
        }
    }

    public ServiceResult<object> SetRefreshTokenResponseCookie(RefreshToken refreshToken)
    {
        try
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.ExpiresAt,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                REFRESH_TOKEN_COOKIE,
                refreshToken.Value,
                cookieOptions
            );

            return ServiceResult<object>.Success(new object());
        }
        catch (Exception ex)
        {
            return ServiceResult<object>.Failure("Setting refresh token cookie failed:\n" + ex.Message, 500);
        }
    }

    public ServiceResult<string> GetRefreshTokenFromRequestCookie()
    {
        try
        {
            var refreshTokenValue = _httpContextAccessor.HttpContext.Request.Cookies[REFRESH_TOKEN_COOKIE];
            if (refreshTokenValue == null)
            {
                return ServiceResult<string>.Failure("No refresh token provided.", 401);
            }
            return ServiceResult<string>.Success(refreshTokenValue);
        }
        catch (Exception ex)
        {
            return ServiceResult<string>.Failure("Getting refresh token from cookie failed:\n" + ex.Message, 500);
        }
    }

    public ServiceResult<object> DeleteRefreshTokenFromResponseCookie()
    {
        try
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(REFRESH_TOKEN_COOKIE);
            return ServiceResult<object>.Success(new object());
        }
        catch (Exception ex)
        {
            return ServiceResult<object>.Failure("Deleting refresh token from cookie failed:\n" + ex.Message, 500);
        }
    }
}
