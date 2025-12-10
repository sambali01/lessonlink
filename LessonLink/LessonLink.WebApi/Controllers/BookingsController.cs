using LessonLink.BusinessLogic.DTOs.Booking;
using LessonLink.BusinessLogic.Services;
using LessonLink.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LessonLink.WebApi.Controllers;

[Authorize]
public class BookingsController(BookingService bookingService) : BaseApiController
{
    [HttpGet("my-bookings/student")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyBookingsAsStudent()
    {
        var result = await bookingService.GetMyBookingsAsStudentAsync(User.GetUserId());
        return HandleServiceResult(result);
    }

    [HttpGet("my-bookings/teacher")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMyBookingsAsTeacher()
    {
        var result = await bookingService.GetMyBookingsAsTeacherAsync(User.GetUserId());
        return HandleServiceResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest createBookingRequest)
    {
        var result = await bookingService.CreateBookingAsync(User.GetUserId(), createBookingRequest);
        return HandleServiceResult(result);
    }

    [HttpPut("{bookingId}/status")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DecideBookingAcceptance(int bookingId, [FromBody] BookingAcceptanceRequest bookingAcceptanceRequest)
    {
        var result = await bookingService.UpdateBookingStatusAsync(User.GetUserId(), bookingId, bookingAcceptanceRequest);
        return HandleServiceResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var result = await bookingService.CancelBookingAsync(User.GetUserId(), id);
        return HandleServiceResult(result);
    }
}
