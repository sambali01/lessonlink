using LessonLink.BusinessLogic.Common;
using LessonLink.BusinessLogic.DTOs.Booking;
using LessonLink.BusinessLogic.Services;
using LessonLink.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LessonLink.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly BookingService _bookingService;

    public BookingsController(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // GET: api/Bookings/my-bookings/student
    [HttpGet("my-bookings/student")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyBookingsAsStudent()
    {
        var result = await _bookingService.GetMyBookingsAsStudentAsync(User.GetUserId());
        return HandleServiceResult(result);
    }

    // GET: api/Bookings/my-bookings/teacher
    [HttpGet("my-bookings/teacher")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMyBookingsAsTeacher()
    {
        var result = await _bookingService.GetMyBookingsAsTeacherAsync(User.GetUserId());
        return HandleServiceResult(result);
    }

    // POST: api/Bookings
    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto createDto)
    {
        var result = await _bookingService.CreateBookingAsync(User.GetUserId(), createDto);
        return HandleServiceResult(result);
    }

    // PUT: api/Bookings/{id}/status
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] BookingUpdateStatusDto updateDto)
    {
        var result = await _bookingService.UpdateBookingStatusAsync(User.GetUserId(), id, updateDto);
        return HandleServiceResult(result);
    }

    // DELETE: api/Bookings/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var result = await _bookingService.CancelBookingAsync(User.GetUserId(), id);
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
                403 => Forbid(),
                404 => NotFound(result.Errors),
                409 => Conflict(result.Errors),
                _ => StatusCode(result.StatusCode, result.Errors)
            };
        }
    }
}