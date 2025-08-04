using FileGuard.Shared;
using FileGuard.Shared.Exceptions;

namespace FileGuard.Identity.Application.Exceptions;

public class FolderNotFoundException(string message) : NotFoundException(ErrorCodes.FolderNotFound, message)
{
}