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
                Id = "1a1a1111-1111-1111-1111-111111111111",
                Name = Role.Student.ToString(),
                NormalizedName = Role.Student.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = "2b2b2222-2222-2222-2222-222222222222",
                Name = Role.Teacher.ToString(),
                NormalizedName = Role.Teacher.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = "3c3c3333-3333-3333-3333-333333333333",
                Name = Role.Admin.ToString(),
                NormalizedName = Role.Admin.ToString().ToUpper()
            }
        };
    }
}
