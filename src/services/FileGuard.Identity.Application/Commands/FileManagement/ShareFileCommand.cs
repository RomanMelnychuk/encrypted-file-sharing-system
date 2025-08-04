using FileGuard.Identity.Application.Exceptions;
using FileGuard.Identity.DataAccess;
using FileGuard.Identity.DataAccess.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FileGuard.Identity.Application.Commands.FileManagement;

public class ShareFileCommand(Guid fileId, string currentUserId, string sharedWithUserId) : IRequest
{
    public Guid FileId { get; set; } = fileId;
    public string CurrentUserId { get; set; } = currentUserId;
    public string SharedWithUserId { get; set; } = sharedWithUserId;
}

internal class ShareFileCommandHandler(FileGuardDbContext db) : IRequestHandler<ShareFileCommand>
{
    public async Task Handle(ShareFileCommand request, CancellationToken cancellationToken)
    {
        var userFile = await db.UserFiles
            .Where(uf => uf.FileId == request.FileId && uf.UserId == request.CurrentUserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userFile == null)
            throw new FileNotFoundException($"File with id = '{request.FileId}' was not found");

        if (userFile.AccessLevel != AccessLevel.Owner)
            throw new UserInvalidOperationException("User cannot share this file", "file_access_restricted_error");

        var userNeedToHaveAccessToFile = await db.Users.FirstOrDefaultAsync(u => u.Id == request.SharedWithUserId, cancellationToken)
            ?? throw new UserNotFoundException("The user you want to share the file with is not found");

        await db.UserFiles.AddAsync(new UserFile()
        {
            AccessLevel = AccessLevel.Read,
            FileId = userFile.FileId,
            UserId = userNeedToHaveAccessToFile.Id
        }, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }
}

