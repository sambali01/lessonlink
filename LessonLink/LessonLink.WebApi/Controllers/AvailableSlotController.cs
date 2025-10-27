using LessonLink.BusinessLogic.Common;
using LessonLink.BusinessLogic.DTOs.AvailableSlot;
using LessonLink.BusinessLogic.Services;
using LessonLink.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LessonLink.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AvailableSlotsController : ControllerBase
{
    private readonly AvailableSlotService _availableSlotService;

    public AvailableSlotsController(AvailableSlotService availableSlotService)
    {
        _availableSlotService = availableSlotService;
    }

    // GET: api/AvailableSlots/my-slots
    [HttpGet("my-slots")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMySlots()
    {
        var result = await _availableSlotService.GetMySlotsAsync(User.GetUserId());
        return HandleServiceResult(result);
    }

    // GET: api/AvailableSlots/teacher/{teacherId}
    [HttpGet("teacher/{teacherId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSlotsByTeacherId(string teacherId)
    {
        var result = await _availableSlotService.GetNotBookedSlotsByTeacherIdAsync(teacherId);
        return HandleServiceResult(result);
    }

    // POST: api/AvailableSlots
    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateSlot([FromBody] AvailableSlotCreateDto createDto)
    {
        var result = await _availableSlotService.CreateSlotAsync(User.GetUserId(), createDto);
        return HandleServiceResult(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DeleteSlot(int id)
    {
        var result = await _availableSlotService.DeleteSlotAsync(User.GetUserId(), id);
        return HandleServiceResult(result);
    }

    private IActionResult HandleServiceResult<T>(ServiceResult<T> result)
    {
        if (result.Succeeded)
        {
            return result.StatusCode switch
            {
                200 => Ok(result.Data),
                201 => Created("", result.Data),
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
