using LessonLink.BusinessLogic.Common;
using LessonLink.BusinessLogic.DTOs.AvailableSlot;
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

    public async Task<ServiceResult<IReadOnlyCollection<AvailableSlot>>> GetMySlotsAsync(string teacherId)
    {
        try
        {
            if (string.IsNullOrEmpty(teacherId))
            {
                return ServiceResult<IReadOnlyCollection<AvailableSlot>>.Failure("Teacher not found.", 401);
            }

            var slots = await _availableSlotRepository.GetByTeacherIdAsync(teacherId);
            return ServiceResult<IReadOnlyCollection<AvailableSlot>>.Success(slots);
        }
        catch (Exception ex)
        {
            return ServiceResult<IReadOnlyCollection<AvailableSlot>>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResult<IReadOnlyCollection<AvailableSlot>>> GetSlotsByTeacherIdAsync(string teacherId)
    {
        try
        {
            var slots = await _availableSlotRepository.GetByTeacherIdAsync(teacherId);
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

    // AvailableSlotService.cs - Add hozzá ezt a metódust
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

            // Ellenőrizzük, hogy a slot a bejelentkezett tanárhoz tartozik-e
            if (slot.TeacherId != teacherId)
            {
                return ServiceResult<bool>.Failure("Nincs jogosultságod törölni ezt az időpontot.", 403);
            }

            // Ellenőrizzük, hogy nincs-e foglalás a slot-ra
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
}
