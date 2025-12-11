using LessonLink.BusinessLogic.DTOs.AvailableSlot;
using LessonLink.BusinessLogic.DTOs.Common;
using LessonLink.BusinessLogic.Helpers;
using LessonLink.BusinessLogic.Mappers;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;

namespace LessonLink.BusinessLogic.Services;

public class AvailableSlotService(IUnitOfWork unitOfWork)
{
    public async Task<ServiceResult<AvailableSlotResponse>> GetSlotDetailsAsync(string teacherId, int slotId)
    {
        if (string.IsNullOrEmpty(teacherId))
        {
            return ServiceResult<AvailableSlotResponse>.Failure("A tanár nem található.", 401);
        }

        var slot = await unitOfWork.AvailableSlotRepository.GetByIdWithBookingAsync(slotId);
        if (slot == null)
        {
            return ServiceResult<AvailableSlotResponse>.Failure("Az időpont nem található.", 404);
        }

        if (slot.TeacherId != teacherId)
        {
            return ServiceResult<AvailableSlotResponse>.Failure("Nincs jogosultságod megtekinteni ezt az időpontot.", 403);
        }

        var availableSlotResponse = AvailableSlotMappers.AvailableSlotToResponse(slot);

        return ServiceResult<AvailableSlotResponse>.Success(availableSlotResponse);
    }

    public async Task<ServiceResult<PaginatedResponse<AvailableSlotResponse>>> GetCurrentSlotsPaginatedAsync(string teacherId, int page = 1, int pageSize = 10)
    {
        if (string.IsNullOrEmpty(teacherId))
        {
            return ServiceResult<PaginatedResponse<AvailableSlotResponse>>.Failure("A tanár nem található.", 401);
        }

        var paginatedSlots = await unitOfWork.AvailableSlotRepository.GetCurrentSlotsByTeacherIdWithBookingsPaginatedAsync(teacherId, page, pageSize);

        var slotDtos = paginatedSlots.Items.Select(slot => AvailableSlotMappers.AvailableSlotToResponse(slot)).ToList();

        var result = new PaginatedResponse<AvailableSlotResponse>
        {
            Items = slotDtos,
            TotalCount = paginatedSlots.TotalCount,
            Page = paginatedSlots.Page,
            PageSize = paginatedSlots.PageSize,
            TotalPages = paginatedSlots.TotalPages
        };

        return ServiceResult<PaginatedResponse<AvailableSlotResponse>>.Success(result);
    }

    public async Task<ServiceResult<PaginatedResponse<AvailableSlotResponse>>> GetPastSlotsPaginatedAsync(string teacherId, int page = 1, int pageSize = 10)
    {
        if (string.IsNullOrEmpty(teacherId))
        {
            return ServiceResult<PaginatedResponse<AvailableSlotResponse>>.Failure("A tanár nem található.", 401);
        }

        var paginatedSlots = await unitOfWork.AvailableSlotRepository.GetPastSlotsByTeacherIdWithBookingsPaginatedAsync(teacherId, page, pageSize);

        var slotDtos = paginatedSlots.Items.Select(AvailableSlotMappers.AvailableSlotToResponse).ToList();

        var result = new PaginatedResponse<AvailableSlotResponse>
        {
            Items = slotDtos,
            TotalCount = paginatedSlots.TotalCount,
            Page = paginatedSlots.Page,
            PageSize = paginatedSlots.PageSize,
            TotalPages = paginatedSlots.TotalPages
        };

        return ServiceResult<PaginatedResponse<AvailableSlotResponse>>.Success(result);
    }

