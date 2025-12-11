namespace LessonLink.BusinessLogic.DTOs.AvailableSlot;

public class UpdateAvailableSlotRequest
{
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
}
