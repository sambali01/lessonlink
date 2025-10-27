using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories;

public interface IBookingRepository
{
    Task<IReadOnlyCollection<Booking>> GetByStudentIdAsync(string studentId);
    Task<IReadOnlyCollection<Booking>> GetByTeacherIdAsync(string teacherId);
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking> CreateAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task DeleteAsync(Booking booking);
}