using LessonLink.BusinessLogic.DTOs.Teacher;
using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Mappers;

public static class TeacherMappers
{
    public static TeacherResponse TeacherToResponse(Teacher teacher)
    {
        return new TeacherResponse
        {
            UserId = teacher.UserId,
            FirstName = teacher.User.FirstName,
            SurName = teacher.User.SurName,
            NickName = teacher.User.NickName,
            ImageUrl = teacher.User.ImageUrl,
            AcceptsOnline = teacher.AcceptsOnline,
            AcceptsInPerson = teacher.AcceptsInPerson,
            Location = teacher.Location,
            HourlyRate = teacher.HourlyRate,
            Description = teacher.Description,
            Contact = teacher.Contact,
            Subjects = [.. teacher.TeacherSubjects.Select(ts => SubjectMappers.SubjectToResponse(ts.Subject))],
        };
    }
}
