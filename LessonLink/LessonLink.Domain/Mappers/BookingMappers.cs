using LessonLink.BusinessLogic.DTOs.Booking;
using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Mappers;

public static class BookingMappers
{
    public static BookingResponse BookingToResponse(Booking booking)
    {
        return new BookingResponse
        {
            Id = booking.Id,
            StudentId = booking.StudentId,
            StudentName = $"{booking.Student.SurName} {booking.Student.FirstName}",
            AvailableSlotId = booking.AvailableSlotId,
            SlotStartTime = booking.AvailableSlot.StartTime,
            SlotEndTime = booking.AvailableSlot.EndTime,
            TeacherId = booking.AvailableSlot.TeacherId,
            TeacherName = $"{booking.AvailableSlot.Teacher.User.SurName} {booking.AvailableSlot.Teacher.User.FirstName}",
            Status = booking.Status,
            CreatedAt = booking.CreatedAt
        };
    }
}
