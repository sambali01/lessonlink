using LessonLink.BusinessLogic.Common;
using LessonLink.BusinessLogic.DTOs.User;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace LessonLink.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users); // 200
    }

    // GET: api/Users/abc123
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(); // 404
        }

        return Ok(user); // 200
    }

    // POST: api/Users/register
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _userService.CreateAsync(registerDto);
        return HandleServiceResult(result);
    }

    // PATCH: api/Users/5
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromForm] UserUpdateDto updateDto)
    {
        var result = await _userService.UpdateAsync(id, updateDto);
        return HandleServiceResult(result);
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var result = await _userService.DeleteAsync(id);
        return HandleServiceResult(result);
    }

    private async Task<bool> UserExists(string id)
    {
        return await _userService.GetByIdAsync(id) != null;
    }

    private IActionResult HandleServiceResult<T>(ServiceResult<T> result)
    {
        if (result.Succeeded)
        {
            return result.StatusCode switch
            {
                200 => Ok(result.Data),
                201 => CreatedAtAction(nameof(GetUserById), new { id = (result.Data as User)?.Id }, result.Data),
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
                409 => Conflict(result.Errors),
                _ => StatusCode(result.StatusCode, result.Errors)
            };
        }
    }
}
