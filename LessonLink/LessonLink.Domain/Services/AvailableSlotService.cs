using LessonLink.BusinessLogic.DTOs.AvailableSlot;
using LessonLink.BusinessLogic.DTOs.Common;
using LessonLink.BusinessLogic.Helpers;
using LessonLink.BusinessLogic.Mappers;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;

namespace LessonLink.BusinessLogic.Services;

public class AvailableSlotService
{
    private readonly IAvailableSlotRepository _availableSlotRepository;

    public AvailableSlotService(
        IAvailableSlotRepository availableSlotRepository)
    {
        _availableSlotRepository = availableSlotRepository;
    }

    public async Task<ServiceResult<IReadOnlyCollection<AvailableSlotWithBookingsDto>>> GetMySlotsAsync(string teacherId)
    {
        try
        {
            if (string.IsNullOrEmpty(teacherId))
            {
                return ServiceResult<IReadOnlyCollection<AvailableSlotWithBookingsDto>>.Failure("Teacher not found.", 401);
            }

            var slots = await _availableSlotRepository.GetByTeacherIdWithBookingsAsync(teacherId);

            var slotDtos = slots.Select(slot => new AvailableSlotWithBookingsDto
            {
                Id = slot.Id,
                TeacherId = slot.TeacherId,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                Bookings = slot.Bookings
                    .Where(booking => booking.Status != BookingStatus.Cancelled)
                    .Select(booking => booking.ToGetDto())
                    .ToList()
            }).ToList();

            return ServiceResult<IReadOnlyCollection<AvailableSlotWithBookingsDto>>.Success(slotDtos);
        }
        catch (Exception ex)
        {
            return ServiceResult<IReadOnlyCollection<AvailableSlotWithBookingsDto>>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResult<PaginatedResponse<AvailableSlotWithBookingsDto>>> GetMySlotsPaginatedAsync(string teacherId, int page = 1, int pageSize = 10)
    {
        try
        {
            if (string.IsNullOrEmpty(teacherId))
            {
                return ServiceResult<PaginatedResponse<AvailableSlotWithBookingsDto>>.Failure("Teacher not found.", 401);
            }

            var paginatedSlots = await _availableSlotRepository.GetByTeacherIdWithBookingsPaginatedAsync(teacherId, page, pageSize);

            var slotDtos = paginatedSlots.Items.Select(slot => new AvailableSlotWithBookingsDto
            {
                Id = slot.Id,
                TeacherId = slot.TeacherId,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                Bookings = slot.Bookings
                    .Where(booking => booking.Status != BookingStatus.Cancelled)
                    .Select(booking => booking.ToGetDto())
                    .ToList()
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
        catch (Exception ex)
        {
            return ServiceResult<PaginatedResponse<AvailableSlotWithBookingsDto>>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResult<IReadOnlyCollection<AvailableSlot>>> GetNotBookedSlotsByTeacherIdAsync(string teacherId)
    {
        try
        {
            var slots = await _availableSlotRepository.GetNotBookedByTeacherIdAsync(teacherId);
            return ServiceResult<IReadOnlyCollection<AvailableSlot>>.Success(slots);
        }
        catch (Exception ex)
        {
            return ServiceResult<IReadOnlyCollection<AvailableSlot>>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResult<AvailableSlot>> CreateSlotAsync(string teacherId, AvailableSlotCreateDto createDto)
    {
        try
        {
            if (string.IsNullOrEmpty(teacherId))
            {
                return ServiceResult<AvailableSlot>.Failure("Teacher in HttpContext not found.", 401);
            }

            // Start time is before end time
            if (createDto.StartTime >= createDto.EndTime)
            {
                return ServiceResult<AvailableSlot>.Failure("A kezdési időnek korábban kell lennie, mint a befejezési időnek", 400);
            }
            if (createDto.StartTime < DateTime.Now)
            {
                return ServiceResult<AvailableSlot>.Failure("Nem adhatsz múltbeli időpontot", 400);
            }

            // No overlap
            var hasOverlap = await _availableSlotRepository.HasOverlappingSlotAsync(
                teacherId, createDto.StartTime, createDto.EndTime);
            if (hasOverlap)
            {
                return ServiceResult<AvailableSlot>.Failure("Az időpont átfedésben van egy már meglévő időponttal", 400);
            }

            var slot = new AvailableSlot
            {
                TeacherId = teacherId,
                StartTime = createDto.StartTime,
                EndTime = createDto.EndTime
            };

            var createdSlot = await _availableSlotRepository.CreateAsync(slot);
            return ServiceResult<AvailableSlot>.Success(createdSlot, 201);
        }
        catch (Exception ex)
        {
            return ServiceResult<AvailableSlot>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResult<bool>> DeleteSlotAsync(string teacherId, int slotId)
    {
        try
        {
            if (string.IsNullOrEmpty(teacherId))
            {
                return ServiceResult<bool>.Failure("Tanár nem található.", 401);
            }

            var slot = await _availableSlotRepository.GetByIdAsync(slotId);
            if (slot == null)
            {
                return ServiceResult<bool>.Failure("Az időpont nem található.", 404);
            }

            if (slot.TeacherId != teacherId)
            {
                return ServiceResult<bool>.Failure("Nincs jogosultságod törölni ezt az időpontot.", 403);
            }

            var hasBooking = await _availableSlotRepository.HasBookingAsync(slotId);
            if (hasBooking)
            {
                return ServiceResult<bool>.Failure("Nem törölhető az időpont, mert már van foglalás rá.", 400);
            }

            await _availableSlotRepository.DeleteAsync(slot);
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResult<AvailableSlotDetailsDto>> GetSlotDetailsAsync(string teacherId, int slotId)
    {
        try
        {
            if (string.IsNullOrEmpty(teacherId))
            {
                return ServiceResult<AvailableSlotDetailsDto>.Failure("Tanár nem található.", 401);
            }

            var slot = await _availableSlotRepository.GetByIdWithBookingAsync(slotId);
            if (slot == null)
            {
                return ServiceResult<AvailableSlotDetailsDto>.Failure("Az időpont nem található.", 404);
            }

            if (slot.TeacherId != teacherId)
            {
                return ServiceResult<AvailableSlotDetailsDto>.Failure("Nincs jogosultságod megtekinteni ezt az időpontot.", 403);
            }

            var detailsDto = new AvailableSlotDetailsDto
            {
                Id = slot.Id,
                TeacherId = slot.TeacherId,
                TeacherName = slot.Teacher?.User != null
                    ? $"{slot.Teacher.User.FirstName} {slot.Teacher.User.SurName}"
                    : string.Empty,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                Booking = slot.Bookings?.FirstOrDefault()?.ToGetDto()
            };

            return ServiceResult<AvailableSlotDetailsDto>.Success(detailsDto);
        }
        catch (Exception ex)
        {
            return ServiceResult<AvailableSlotDetailsDto>.Failure(ex.Message, 500);
        }
    }
}
