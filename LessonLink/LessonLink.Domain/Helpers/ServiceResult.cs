namespace LessonLink.BusinessLogic.Helpers;

public class ServiceResult<T>
{
    public bool Succeeded { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public int StatusCode { get; set; }

    public static ServiceResult<T> Success(T data, int statusCode = 200)
    {
        return new ServiceResult<T>
        {
            Succeeded = true,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ServiceResult<T> Failure(List<string> errors, int statusCode = 400)
    {
        return new ServiceResult<T>
        {
            Succeeded = false,
            Errors = errors,
            StatusCode = statusCode
        };
    }

    public static ServiceResult<T> Failure(string error, int statusCode = 400)
    {
        return new ServiceResult<T>
        {
            Succeeded = false,
            Errors = new List<string> { error },
            StatusCode = statusCode
        };
    }
}
