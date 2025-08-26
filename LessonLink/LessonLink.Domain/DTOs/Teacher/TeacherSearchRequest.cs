namespace LessonLink.BusinessLogic.DTOs.Teacher;

public class TeacherSearchRequest
{
    public string? SearchQuery { get; set; }
    public List<string>? Subjects { get; set; }
    public int? MinPrice { get; set; }
    public int? MaxPrice { get; set; }
    public double? MinRating { get; set; }
    public bool? AcceptsOnline { get; set; }
    public bool? AcceptsInPerson { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}