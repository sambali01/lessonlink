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
            StudentName = $"{booking.Student?.FirstName} {booking.Student?.SurName}",
            AvailableSlotId = booking.AvailableSlotId,
            SlotStartTime = booking.AvailableSlot?.StartTime ?? default,
            SlotEndTime = booking.AvailableSlot?.EndTime ?? default,
            TeacherId = booking.AvailableSlot?.TeacherId ?? string.Empty,
            TeacherName = booking.AvailableSlot?.Teacher?.User != null
                ? $"{booking.AvailableSlot.Teacher.User.SurName} {booking.AvailableSlot.Teacher.User.FirstName}"
                : string.Empty,
            Status = booking.Status,
            Notes = booking.Notes,
            CreatedAt = booking.CreatedAt
        };
    }

    public static BookingGetDto ToGetDto(this Booking booking)
    {
        return BookingToGetDto(booking);
    }
}