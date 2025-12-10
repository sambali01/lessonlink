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
        Task<IReadOnlyCollection<AvailableSlot>> GetNotBookedByTeacherIdAsync(string teacherId);
        Task<bool> HasOverlappingSlotAsync(string teacherId, DateTime startTime, DateTime endTime);
        Task<bool> HasBookingAsync(int slotId);
        AvailableSlot CreateAsync(AvailableSlot slot);
        void UpdateAsync(AvailableSlot slot);
        void DeleteAsync(AvailableSlot slot);
    }
}
