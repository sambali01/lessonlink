using LessonLink.BusinessLogic.DTOs.AvailableSlot;
using LessonLink.BusinessLogic.Helpers;
using LessonLink.BusinessLogic.Services;
using LessonLink.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LessonLink.WebApi.Controllers;

public class AvailableSlotsController : BaseApiController
{
    private readonly AvailableSlotService _availableSlotService;

    public AvailableSlotsController(AvailableSlotService availableSlotService)
    {
        _availableSlotService = availableSlotService;
    }

    [HttpGet("my-slots")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMySlots([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _availableSlotService.GetMySlotsPaginatedAsync(User.GetUserId(), page, pageSize);
        return HandleServiceResult(result);
    }

    [HttpGet("{id}/details")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetSlotDetails(int id)
    {
        var result = await _availableSlotService.GetSlotDetailsAsync(User.GetUserId(), id);
        return HandleServiceResult(result);
    }

    [HttpGet("teacher/{teacherId}")]
    public async Task<IActionResult> GetSlotsByTeacherId(string teacherId)
    {
        var result = await _availableSlotService.GetNotBookedSlotsByTeacherIdAsync(teacherId);
        return HandleServiceResult(result);
    }

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
}
