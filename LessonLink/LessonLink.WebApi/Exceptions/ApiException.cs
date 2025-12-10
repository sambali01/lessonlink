namespace LessonLink.WebApi.Exceptions;

public class ApiException(int statusCode, string message)
{
    public int StatusCode { get; set; } = statusCode;
    public string Message { get; set; } = message;
}
