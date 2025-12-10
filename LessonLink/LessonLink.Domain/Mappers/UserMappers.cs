using LessonLink.BusinessLogic.DTOs.User;
using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Mappers;

public static class UserMappers
{
    public static User RegisterStudentRequestToUser(RegisterStudentRequest registerDto)
    {
        return new User
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            SurName = registerDto.SurName,
            NickName = registerDto.FirstName
        };
    }

    public static User RegisterTeacherRequestToUser(RegisterTeacherRequest registerDto)
    {
        return new User
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            SurName = registerDto.SurName,
            NickName = registerDto.FirstName
        };
    }
}
