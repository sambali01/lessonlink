using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Repositories;

public interface IBookingRepository
{
    Task<IReadOnlyCollection<Booking>> GetByStudentIdAsync(string studentId);
    Task<IReadOnlyCollection<Booking>> GetByTeacherIdAsync(string teacherId);
    Task<Booking?> GetByIdAsync(int id);
    Booking CreateAsync(Booking booking);
    void UpdateAsync(Booking booking);
    void DeleteAsync(Booking booking);
}