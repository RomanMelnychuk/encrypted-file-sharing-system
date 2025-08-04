namespace FileGuard.Shared.Exceptions;

public class StatusCodeBaseException(int statusCode, string errorCode, string message) : Exception(message)
{
    public int StatusCode { get; set; } = statusCode;
    public string ErrorCode { get; set; } = errorCode;
}
