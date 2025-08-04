using FileGuard.Identity.Application.Exceptions;
using FileGuard.Identity.DataAccess;
using FileGuard.Identity.DataAccess.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FileGuard.Identity.Application.Commands.FileManagement;

public class DeleteFileCommand(Guid fileId, string userId) : IRequest
{
    public Guid FileId { get; set; } = fileId;
    public string UserId { get; set; } = userId;
}

internal class DeleteFileCommandHandler(FileGuardDbContext db) : IRequestHandler<DeleteFileCommand>
{
    public async Task Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        var userFile = await db.UserFiles
            .Where(uf => uf.FileId == request.FileId && uf.UserId == request.UserId)
            .FirstOrDefaultAsync(cancellationToken)
                ?? throw new FileNotFoundException($"File with id = '{request.FileId}' not found");

        if (userFile.AccessLevel != AccessLevel.Owner)
            throw new UserInvalidOperationException("User can't delete file", "user_cant_delete_file");

        await db.UserFiles.Where(uf => uf.FileId == userFile.FileId).ExecuteDeleteAsync(cancellationToken);
        await db.Files.Where(f => f.Id == userFile.FileId).ExecuteDeleteAsync(cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }
}