using FileGuard.Shared;
using FileGuard.Shared.Exceptions;

namespace FileGuard.Identity.Application.Exceptions;

public class UserInvalidOperationException(string message, string errorCode)
    : BadRequestException(errorCode, message)
{
}
