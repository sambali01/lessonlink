using LessonLink.BusinessLogic.DTOs.Common;
using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories
{
    public interface IAvailableSlotRepository
    {
        Task<AvailableSlot?> GetByIdAsync(int id);
        Task<AvailableSlot?> GetByIdWithBookingAsync(int id);
        Task<IReadOnlyCollection<AvailableSlot>> GetByTeacherIdAsync(string teacherId);
        Task<PaginatedResponse<AvailableSlot>> GetByTeacherIdPaginatedAsync(string teacherId, int page, int pageSize);
        Task<IReadOnlyCollection<AvailableSlot>> GetByTeacherIdWithBookingsAsync(string teacherId);
        Task<PaginatedResponse<AvailableSlot>> GetByTeacherIdWithBookingsPaginatedAsync(string teacherId, int page, int pageSize);
        Task<PaginatedResponse<AvailableSlot>> GetCurrentSlotsByTeacherIdWithBookingsPaginatedAsync(string teacherId, int page, int pageSize);
        Task<PaginatedResponse<AvailableSlot>> GetPastSlotsByTeacherIdWithBookingsPaginatedAsync(string teacherId, int page, int pageSize);
        Task<PaginatedResponse<AvailableSlot>> GetCurrentNotBookedSlotsByTeacherIdPaginatedAsync(string teacherId, int page, int pageSize);
        Task<bool> HasOverlappingSlotAsync(string teacherId, DateTime startTime, DateTime endTime, int? excludeSlotId = null);
        Task<bool> HasBookingAsync(int slotId);
        AvailableSlot CreateAsync(AvailableSlot slot);
        void UpdateAsync(AvailableSlot slot);
        void DeleteAsync(AvailableSlot slot);
    }
}
