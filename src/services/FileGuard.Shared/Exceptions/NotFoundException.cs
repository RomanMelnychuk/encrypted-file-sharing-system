using System.Net;

namespace FileGuard.Shared.Exceptions;

public class NotFoundException(string errorCode, string message)
    : StatusCodeBaseException((int)HttpStatusCode.NotFound, errorCode, message)
{

}
