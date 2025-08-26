using System.Security.Cryptography;
using System.Text;
using LessonLink.BusinessLogic.Common;
using LessonLink.BusinessLogic.DTOs.User;
using LessonLink.BusinessLogic.Repositories;

namespace LessonLink.BusinessLogic.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenService _tokenService;


    public AuthService(
        IUserRepository userRepository,
        JwtTokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<ServiceResult<AuthDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            // Check login data
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
                return ServiceResult<AuthDto>.Failure("There is no user with the email provided.", 401);

            var passwordCheck = await _userRepository.CheckPasswordSignInAsync(user, loginDto.Password);
            if (!passwordCheck.Succeeded)
                return ServiceResult<AuthDto>.Failure("Incorrect email or password.", 401);

            // Generate access and refresh token
            var userRoles = await _userRepository.GetUserRolesAsync(user);

            var accessTokenResult = _tokenService.GenerateAccessToken(user, userRoles);
            var accessTokenValue = accessTokenResult.Data;
            if (accessTokenValue == null)
                return ServiceResult<AuthDto>.Failure(accessTokenResult.Errors, accessTokenResult.StatusCode);

            var refreshTokenResult = await _tokenService.GenerateRefreshTokenAsync(user);
            var refreshToken = refreshTokenResult.Data;
            if (refreshToken == null)
                return ServiceResult<AuthDto>.Failure(refreshTokenResult.Errors, refreshTokenResult.StatusCode);

            // Put generated refresh token value in cookie
            var setCookieResult = _tokenService.SetRefreshTokenResponseCookie(refreshToken);
            if (!setCookieResult.Succeeded)
                return ServiceResult<AuthDto>.Failure(setCookieResult.Errors, setCookieResult.StatusCode);

            var authDto = new AuthDto
            {
                Id = user.Id,
                Token = accessTokenValue,
                Roles = userRoles
            };

            return ServiceResult<AuthDto>.Success(authDto);
        }
        catch (Exception ex)
        {
            return ServiceResult<AuthDto>.Failure("Login failed:\n" + ex.Message, 500);
        }
    }

    public async Task<ServiceResult<AuthDto>> RefreshAsync()
    {
        try
        {
            // Get refresh token value from cookie
            var refreshTokenFromCookieResult = _tokenService.GetRefreshTokenFromRequestCookie();
            var refreshTokenValueFromCookie = refreshTokenFromCookieResult.Data;
            if (refreshTokenValueFromCookie == null)
                return ServiceResult<AuthDto>.Failure(refreshTokenFromCookieResult.Errors, refreshTokenFromCookieResult.StatusCode);

            // Hash the token value from cookie
            var hashedIncomingTokenBytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshTokenValueFromCookie));
            var hashedIncomingTokenString = Convert.ToBase64String(hashedIncomingTokenBytes);

            // Get refresh token from database
            var refreshTokenFromDbResult = await _tokenService.GetRefreshTokenByHashedValueAsync(hashedIncomingTokenString);
            var refreshTokenFromDb = refreshTokenFromDbResult.Data;
            if (refreshTokenFromDb == null)
            {
                return ServiceResult<AuthDto>.Failure(refreshTokenFromDbResult.Errors, refreshTokenFromDbResult.StatusCode);
            }

            // Get user who owns the refresh token
            var user = await _userRepository.GetByIdAsync(refreshTokenFromDb.UserId);
            if (user == null)
                return ServiceResult<AuthDto>.Failure("The user who owns the refresh token was not found.", 404);

            // Generate access and refresh token
            var userRoles = await _userRepository.GetUserRolesAsync(user);

            var accessTokenResult = _tokenService.GenerateAccessToken(user, userRoles);
            var accessTokenValue = accessTokenResult.Data;
            if (accessTokenValue == null)
                return ServiceResult<AuthDto>.Failure(accessTokenResult.Errors, accessTokenResult.StatusCode);

            var refreshTokenResult = await _tokenService.GenerateRefreshTokenAsync(user);
            var refreshToken = refreshTokenResult.Data;
            if (refreshToken == null)
                return ServiceResult<AuthDto>.Failure(refreshTokenResult.Errors, refreshTokenResult.StatusCode);

            // Put generated refresh token value in cookie
            var setCookieResult = _tokenService.SetRefreshTokenResponseCookie(refreshToken);
            if (!setCookieResult.Succeeded)
                return ServiceResult<AuthDto>.Failure(setCookieResult.Errors, setCookieResult.StatusCode);

            var authDto = new AuthDto
            {
                Id = user.Id,
                Token = accessTokenValue,
                Roles = userRoles
            };

            return ServiceResult<AuthDto>.Success(authDto);
        }
        catch (Exception ex)
        {
            return ServiceResult<AuthDto>.Failure("Refreshing JWT failed:\n" + ex.Message, 500);
        }
    }

    public async Task<ServiceResult<object>> LogoutAsync()
    {
        try
        {
            // Get refresh token value from cookie
            var refreshTokenFromCookieResult = _tokenService.GetRefreshTokenFromRequestCookie();
            var refreshTokenValueFromCookie = refreshTokenFromCookieResult.Data;
            if (refreshTokenValueFromCookie == null)
                return ServiceResult<object>.Failure(refreshTokenFromCookieResult.Errors, refreshTokenFromCookieResult.StatusCode);

            // Hash the token value from cookie
            var hashedIncomingTokenBytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshTokenValueFromCookie));
            var hashedIncomingTokenString = Convert.ToBase64String(hashedIncomingTokenBytes);

            // Get refresh token from database
            var refreshTokenFromDbResult = await _tokenService.GetRefreshTokenByHashedValueAsync(hashedIncomingTokenString);
            var refreshTokenFromDb = refreshTokenFromDbResult.Data;
            if (refreshTokenFromDb == null)
                return ServiceResult<object>.Failure(refreshTokenFromDbResult.Errors, refreshTokenFromDbResult.StatusCode);

            // Delete refresh token from database
            await _tokenService.DeleteRefreshTokenAsync(refreshTokenFromDb);

            // Delete refresh token from cookie
            _tokenService.DeleteRefreshTokenFromResponseCookie();

            return ServiceResult<object>.Success(new object(), 204);
        }
        catch (Exception ex)
        {
            return ServiceResult<object>.Failure("Logout failed:\n" + ex.Message, 500);
        }
    }
}
