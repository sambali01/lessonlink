using LessonLink.BusinessLogic.DTOs.User;
using LessonLink.BusinessLogic.Helpers;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace LessonLink.WebApi.Controllers;

public class AuthController(
    TokenService tokenService,
    UserManager<User> userManager,
    SignInManager<User> signInManager
) : BaseApiController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        // Check login data
        var user = await userManager.FindByEmailAsync(loginRequest.Email);
        if (user == null)
        {
            return Unauthorized("Nincs felhasználó a megadott email címmel.");
        }

        var passwordCheck = await signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
        if (!passwordCheck.Succeeded)
        {
            return Unauthorized("Helytelen email cím vagy jelszó.");
        }

        // Generate access and refresh token
        var userRoles = await userManager.GetRolesAsync(user);

        var accessTokenResult = tokenService.GenerateAccessToken(user, userRoles);
        var accessTokenValue = accessTokenResult.Data;
        if (accessTokenValue == null)
        {
            return HandleServiceResult(ServiceResult<UserAuthResponse>.Failure(accessTokenResult.Errors, accessTokenResult.StatusCode));
        }

        var refreshTokenResult = await tokenService.GenerateRefreshTokenAsync(user);
        var refreshToken = refreshTokenResult.Data;
        if (refreshToken == null)
        {
            return HandleServiceResult(ServiceResult<UserAuthResponse>.Failure(refreshTokenResult.Errors, refreshTokenResult.StatusCode));
        }

        // Put generated refresh token value in cookie
        var setCookieResult = tokenService.SetRefreshTokenResponseCookie(refreshToken);
        if (!setCookieResult.Succeeded)
        {
            return HandleServiceResult(ServiceResult<UserAuthResponse>.Failure(setCookieResult.Errors, setCookieResult.StatusCode));
        }

        var userAuthResponse = new UserAuthResponse
        {
            Id = user.Id,
            Token = accessTokenValue,
            Roles = userRoles
        };

        return Ok(userAuthResponse);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        // Get refresh token value from cookie
        var refreshTokenFromCookieResult = tokenService.GetRefreshTokenFromRequestCookie();
        var refreshTokenValueFromCookie = refreshTokenFromCookieResult.Data;
        if (refreshTokenValueFromCookie == null)
        {
            return HandleServiceResult(ServiceResult<UserAuthResponse>.Failure(refreshTokenFromCookieResult.Errors, refreshTokenFromCookieResult.StatusCode));
        }

        // Hash the token value from cookie
        var hashedIncomingTokenBytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshTokenValueFromCookie));
        var hashedIncomingTokenString = Convert.ToBase64String(hashedIncomingTokenBytes);

        // Get refresh token from database
        var refreshTokenFromDbResult = await tokenService.GetRefreshTokenByHashedValueAsync(hashedIncomingTokenString);
        var refreshTokenFromDb = refreshTokenFromDbResult.Data;
        if (refreshTokenFromDb == null)
        {
            return HandleServiceResult(ServiceResult<UserAuthResponse>.Failure(refreshTokenFromDbResult.Errors, refreshTokenFromDbResult.StatusCode));
        }

        // Get user who owns the refresh token
        var user = await userManager.FindByIdAsync(refreshTokenFromDb.UserId);
        if (user == null)
        {
            return NotFound("A refresh tokenhez tartozó felhasználó nem található.");
        }

        // Generate access and refresh token
        var userRoles = await userManager.GetRolesAsync(user);

        var accessTokenResult = tokenService.GenerateAccessToken(user, userRoles);
        var accessTokenValue = accessTokenResult.Data;
        if (accessTokenValue == null)
        {
            return HandleServiceResult(ServiceResult<UserAuthResponse>.Failure(accessTokenResult.Errors, accessTokenResult.StatusCode));
        }

        var refreshTokenResult = await tokenService.GenerateRefreshTokenAsync(user);
        var refreshToken = refreshTokenResult.Data;
        if (refreshToken == null)
        {
            return HandleServiceResult(ServiceResult<UserAuthResponse>.Failure(refreshTokenResult.Errors, refreshTokenResult.StatusCode));
        }

        // Put generated refresh token value in cookie
        var setCookieResult = tokenService.SetRefreshTokenResponseCookie(refreshToken);
        if (!setCookieResult.Succeeded)
        {
            return HandleServiceResult(ServiceResult<UserAuthResponse>.Failure(setCookieResult.Errors, setCookieResult.StatusCode));
        }

        var userAuthResponse = new UserAuthResponse
        {
            Id = user.Id,
            Token = accessTokenValue,
            Roles = userRoles
        };

        return Ok(userAuthResponse);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        // Get refresh token value from cookie
        var refreshTokenFromCookieResult = tokenService.GetRefreshTokenFromRequestCookie();
        var refreshTokenValueFromCookie = refreshTokenFromCookieResult.Data;
        if (refreshTokenValueFromCookie == null)
        {
            return HandleServiceResult(ServiceResult<object>.Failure(refreshTokenFromCookieResult.Errors, refreshTokenFromCookieResult.StatusCode));
        }

        // Hash the token value from cookie
        var hashedIncomingTokenBytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshTokenValueFromCookie));
        var hashedIncomingTokenString = Convert.ToBase64String(hashedIncomingTokenBytes);

        // Get refresh token from database
        var refreshTokenFromDbResult = await tokenService.GetRefreshTokenByHashedValueAsync(hashedIncomingTokenString);
        var refreshTokenFromDb = refreshTokenFromDbResult.Data;
        if (refreshTokenFromDb == null)
        {
            return HandleServiceResult(ServiceResult<object>.Failure(refreshTokenFromDbResult.Errors, refreshTokenFromDbResult.StatusCode));
        }

        // Delete refresh token from database
        var deleteRefreshTokenResult = await tokenService.DeleteRefreshTokenAsync(refreshTokenFromDb);
        if (!deleteRefreshTokenResult.Succeeded)
        {
            return HandleServiceResult(ServiceResult<object>.Failure(deleteRefreshTokenResult.Errors, deleteRefreshTokenResult.StatusCode));
        }

        // Delete refresh token from cookie
        var deleteCookieResult = tokenService.DeleteRefreshTokenFromResponseCookie();
        if (!deleteCookieResult.Succeeded)
        {
            return HandleServiceResult(ServiceResult<object>.Failure(deleteCookieResult.Errors, deleteCookieResult.StatusCode));
        }

        return NoContent();
    }
}
