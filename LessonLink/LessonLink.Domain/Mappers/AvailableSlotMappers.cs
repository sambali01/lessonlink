using LessonLink.BusinessLogic.DTOs.AvailableSlot;
using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Mappers;

public static class AvailableSlotMappers
{
    public static AvailableSlotResponse AvailableSlotToResponse(AvailableSlot slot)
    {
        return new AvailableSlotResponse
        {
            Id = slot.Id,
            TeacherId = slot.TeacherId,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            Bookings = [.. slot.Bookings.Select(BookingMappers.BookingToResponse)]
        };
    }
}
