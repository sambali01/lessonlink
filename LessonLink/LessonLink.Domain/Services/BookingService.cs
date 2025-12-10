using LessonLink.BusinessLogic.DTOs.Booking;
using LessonLink.BusinessLogic.Helpers;
using LessonLink.BusinessLogic.Mappers;
using LessonLink.BusinessLogic.Repositories;

namespace LessonLink.BusinessLogic.Services;

public class BookingService(IUnitOfWork unitOfWork)
{
    public async Task<ServiceResult<BookingGetDto[]>> GetMyBookingsAsStudentAsync(string studentId)
    {
        if (string.IsNullOrEmpty(studentId))
        {
            return ServiceResult<BookingGetDto[]>.Failure("Student not found.", 401);
        }

        var bookings = await unitOfWork.BookingRepository.GetByStudentIdAsync(studentId);

        var bookingDtos = bookings
            .Select(BookingMappers.BookingToGetDto)
            .ToArray();

        return ServiceResult<BookingGetDto[]>.Success(bookingDtos);
    }

    public async Task<ServiceResult<BookingGetDto[]>> GetMyBookingsAsTeacherAsync(string teacherId)
    {
        if (string.IsNullOrEmpty(teacherId))
        {
            return ServiceResult<BookingGetDto[]>.Failure("Teacher not found.", 401);
        }

        var bookings = await unitOfWork.BookingRepository.GetByTeacherIdAsync(teacherId);

        var bookingDtos = bookings
            .Select(BookingMappers.BookingToGetDto)
            .ToArray();

        return ServiceResult<BookingGetDto[]>.Success(bookingDtos);
    }

    public async Task<ServiceResult<BookingGetDto>> CreateBookingAsync(string studentId, BookingCreateDto createDto)
    {
        if (string.IsNullOrEmpty(studentId))
        {
            return ServiceResult<BookingGetDto>.Failure("You are not authenticated.", 401);
        }

        // Check if available slot exists and is available
        var availableSlot = await unitOfWork.AvailableSlotRepository.GetByIdAsync(createDto.AvailableSlotId);
        if (availableSlot == null)
        {
            return ServiceResult<BookingGetDto>.Failure("Available slot not found.", 404);
        }

        // Create booking
        var booking = BookingMappers.CreateDtoToBooking(createDto, studentId);
        var createdBooking = unitOfWork.BookingRepository.CreateAsync(booking);

        if (await unitOfWork.CompleteAsync())
        {
            // Get the created booking with all includes for proper DTO mapping
            var fullBooking = await unitOfWork.BookingRepository.GetByIdAsync(createdBooking.Id);
            if (fullBooking == null)
            {
                return ServiceResult<BookingGetDto>.Failure("Error retrieving created booking.", 500);
            }

            var bookingDto = BookingMappers.BookingToGetDto(fullBooking);

            return ServiceResult<BookingGetDto>.Success(bookingDto, 201);
        }

        return ServiceResult<BookingGetDto>.Failure("An error occurred while creating the booking.", 500);
    }

    public async Task<ServiceResult<BookingGetDto>> UpdateBookingStatusAsync(string userId, int bookingId, BookingUpdateStatusDto updateDto)
    {
        var booking = await unitOfWork.BookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            return ServiceResult<BookingGetDto>.Failure("Booking not found.", 404);
        }

        // Check if the user is the teacher of this booking's slot
        if (booking.AvailableSlot.TeacherId != userId)
        {
            return ServiceResult<BookingGetDto>.Failure("Unauthorized to modify this booking.", 403);
        }

        booking.Status = updateDto.Status;
        unitOfWork.BookingRepository.UpdateAsync(booking);

        if (await unitOfWork.CompleteAsync())
        {
            var bookingDto = BookingMappers.BookingToGetDto(booking);
            return ServiceResult<BookingGetDto>.Success(bookingDto);
        }

        return ServiceResult<BookingGetDto>.Failure("An error occurred while updating the booking.", 500);
    }

    public async Task<ServiceResult<object>> CancelBookingAsync(string userId, int bookingId)
    {
        var booking = await unitOfWork.BookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            return ServiceResult<object>.Failure("Booking not found.", 404);
        }

        // Check if user is either the student or the teacher of this booking
        if (booking.StudentId != userId && booking.AvailableSlot.TeacherId != userId)
        {
            return ServiceResult<object>.Failure("Unauthorized to cancel this booking.", 403);
        }

        unitOfWork.BookingRepository.DeleteAsync(booking);

        if (await unitOfWork.CompleteAsync())
        {
            return ServiceResult<object>.Success(new object(), 204);
        }

        return ServiceResult<object>.Failure("An error occurred while cancelling the booking.", 500);
    }
}