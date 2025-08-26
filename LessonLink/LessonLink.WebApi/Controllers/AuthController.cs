using LessonLink.BusinessLogic.Common;
using LessonLink.BusinessLogic.DTOs.User;
using LessonLink.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace LessonLink.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var loginResult = await _authService.LoginAsync(loginDto);
        return HandleServiceResult(loginResult);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshResult = await _authService.RefreshAsync();
        return HandleServiceResult(refreshResult);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var logoutResult = await _authService.LogoutAsync();
        return HandleServiceResult(logoutResult);
    }

    private IActionResult HandleServiceResult<T>(ServiceResult<T> result)
    {
        if (result.Succeeded)
        {
            return result.StatusCode switch
            {
                200 => Ok(result.Data),
                204 => NoContent(),
                _ => StatusCode(result.StatusCode, result.Data)
            };
        }
        else
        {
            return result.StatusCode switch
            {
                400 => BadRequest(result.Errors),
                401 => Unauthorized(result.Errors),
                404 => NotFound(result.Errors),
                _ => StatusCode(result.StatusCode, result.Errors)
            };
        }
    }
}
