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
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        // Check login data
        var user = await userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return Unauthorized("There is no user with the email provided.");
        }

        var passwordCheck = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!passwordCheck.Succeeded)
        {
            return Unauthorized("Incorrect email or password.");
        }

        // Generate access and refresh token
        var userRoles = await userManager.GetRolesAsync(user);

        var accessTokenResult = tokenService.GenerateAccessToken(user, userRoles);
        var accessTokenValue = accessTokenResult.Data;
        if (accessTokenValue == null)
        {
            return HandleServiceResult(ServiceResult<AuthDto>.Failure(accessTokenResult.Errors, accessTokenResult.StatusCode));
        }

        var refreshTokenResult = await tokenService.GenerateRefreshTokenAsync(user);
        var refreshToken = refreshTokenResult.Data;
        if (refreshToken == null)
        {
            return HandleServiceResult(ServiceResult<AuthDto>.Failure(refreshTokenResult.Errors, refreshTokenResult.StatusCode));
        }

        // Put generated refresh token value in cookie
        var setCookieResult = tokenService.SetRefreshTokenResponseCookie(refreshToken);
        if (!setCookieResult.Succeeded)
        {
            return HandleServiceResult(ServiceResult<AuthDto>.Failure(setCookieResult.Errors, setCookieResult.StatusCode));
        }

        var authDto = new AuthDto
        {
            Id = user.Id,
            Token = accessTokenValue,
            Roles = userRoles
        };

        return Ok(authDto);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        // Get refresh token value from cookie
        var refreshTokenFromCookieResult = tokenService.GetRefreshTokenFromRequestCookie();
        var refreshTokenValueFromCookie = refreshTokenFromCookieResult.Data;
        if (refreshTokenValueFromCookie == null)
        {
            return HandleServiceResult(ServiceResult<AuthDto>.Failure(refreshTokenFromCookieResult.Errors, refreshTokenFromCookieResult.StatusCode));
        }

        // Hash the token value from cookie
        var hashedIncomingTokenBytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshTokenValueFromCookie));
        var hashedIncomingTokenString = Convert.ToBase64String(hashedIncomingTokenBytes);

        // Get refresh token from database
        var refreshTokenFromDbResult = await tokenService.GetRefreshTokenByHashedValueAsync(hashedIncomingTokenString);
        var refreshTokenFromDb = refreshTokenFromDbResult.Data;
        if (refreshTokenFromDb == null)
        {
            return HandleServiceResult(ServiceResult<AuthDto>.Failure(refreshTokenFromDbResult.Errors, refreshTokenFromDbResult.StatusCode));
        }

        // Get user who owns the refresh token
        var user = await userManager.FindByIdAsync(refreshTokenFromDb.UserId);
        if (user == null)
        {
            return NotFound("The user who owns the refresh token was not found.");
        }

        // Generate access and refresh token
        var userRoles = await userManager.GetRolesAsync(user);

        var accessTokenResult = tokenService.GenerateAccessToken(user, userRoles);
        var accessTokenValue = accessTokenResult.Data;
        if (accessTokenValue == null)
        {
            return HandleServiceResult(ServiceResult<AuthDto>.Failure(accessTokenResult.Errors, accessTokenResult.StatusCode));
        }

        var refreshTokenResult = await tokenService.GenerateRefreshTokenAsync(user);
        var refreshToken = refreshTokenResult.Data;
        if (refreshToken == null)
        {
            return HandleServiceResult(ServiceResult<AuthDto>.Failure(refreshTokenResult.Errors, refreshTokenResult.StatusCode));
        }

        // Put generated refresh token value in cookie
        var setCookieResult = tokenService.SetRefreshTokenResponseCookie(refreshToken);
        if (!setCookieResult.Succeeded)
        {
            return HandleServiceResult(ServiceResult<AuthDto>.Failure(setCookieResult.Errors, setCookieResult.StatusCode));
        }

        var authDto = new AuthDto
        {
            Id = user.Id,
            Token = accessTokenValue,
            Roles = userRoles
        };

        return Ok(authDto);
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
