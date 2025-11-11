using LessonLink.BusinessLogic.DTOs.Booking;
using LessonLink.BusinessLogic.Helpers;
using LessonLink.BusinessLogic.Mappers;
using LessonLink.BusinessLogic.Repositories;

namespace LessonLink.BusinessLogic.Services;

public class BookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IAvailableSlotRepository _availableSlotRepository;
    private readonly IUserRepository _userRepository;

    public BookingService(
        IBookingRepository bookingRepository,
        IAvailableSlotRepository availableSlotRepository,
        IUserRepository userRepository)
    {
        _bookingRepository = bookingRepository;
        _availableSlotRepository = availableSlotRepository;
        _userRepository = userRepository;
    }

    public async Task<ServiceResult<BookingGetDto[]>> GetMyBookingsAsStudentAsync(string studentId)
    {
        try
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return ServiceResult<BookingGetDto[]>.Failure("Student not found.", 401);
            }

            var bookings = await _bookingRepository.GetByStudentIdAsync(studentId);

            var bookingDtos = bookings
                .Select(BookingMappers.BookingToGetDto)
                .ToArray();

            return ServiceResult<BookingGetDto[]>.Success(bookingDtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<BookingGetDto[]>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResult<BookingGetDto[]>> GetMyBookingsAsTeacherAsync(string teacherId)
    {
        try
        {
            if (string.IsNullOrEmpty(teacherId))
            {
                return ServiceResult<BookingGetDto[]>.Failure("Teacher not found.", 401);
            }

            var bookings = await _bookingRepository.GetByTeacherIdAsync(teacherId);

            var bookingDtos = bookings
                .Select(BookingMappers.BookingToGetDto)
                .ToArray();

            return ServiceResult<BookingGetDto[]>.Success(bookingDtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<BookingGetDto[]>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResult<BookingGetDto>> CreateBookingAsync(string studentId, BookingCreateDto createDto)
    {
        try
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return ServiceResult<BookingGetDto>.Failure("Student not found.", 401);
            }

            // Check if available slot exists and is available
            var availableSlot = await _availableSlotRepository.GetByIdAsync(createDto.AvailableSlotId);
            if (availableSlot == null)
            {
                return ServiceResult<BookingGetDto>.Failure("Available slot not found.", 404);
            }

            // Check if student exists
            var student = await _userRepository.GetByIdAsync(studentId);
            if (student == null)
            {
                return ServiceResult<BookingGetDto>.Failure("Student not found.", 404);
            }

            // Create booking
            var booking = BookingMappers.CreateDtoToBooking(createDto, studentId);
            var createdBooking = await _bookingRepository.CreateAsync(booking);

            // Get the created booking with all includes for proper DTO mapping
            var fullBooking = await _bookingRepository.GetByIdAsync(createdBooking.Id);
            if (fullBooking == null)
            {
                return ServiceResult<BookingGetDto>.Failure("Error retrieving created booking.", 500);
            }

            var bookingDto = BookingMappers.BookingToGetDto(fullBooking);

            return ServiceResult<BookingGetDto>.Success(bookingDto, 201);
        }
        catch (Exception ex)
        {
            return ServiceResult<BookingGetDto>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResult<BookingGetDto>> UpdateBookingStatusAsync(string userId, int bookingId, BookingUpdateStatusDto updateDto)
    {
        try
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return ServiceResult<BookingGetDto>.Failure("Booking not found.", 404);
            }

            // Check if user is the teacher of this booking
            if (booking.AvailableSlot.TeacherId != userId)
            {
                return ServiceResult<BookingGetDto>.Failure("Unauthorized to modify this booking.", 403);
            }

            booking.Status = updateDto.Status;
            await _bookingRepository.UpdateAsync(booking);

            var bookingDto = BookingMappers.BookingToGetDto(booking);

            return ServiceResult<BookingGetDto>.Success(bookingDto);
        }
        catch (Exception ex)
        {
            return ServiceResult<BookingGetDto>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResult<object>> CancelBookingAsync(string userId, int bookingId)
    {
        try
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return ServiceResult<object>.Failure("Booking not found.", 404);
            }

            // Check if user is either the student or the teacher of this booking
            if (booking.StudentId != userId && booking.AvailableSlot.TeacherId != userId)
            {
                return ServiceResult<object>.Failure("Unauthorized to cancel this booking.", 403);
            }

            await _bookingRepository.DeleteAsync(booking);

            return ServiceResult<object>.Success(new object(), 204);
        }
        catch (Exception ex)
        {
            return ServiceResult<object>.Failure(ex.Message, 500);
        }
    }
}