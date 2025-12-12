
using LessonLink.WebApi.Exceptions;
using System.Net;
using System.Text.Json;

namespace LessonLink.WebApi.MiddleWare;

public class ExceptionMiddleWare(RequestDelegate next, ILogger<ExceptionMiddleWare> logger)
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{message}", ex.Message);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new ApiException(httpContext.Response.StatusCode, ex.Message);

            var json = JsonSerializer.Serialize(response, jsonSerializerOptions);

            await httpContext.Response.WriteAsync(json);
        }
    }
}
