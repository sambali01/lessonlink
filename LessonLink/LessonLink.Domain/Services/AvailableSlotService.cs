using LessonLink.BusinessLogic.DTOs.AvailableSlot;
using LessonLink.BusinessLogic.DTOs.Common;
using LessonLink.BusinessLogic.Helpers;
using LessonLink.BusinessLogic.Mappers;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;

namespace LessonLink.BusinessLogic.Services;

public class AvailableSlotService(IUnitOfWork unitOfWork)
{
    public async Task<ServiceResult<IReadOnlyCollection<AvailableSlotWithBookingsDto>>> GetMySlotsAsync(string teacherId)
    {
        if (string.IsNullOrEmpty(teacherId))
        {
            return ServiceResult<IReadOnlyCollection<AvailableSlotWithBookingsDto>>.Failure("Teacher not found.", 401);
        }

        var slots = await unitOfWork.AvailableSlotRepository.GetByTeacherIdWithBookingsAsync(teacherId);

        var slotDtos = slots.Select(slot => new AvailableSlotWithBookingsDto
        {
            Id = slot.Id,
            TeacherId = slot.TeacherId,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            Bookings = [.. slot.Bookings
                .Where(booking => booking.Status != BookingStatus.Cancelled)
                .Select(booking => booking.ToGetDto())]
        }).ToList();

        return ServiceResult<IReadOnlyCollection<AvailableSlotWithBookingsDto>>.Success(slotDtos);
    }

    public async Task<ServiceResult<PaginatedResponse<AvailableSlotWithBookingsDto>>> GetMySlotsPaginatedAsync(string teacherId, int page = 1, int pageSize = 10)
    {
        if (string.IsNullOrEmpty(teacherId))
        {
            return ServiceResult<PaginatedResponse<AvailableSlotWithBookingsDto>>.Failure("Teacher not found.", 401);
        }

        var paginatedSlots = await unitOfWork.AvailableSlotRepository.GetByTeacherIdWithBookingsPaginatedAsync(teacherId, page, pageSize);

        var slotDtos = paginatedSlots.Items.Select(slot => new AvailableSlotWithBookingsDto
        {
            Id = slot.Id,
            TeacherId = slot.TeacherId,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            Bookings = [.. slot.Bookings
                .Where(booking => booking.Status != BookingStatus.Cancelled)
                .Select(booking => booking.ToGetDto())]
        }).ToList();

        var result = new PaginatedResponse<AvailableSlotWithBookingsDto>
        {
            Items = slotDtos,
            TotalCount = paginatedSlots.TotalCount,
            Page = paginatedSlots.Page,
            PageSize = paginatedSlots.PageSize,
            TotalPages = paginatedSlots.TotalPages
        };

        return ServiceResult<PaginatedResponse<AvailableSlotWithBookingsDto>>.Success(result);
    }

    public async Task<ServiceResult<AvailableSlotDetailsDto>> GetSlotDetailsAsync(string teacherId, int slotId)
    {
        if (string.IsNullOrEmpty(teacherId))
        {
            return ServiceResult<AvailableSlotDetailsDto>.Failure("Teacher not found.", 401);
        }

        var slot = await unitOfWork.AvailableSlotRepository.GetByIdWithBookingAsync(slotId);
        if (slot == null)
        {
            return ServiceResult<AvailableSlotDetailsDto>.Failure("Slot not found.", 404);
        }

        if (slot.TeacherId != teacherId)
        {
            return ServiceResult<AvailableSlotDetailsDto>.Failure("You do not have permission to view this slot.", 403);
        }

        var detailsDto = new AvailableSlotDetailsDto
        {
            Id = slot.Id,
            TeacherId = slot.TeacherId,
            TeacherName = slot.Teacher?.User != null
                ? $"{slot.Teacher.User.SurName} {slot.Teacher.User.FirstName}"
                : string.Empty,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            Booking = slot.Bookings?.FirstOrDefault()?.ToGetDto()
        };

        return ServiceResult<AvailableSlotDetailsDto>.Success(detailsDto);
    }

    public async Task<ServiceResult<IReadOnlyCollection<AvailableSlot>>> GetNotBookedSlotsByTeacherIdAsync(string teacherId)
    {
        var slots = await unitOfWork.AvailableSlotRepository.GetNotBookedByTeacherIdAsync(teacherId);
        return ServiceResult<IReadOnlyCollection<AvailableSlot>>.Success(slots);
    }

    public async Task<ServiceResult<AvailableSlot>> CreateSlotAsync(string teacherId, AvailableSlotCreateDto createDto)
    {
        if (string.IsNullOrEmpty(teacherId))
        {
            return ServiceResult<AvailableSlot>.Failure("Teacher in HttpContext not found.", 401);
        }

        // Validate start time is before end time
        if (createDto.StartTime >= createDto.EndTime)
        {
            return ServiceResult<AvailableSlot>.Failure("The start time must be earlier than the end time.", 400);
        }

        // Validate not in the past
        if (createDto.StartTime < DateTime.UtcNow)
        {
            return ServiceResult<AvailableSlot>.Failure("You cannot provide a time in the past.", 400);
        }

        // Check for overlapping slots
        var hasOverlap = await unitOfWork.AvailableSlotRepository.HasOverlappingSlotAsync(
            teacherId, createDto.StartTime, createDto.EndTime);
        if (hasOverlap)
        {
            return ServiceResult<AvailableSlot>.Failure("The slot overlaps with an existing slot.", 400);
        }

        var slot = new AvailableSlot
        {
            TeacherId = teacherId,
            StartTime = createDto.StartTime,
            EndTime = createDto.EndTime
        };

        var createdSlot = unitOfWork.AvailableSlotRepository.CreateAsync(slot);

        if (await unitOfWork.CompleteAsync())
        {
            return ServiceResult<AvailableSlot>.Success(createdSlot, 201);
        }

        return ServiceResult<AvailableSlot>.Failure("An error occurred while creating the slot.", 500);
    }

    public async Task<ServiceResult<bool>> DeleteSlotAsync(string teacherId, int slotId)
    {
        if (string.IsNullOrEmpty(teacherId))
        {
            return ServiceResult<bool>.Failure("You are not authenticated.", 401);
        }

        var slot = await unitOfWork.AvailableSlotRepository.GetByIdAsync(slotId);
        if (slot == null)
        {
            return ServiceResult<bool>.Failure("Slot not found.", 404);
        }

        if (slot.TeacherId != teacherId)
        {
            return ServiceResult<bool>.Failure("You do not have permission to delete this slot.", 403);
        }

        var hasBooking = await unitOfWork.AvailableSlotRepository.HasBookingAsync(slotId);
        if (hasBooking)
        {
            return ServiceResult<bool>.Failure("The slot cannot be deleted because it already has a booking.", 400);
        }

        unitOfWork.AvailableSlotRepository.DeleteAsync(slot);

        if (await unitOfWork.CompleteAsync())
        {
            return ServiceResult<bool>.Success(true);
        }

        return ServiceResult<bool>.Failure("An error occurred while deleting the slot.", 500);
    }
}
