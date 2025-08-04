using FileGuard.Identity.Application.DTOs;
using FileGuard.Identity.Application.Queries.FileManagement;
using FileGuard.Identity.DataAccess;
using FileGuard.Identity.DataAccess.Models;
using FileGuard.Identity.Models;
using FileGuard.Shared.Extensions;
using FileGuard.Storage.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FileGuard.Identity.Application.Commands.FileManagement;

public class CreateFolderCommand(string folderName, string ownerId, Guid? parentFolderId) : IRequest<FolderDto>
{
    public string FolderName { get; set; } = folderName;
    public string OwnerId { get; set; } = ownerId;
    public Guid? ParentFolderId { get; set; } = parentFolderId;
}

internal class CreateFolderCommandHandler(
    FileGuardDbContext db,
    IFolderService folder,
    ILogger<CreateFolderCommandHandler> logger,
    IMediator mediator)
    : IRequestHandler<CreateFolderCommand, FolderDto>
{
    public async Task<FolderDto> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
    {
        string newFolderPath;
        try
        {
            string fullParentFolderPath;

            if (request.ParentFolderId.HasValue)
            {
                var parentFolder = await mediator.Send(new GetFolderPathQuery(request.ParentFolderId.Value, request.OwnerId), cancellationToken);
                fullParentFolderPath = Path.Combine(request.OwnerId.ToUserRootFolderName(), parentFolder);
            }
            else
            {
                fullParentFolderPath = request.OwnerId.ToUserRootFolderName();
            }

            newFolderPath = Path.Combine(fullParentFolderPath, request.FolderName);

            folder.Create(newFolderPath);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured during creation folder in file system");
            throw;
        }

        try
        {
            var newFolder = new Folder()
            {
                Id = Guid.NewGuid(),
                Name = request.FolderName,
                CreatedAt = DateTime.UtcNow,
                ParentFolderId = request.ParentFolderId,
            };

            await db.Folders.AddAsync(newFolder, cancellationToken);

            var newUserFolder = new UserFolder()
            {
                FolderId = newFolder.Id,
                UserId = request.OwnerId,
                AccessLevel = AccessLevel.Owner,
            };

            await db.UserFolders.AddAsync(newUserFolder, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            return new FolderDto()
            {
                Id = newFolder.Id,
                Name = request.FolderName,
                CreatedAt = newFolder.CreatedAt,
                SizeInBytes = 0,
                ParentFolderId = newFolder.ParentFolderId,
                Files = [],
                SubFolders = [],
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured during creation folder in database. Deleting folder in file system...");
            folder.Delete(newFolderPath);

            throw;
        }
    }

}