    public async Task<ServiceResult<PaginatedResponse<AvailableSlotResponse>>> GetCurrentNotBookedSlotsPaginatedAsync(string teacherId, int page = 1, int pageSize = 10)
    {
        var paginatedSlots = await unitOfWork.AvailableSlotRepository.GetCurrentNotBookedSlotsByTeacherIdPaginatedAsync(teacherId, page, pageSize);
        var paginatedSlotsResponse = paginatedSlots.Items.Select(slot => AvailableSlotMappers.AvailableSlotToResponse(slot)).ToList();

        var paginatedSlotsResult = new PaginatedResponse<AvailableSlotResponse>
        {
            Items = paginatedSlotsResponse,
            TotalCount = paginatedSlots.TotalCount,
            Page = paginatedSlots.Page,
            PageSize = paginatedSlots.PageSize,
            TotalPages = paginatedSlots.TotalPages
        };

        return ServiceResult<PaginatedResponse<AvailableSlotResponse>>.Success(paginatedSlotsResult);
    }

    public async Task<ServiceResult<AvailableSlot>> CreateSlotAsync(string teacherId, CreateAvailableSlotRequest createDto)
    {
        if (string.IsNullOrEmpty(teacherId))
        {
            return ServiceResult<AvailableSlot>.Failure("Nem vagy bejelentkezve.", 401);
        }

        // Validate start time is before end time
        if (createDto.StartTime >= createDto.EndTime)
        {
            return ServiceResult<AvailableSlot>.Failure("A kezdési időpontnak korábbinak kell lennie, mint a befejezési időpont.", 400);
        }

        // Validate not in the past
        if (createDto.StartTime < DateTime.UtcNow)
        {
            return ServiceResult<AvailableSlot>.Failure("Múltbeli időpontot nem adhatsz meg.", 400);
        }

        // Check for overlapping slots
        var hasOverlap = await unitOfWork.AvailableSlotRepository.HasOverlappingSlotAsync(
            teacherId, createDto.StartTime, createDto.EndTime);
        if (hasOverlap)
        {
            return ServiceResult<AvailableSlot>.Failure("Az időpont átfedésben van egy meglévő időponttal.", 400);
        }

        // Check if teacher has any active bookings that would overlap with this new slot
        var hasOverlappingBooking = await unitOfWork.BookingRepository.HasOverlappingActiveBookingForTeacherAsync(teacherId, createDto.StartTime, createDto.EndTime);
        if (hasOverlappingBooking)
        {
            return ServiceResult<AvailableSlot>.Failure("Aktív foglalásod van ebben az időszakban.", 400);
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

        return ServiceResult<AvailableSlot>.Failure("Hiba történt az időpont létrehozása során.", 500);
    }

    public async Task<ServiceResult<bool>> DeleteSlotAsync(string teacherId, int slotId)
    {
        if (string.IsNullOrEmpty(teacherId))
        {
            return ServiceResult<bool>.Failure("Nem vagy bejelentkezve.", 401);
        }

        var slot = await unitOfWork.AvailableSlotRepository.GetByIdWithBookingAsync(slotId);
        if (slot == null)
        {
            return ServiceResult<bool>.Failure("Időpont nem található.", 404);
        }

        if (slot.TeacherId != teacherId)
        {
            return ServiceResult<bool>.Failure("Nincs jogosultságod törölni ezt az időpontot.", 403);
        }

        // Check if the slot is in the past
        if (slot.EndTime < DateTime.UtcNow)
        {
            return ServiceResult<bool>.Failure("Múltbeli időpontot nem törölhetsz.", 400);
        }

        var bookingsOnSlot = slot.Bookings;
        if (bookingsOnSlot.Count > 0)
        {
            // if there are any active bookings, prevent deletion
            var hasAnyActiveBooking = bookingsOnSlot.Any(b => b.Status != BookingStatus.Cancelled);
            if (hasAnyActiveBooking)
            {
                return ServiceResult<bool>.Failure("Ez az időpont nem törölhető, mert aktív foglalások vannak rajta.", 409);
            }

            // Delete all bookings associated with the slot because they are all cancelled
            unitOfWork.BookingRepository.DeleteRangeAsync(bookingsOnSlot);
        }

        unitOfWork.AvailableSlotRepository.DeleteAsync(slot);

        if (await unitOfWork.CompleteAsync())
        {
            return ServiceResult<bool>.Success(true);
        }

        return ServiceResult<bool>.Failure("Hiba történt az időpont törlése során.", 500);
    }

    public async Task<ServiceResult<AvailableSlotResponse>> UpdateSlotAsync(string teacherId, int slotId, UpdateAvailableSlotRequest updateRequest)
    {
        if (string.IsNullOrEmpty(teacherId))
        {
            return ServiceResult<AvailableSlotResponse>.Failure("Nem vagy bejelentkezve.", 401);
        }

        var slot = await unitOfWork.AvailableSlotRepository.GetByIdWithBookingAsync(slotId);
        if (slot == null)
        {
            return ServiceResult<AvailableSlotResponse>.Failure("Az időpont nem található.", 404);
        }

        if (slot.TeacherId != teacherId)
        {
            return ServiceResult<AvailableSlotResponse>.Failure("Nincs jogosultságod módosítani ezt az időpontot.", 403);
        }

        // Check if the slot is in the past
        if (slot.EndTime < DateTime.UtcNow)
        {
            return ServiceResult<AvailableSlotResponse>.Failure("Múltbeli időpontot nem módosíthatsz.", 400);
        }

        // Check if there's an active booking
        var hasActiveBooking = slot.Bookings.Any(b => b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed);
        if (hasActiveBooking)
        {
            return ServiceResult<AvailableSlotResponse>.Failure("Az időpont nem módosítható, mert aktív foglalás van rajta.", 400);
        }

        // Validate start time is before end time
        if (updateRequest.StartTime >= updateRequest.EndTime)
        {
            return ServiceResult<AvailableSlotResponse>.Failure("A kezdési időpontnak korábbinak kell lennie, mint a befejezési időpont.", 400);
        }

        // Validate not in the past
        if (updateRequest.StartTime < DateTime.UtcNow)
        {
            return ServiceResult<AvailableSlotResponse>.Failure("Múltbeli időpontot nem adhatsz meg.", 400);
        }

        // Check for overlapping slots (excluding current slot)
        var hasOverlap = await unitOfWork.AvailableSlotRepository.HasOverlappingSlotAsync(
            teacherId, updateRequest.StartTime, updateRequest.EndTime, slotId);
        if (hasOverlap)
        {
            return ServiceResult<AvailableSlotResponse>.Failure("Az időpont átfedésben van egy meglévő időponttal.", 400);
        }

        // Check if teacher has any active bookings that would overlap with this updated slot
        var hasOverlappingBooking = await unitOfWork.BookingRepository.HasOverlappingActiveBookingForTeacherAsync(
            teacherId, updateRequest.StartTime, updateRequest.EndTime);
        if (hasOverlappingBooking)
        {
            return ServiceResult<AvailableSlotResponse>.Failure("Aktív foglalásod van ebben az időszakban.", 400);
        }

        slot.StartTime = updateRequest.StartTime;
        slot.EndTime = updateRequest.EndTime;

        unitOfWork.AvailableSlotRepository.UpdateAsync(slot);

        if (await unitOfWork.CompleteAsync())
        {
            var updatedSlot = await unitOfWork.AvailableSlotRepository.GetByIdWithBookingAsync(slotId);
            if (updatedSlot == null)
            {
                return ServiceResult<AvailableSlotResponse>.Failure("Hiba történt a frissített időpont lekérése során.", 500);
            }

            var slotResponse = new AvailableSlotResponse
            {
                Id = updatedSlot.Id,
                TeacherId = updatedSlot.TeacherId,
                StartTime = updatedSlot.StartTime,
                EndTime = updatedSlot.EndTime,
                Bookings = [.. updatedSlot.Bookings.Select(BookingMappers.BookingToResponse)]
            };

            return ServiceResult<AvailableSlotResponse>.Success(slotResponse);
        }

        return ServiceResult<AvailableSlotResponse>.Failure("Hiba történt az időpont módosítása során.", 500);
    }
}
