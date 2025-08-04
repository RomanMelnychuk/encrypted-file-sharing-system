using System.Net;

namespace FileGuard.Shared.Exceptions;

public class BadRequestException(string errorCode, string message)
    : StatusCodeBaseException((int)HttpStatusCode.BadRequest, errorCode, message)
{

}
