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
            return ServiceResult<BookingResponse[]>.Failure("Student not found.", 401);
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
            return ServiceResult<BookingResponse[]>.Failure("Teacher not found.", 401);
        }

        var bookings = await unitOfWork.BookingRepository.GetByTeacherIdAsync(teacherId);

        var bookingDtos = bookings
            .Select(BookingMappers.BookingToResponse)
            .ToArray();

        return ServiceResult<BookingResponse[]>.Success(bookingDtos);
    }

    public async Task<ServiceResult<BookingResponse>> CreateBookingAsync(string studentId, CreateBookingRequest createRequest)
    {
        if (string.IsNullOrEmpty(studentId))
        {
            return ServiceResult<BookingResponse>.Failure("You are not authenticated.", 401);
        }

        // Check if available slot exists and is available
        var availableSlot = await unitOfWork.AvailableSlotRepository.GetByIdAsync(createRequest.AvailableSlotId);
        if (availableSlot == null)
        {
            return ServiceResult<BookingResponse>.Failure("Available slot not found.", 404);
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
            // Get the created booking with all includes for proper DTO mapping
            var fullBooking = await unitOfWork.BookingRepository.GetByIdAsync(createdBooking.Id);
            if (fullBooking == null)
            {
                return ServiceResult<BookingResponse>.Failure("Error retrieving created booking.", 500);
            }

            var bookingDto = BookingMappers.BookingToResponse(fullBooking);

            return ServiceResult<BookingResponse>.Success(bookingDto, 201);
        }

        return ServiceResult<BookingResponse>.Failure("An error occurred while creating the booking.", 500);
    }

    public async Task<ServiceResult<BookingResponse>> UpdateBookingStatusAsync(string userId, int bookingId, BookingAcceptanceRequest acceptanceRequest)
    {
        var booking = await unitOfWork.BookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            return ServiceResult<BookingResponse>.Failure("Booking not found.", 404);
        }

        // Check if the user is the teacher of this booking's slot
        if (booking.AvailableSlot.TeacherId != userId)
        {
            return ServiceResult<BookingResponse>.Failure("Unauthorized to modify this booking.", 403);
        }

        booking.Status = acceptanceRequest.Status;
        unitOfWork.BookingRepository.UpdateAsync(booking);

        if (await unitOfWork.CompleteAsync())
        {
            var bookingDto = BookingMappers.BookingToResponse(booking);
            return ServiceResult<BookingResponse>.Success(bookingDto);
        }

        return ServiceResult<BookingResponse>.Failure("An error occurred while updating the booking.", 500);
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
