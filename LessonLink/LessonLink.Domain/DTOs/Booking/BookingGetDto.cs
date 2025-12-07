using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.DTOs.Booking;

public class BookingGetDto
{
    public int Id { get; set; }
    public required string StudentId { get; set; }
    public required string StudentName { get; set; }
    public int AvailableSlotId { get; set; }
    public required DateTime SlotStartTime { get; set; }
    public required DateTime SlotEndTime { get; set; }
    public required string TeacherId { get; set; }
    public required string TeacherName { get; set; }
    public BookingStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}