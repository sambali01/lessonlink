using LessonLink.BusinessLogic.DTOs.Booking;
using LessonLink.BusinessLogic.Helpers;
using LessonLink.BusinessLogic.Mappers;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;

namespace LessonLink.BusinessLogic.Services;

public class BookingService(IUnitOfWork unitOfWork)
{
    public async Task<ServiceResult<BookingResponse[]>> GetMyBookingsAsStudentAsync(string studentId)
    {
        if (string.IsNullOrEmpty(studentId))
        {
            return ServiceResult<BookingResponse[]>.Failure("A diák nem található.", 401);
        }

        var bookings = await unitOfWork.BookingRepository.GetByStudentIdAsync(studentId);

        var bookingDtos = bookings
            .Select(BookingMappers.BookingToResponse)
            .ToArray();

        return ServiceResult<BookingResponse[]>.Success(bookingDtos);
    }

    public async Task<ServiceResult<BookingResponse[]>> GetMyBookingsAsTeacherAsync(string teacherId)
    {
        if (string.IsNullOrEmpty(teacherId))
        {
            return ServiceResult<BookingResponse[]>.Failure("A tanár nem található.", 401);
        }

        var bookings = await unitOfWork.BookingRepository.GetByTeacherIdAsync(teacherId);

        var bookingDtos = bookings
            .Select(BookingMappers.BookingToResponse)
            .ToArray();

        return ServiceResult<BookingResponse[]>.Success(bookingDtos);
    }

    public async Task<ServiceResult<Booking>> CreateBookingAsync(string studentId, CreateBookingRequest createRequest)
    {
        if (string.IsNullOrEmpty(studentId))
        {
            return ServiceResult<Booking>.Failure("Nem vagy bejelentkezve.", 401);
        }

        // Check if available slot exists and is available
        var availableSlot = await unitOfWork.AvailableSlotRepository.GetByIdAsync(createRequest.AvailableSlotId);
        if (availableSlot == null)
        {
            return ServiceResult<Booking>.Failure("Az időpont nem található.", 404);
        }

        // Check if the student already has an overlapping active booking
        var hasOverlappingBooking = await unitOfWork.BookingRepository.HasOverlappingActiveBookingForStudentAsync(
            studentId, availableSlot.StartTime, availableSlot.EndTime);
        if (hasOverlappingBooking)
        {
            return ServiceResult<Booking>.Failure("Már van foglalásod ebben az időszakban.", 400);
        }

        // If the student is also a teacher, check if they have an overlapping slot
        var teacher = await unitOfWork.TeacherRepository.GetByIdAsync(studentId);
        if (teacher != null)
        {
            var hasOverlappingSlot = await unitOfWork.AvailableSlotRepository.HasOverlappingSlotAsync(
                studentId, availableSlot.StartTime, availableSlot.EndTime);
            if (hasOverlappingSlot)
            {
                return ServiceResult<Booking>.Failure("Aktív időpontod van ebben az időszakban.", 400);
            }
        }

        // Create booking
        var booking = new Booking
        {
            StudentId = studentId,
            AvailableSlotId = createRequest.AvailableSlotId,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var createdBooking = unitOfWork.BookingRepository.CreateAsync(booking);

        if (await unitOfWork.CompleteAsync())
        {

            return ServiceResult<Booking>.Success(createdBooking, 201);
        }

        return ServiceResult<Booking>.Failure("Hiba történt a foglalás létrehozása során.", 500);
    }

    public async Task<ServiceResult<BookingResponse>> UpdateBookingStatusAsync(string userId, int bookingId, BookingAcceptanceRequest acceptanceRequest)
    {
        var booking = await unitOfWork.BookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            return ServiceResult<BookingResponse>.Failure("A foglalás nem található.", 404);
        }

        // Check if the user is the teacher of this booking's slot
        if (booking.AvailableSlot.TeacherId != userId)
        {
            return ServiceResult<BookingResponse>.Failure("Nincs jogosultságod módosítani ezt a foglalást.", 403);
        }

        // Check if the booking is in the past
        if (booking.AvailableSlot.EndTime < DateTime.UtcNow)
        {
            return ServiceResult<BookingResponse>.Failure("Múltbeli időpontra vonatkozó foglalást nem fogadhatsz el vagy utasíthatsz el.", 400);
        }

        booking.Status = acceptanceRequest.Status;
        unitOfWork.BookingRepository.UpdateAsync(booking);

        if (await unitOfWork.CompleteAsync())
        {
            var bookingDto = BookingMappers.BookingToResponse(booking);
            return ServiceResult<BookingResponse>.Success(bookingDto);
        }

        return ServiceResult<BookingResponse>.Failure("Hiba történt a foglalás módosítása során.", 500);
    }

    public async Task<ServiceResult<object>> CancelBookingAsync(string userId, int bookingId)
    {
        var booking = await unitOfWork.BookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            return ServiceResult<object>.Failure("A foglalás nem található.", 404);
        }

        // Check if user is either the student or the teacher of this booking
        if (booking.StudentId != userId && booking.AvailableSlot.TeacherId != userId)
        {
            return ServiceResult<object>.Failure("Nincs jogosultságod törölni ezt a foglalást.", 403);
        }

        // Validate 24-hour cancellation policy for students
        if (booking.StudentId == userId)
        {
            var timeUntilStart = booking.AvailableSlot.StartTime - DateTime.UtcNow;
            if (timeUntilStart.TotalHours < 24)
            {
                return ServiceResult<object>.Failure("A foglalást legalább 24 órával az időpont előtt lehet lemondani.", 400);
            }
        }

        unitOfWork.BookingRepository.DeleteAsync(booking);

        if (await unitOfWork.CompleteAsync())
        {
            return ServiceResult<object>.Success(new object(), 204);
        }

        return ServiceResult<object>.Failure("Hiba történt a foglalás törlése során.", 500);
    }
}
