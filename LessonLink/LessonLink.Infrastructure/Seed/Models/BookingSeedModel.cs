namespace LessonLink.Infrastructure.Seed.Models;

public class BookingSeedModel
{
    public int Id { get; set; }
    public required string StudentId { get; set; }
    public int AvailableSlotId { get; set; }
    public int Status { get; set; }
    /// <summary>
    /// How many days ago was this booking created (positive number)
    /// </summary>
    public int DaysAgoCreated { get; set; }
}
