using FileGuard.Crypto.Interfaces;
using FileGuard.Identity.DataAccess;
using FileGuard.Identity.DataAccess.Models;
using FileGuard.Shared.Exceptions;
using FileGuard.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FileGuard.Identity.Application.Queries.FileManagement;

public class GetUserFileQuery(string userId, Guid fileId) : IRequest<(string FileName, byte[] FileContent)>
{
    public Guid FileId { get; set; } = fileId;
    public string UserId { get; set; } = userId;
}

internal class GetUserQueryHandler(FileGuardDbContext db, ICryptoClient crypto) : IRequestHandler<GetUserFileQuery, (string FileName, byte[] FileContent)>
{
    public async Task<(string FileName, byte[] FileContent)> Handle(GetUserFileQuery request, CancellationToken cancellationToken)
    {
        var file = await db.UserFiles
            .Include(f => f.File)
            .Where(uf => uf.UserId == request.UserId && uf.FileId == request.FileId)
            .Select(f => f.File)
            .FirstOrDefaultAsync(cancellationToken);

        if (file == null)
        {
            throw new StatusCodeBaseException(404, "file_not_found",
                $"File with Id = '{request.FileId}' was not found");
        }

        var ownerId = await db.UserFiles.Where(uf => uf.FileId == request.FileId && uf.AccessLevel == AccessLevel.Owner)
            .Select(f => f.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        var path = Path.Combine(ownerId!.ToUserRootFolderName(), file.Path);

        var content = await crypto.DecryptAndDownloadAsync(path, cancellationToken);

        return (file.Name, content);
    }
}