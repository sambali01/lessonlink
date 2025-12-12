namespace LessonLink.Infrastructure.Seed.Models;

public class AvailableSlotSeedModel
{
    public int Id { get; set; }
    public required string TeacherId { get; set; }
    /// <summary>
    /// Days offset from today. Negative = past, positive = future, 0 = today
    /// </summary>
    public int DayOffset { get; set; }
    /// <summary>
    /// Start time in HH:mm format (e.g., "14:00")
    /// </summary>
    public required string StartTime { get; set; }
    /// <summary>
    /// End time in HH:mm format (e.g., "15:30")
    /// </summary>
    public required string EndTime { get; set; }
}
