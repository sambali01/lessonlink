using LessonLink.BusinessLogic.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace LessonLink.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{
    protected IActionResult HandleServiceResult<T>(ServiceResult<T> result)
    {
        if (result.Succeeded)
        {
            return result.StatusCode switch
            {
                200 => Ok(result.Data),
                201 => Created(),
                204 => NoContent(),
                _ => StatusCode(result.StatusCode, result.Data)
            };
        }
        else
        {
            return result.StatusCode switch
            {
                400 => BadRequest(result.Errors),
                401 => Unauthorized(result.Errors),
                403 => Forbid(),
                404 => NotFound(result.Errors),
                409 => Conflict(result.Errors),
                _ => StatusCode(result.StatusCode, result.Errors)
            };
        }
    }
}
