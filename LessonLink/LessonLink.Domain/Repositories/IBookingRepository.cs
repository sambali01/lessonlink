using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories;

public interface IBookingRepository
{
    Task<IReadOnlyCollection<Booking>> GetByStudentIdAsync(string studentId);
    Task<IReadOnlyCollection<Booking>> GetByTeacherIdAsync(string teacherId);
    Task<Booking?> GetByIdAsync(int id);
    Task<bool> HasOverlappingActiveBookingForStudentAsync(string studentId, DateTime startTime, DateTime endTime);
    Task<bool> HasOverlappingActiveBookingForTeacherAsync(string teacherId, DateTime startTime, DateTime endTime);
    Booking CreateAsync(Booking booking);
    void UpdateAsync(Booking booking);
    void DeleteAsync(Booking booking);
    void DeleteRangeAsync(IEnumerable<Booking> bookings);
}
