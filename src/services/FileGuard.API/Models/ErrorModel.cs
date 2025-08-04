using System.Net;

namespace FileGuard.API.Models;

public class ErrorModel
{
    public ErrorModel()
    {
    }
    public ErrorModel(HttpStatusCode status, string message, string errorCode)
    {
        Status = (int)status;
        Message = message;
        ErrorCode = errorCode;
    }

    public ErrorModel(int status, string message, string errorCode)
    {
        Status = status;
        Message = message;
        ErrorCode = errorCode;
    }

    public int Status { get; set; }
    public string Message { get; set; }
    public string ErrorCode { get; set; }
}