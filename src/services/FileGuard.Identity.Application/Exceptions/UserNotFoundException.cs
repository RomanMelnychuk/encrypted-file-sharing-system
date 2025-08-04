using FileGuard.Shared;
using FileGuard.Shared.Exceptions;

namespace FileGuard.Identity.Application.Exceptions;

public class UserNotFoundException(string message) : NotFoundException(ErrorCodes.UserNotFound, message)
{
}

