using LessonLink.BusinessLogic.Models;
using Microsoft.AspNetCore.Identity;

namespace LessonLink.BusinessLogic.Helpers;

public static class RoleInitializer
{
    public static List<IdentityRole> GetIdentityRoles()
    {
        return new List<IdentityRole>
        {
            new IdentityRole
            {
                Id = "student-role-id",
                Name = Role.Student.ToString(),
                NormalizedName = Role.Student.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = "teacher-role-id",
                Name = Role.Teacher.ToString(),
                NormalizedName = Role.Teacher.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = "admin-role-id",
                Name = Role.Admin.ToString(),
                NormalizedName = Role.Admin.ToString().ToUpper()
            }
        };
    }
}
