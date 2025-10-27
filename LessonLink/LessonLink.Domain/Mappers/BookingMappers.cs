using LessonLink.BusinessLogic.DTOs.Booking;
using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Mappers;

public static class BookingMappers
{
    public static Booking CreateDtoToBooking(BookingCreateDto createDto, string studentId)
    {
        return new Booking
        {
            StudentId = studentId,
            AvailableSlotId = createDto.AvailableSlotId,
            Status = BookingStatus.Pending,
            Notes = createDto.Notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static BookingGetDto BookingToGetDto(Booking booking)
    {
        return new BookingGetDto
        {
            Id = booking.Id,
            StudentId = booking.StudentId,
            StudentName = $"{booking.Student.FirstName} {booking.Student.SurName}",
            AvailableSlotId = booking.AvailableSlotId,
            SlotStartTime = booking.AvailableSlot.StartTime,
            SlotEndTime = booking.AvailableSlot.EndTime,
            TeacherId = booking.AvailableSlot.TeacherId,
            TeacherName = $"{booking.AvailableSlot.Teacher.User.FirstName} {booking.AvailableSlot.Teacher.User.SurName}",
            Status = booking.Status,
            Notes = booking.Notes,
            CreatedAt = booking.CreatedAt
        };
    }
}