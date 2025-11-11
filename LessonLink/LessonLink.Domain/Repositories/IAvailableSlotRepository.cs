using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.DTOs.Common;

namespace LessonLink.BusinessLogic.Repositories
{
    public interface IAvailableSlotRepository
    {
        Task<IReadOnlyCollection<AvailableSlot>> GetByTeacherIdAsync(string teacherId);
        Task<PaginatedResponse<AvailableSlot>> GetByTeacherIdPaginatedAsync(string teacherId, int page, int pageSize);
        Task<IReadOnlyCollection<AvailableSlot>> GetByTeacherIdWithBookingsAsync(string teacherId);
        Task<PaginatedResponse<AvailableSlot>> GetByTeacherIdWithBookingsPaginatedAsync(string teacherId, int page, int pageSize);
        Task<IReadOnlyCollection<AvailableSlot>> GetNotBookedByTeacherIdAsync(string teacherId);
        Task<AvailableSlot?> GetByIdAsync(int id);
        Task<AvailableSlot?> GetByIdWithBookingAsync(int id);
        Task<AvailableSlot> CreateAsync(AvailableSlot slot);
        Task UpdateAsync(AvailableSlot slot);
        Task DeleteAsync(AvailableSlot slot);
        Task<bool> HasOverlappingSlotAsync(string teacherId, DateTime startTime, DateTime endTime);
        Task<bool> HasBookingAsync(int slotId);
    }
}
