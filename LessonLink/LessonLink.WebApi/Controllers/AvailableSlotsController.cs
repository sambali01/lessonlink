using LessonLink.BusinessLogic.DTOs.AvailableSlot;
using LessonLink.BusinessLogic.Services;
using LessonLink.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LessonLink.WebApi.Controllers;

public class AvailableSlotsController(AvailableSlotService availableSlotService) : BaseApiController
{
    [HttpGet("{id}/details")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetSlotDetails(int id)
    {
        var result = await availableSlotService.GetSlotDetailsAsync(User.GetUserId(), id);
        return HandleServiceResult(result);
    }

    [HttpGet("my-slots/current")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMyCurrentSlots([FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = await availableSlotService.GetCurrentSlotsPaginatedAsync(User.GetUserId(), page, pageSize);
        return HandleServiceResult(result);
    }

    [HttpGet("my-slots/past")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMyPastSlots([FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = await availableSlotService.GetPastSlotsPaginatedAsync(User.GetUserId(), page, pageSize);
        return HandleServiceResult(result);
    }

    [HttpGet("teacher/{teacherId}")]
    public async Task<IActionResult> GetCurrentSlotsByTeacherId(string teacherId, [FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = await availableSlotService.GetCurrentNotBookedSlotsPaginatedAsync(teacherId, page, pageSize);
        return HandleServiceResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateSlot([FromBody] CreateAvailableSlotRequest createRequest)
    {
        var result = await availableSlotService.CreateSlotAsync(User.GetUserId(), createRequest);
        return HandleServiceResult(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UpdateSlot(int id, [FromBody] UpdateAvailableSlotRequest updateRequest)
    {
        var result = await availableSlotService.UpdateSlotAsync(User.GetUserId(), id, updateRequest);
        return HandleServiceResult(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DeleteSlot(int id)
    {
        var result = await availableSlotService.DeleteSlotAsync(User.GetUserId(), id);
        return HandleServiceResult(result);
    }
}
