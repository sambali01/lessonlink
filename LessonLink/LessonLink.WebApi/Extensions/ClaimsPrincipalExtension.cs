using System.Security.Claims;

namespace LessonLink.WebApi.Extensions;

public static class ClaimsPrincipalExtension
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new Exception("Cannot get userId from token.");
    }
}
