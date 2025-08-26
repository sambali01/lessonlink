namespace LessonLink.BusinessLogic.Models;

public class AvailableSlot
{
    public int Id { get; set; }

    public string TeacherId { get; set; }
    public Teacher Teacher { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
