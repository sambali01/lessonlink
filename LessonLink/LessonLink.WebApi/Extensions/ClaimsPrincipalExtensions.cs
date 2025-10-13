using System.Security.Claims;

namespace LessonLink.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new Exception("Cannot get memberId from token");
    }
}
