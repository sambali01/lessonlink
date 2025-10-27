using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories
{
    public interface IAvailableSlotRepository
    {
        Task<IReadOnlyCollection<AvailableSlot>> GetByTeacherIdAsync(string teacherId);
        Task<IReadOnlyCollection<AvailableSlot>> GetNotBookedByTeacherIdAsync(string teacherId);
        Task<AvailableSlot?> GetByIdAsync(int id);
        Task<AvailableSlot> CreateAsync(AvailableSlot slot);
        Task UpdateAsync(AvailableSlot slot);
        Task DeleteAsync(AvailableSlot slot);
        Task<bool> HasOverlappingSlotAsync(string teacherId, DateTime startTime, DateTime endTime);
        Task<bool> HasBookingAsync(int slotId);
    }
}
