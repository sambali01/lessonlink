using LessonLink.BusinessLogic.DTOs.Teacher;
using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Mappers;

public static class TeacherMappers
{
    public static Teacher CreateDtoToTeacher(TeacherCreateDto teacherCreateDto)
    {
        return new Teacher
        {
            UserId = teacherCreateDto.UserId
        };
    }

    public static TeacherGetDto TeacherToGetDto(Teacher teacher)
    {
        return new TeacherGetDto
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
            Subjects = [.. teacher.TeacherSubjects.Select(ts => ts.Subject.Name)],
        };
    }
}
