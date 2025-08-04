using FileGuard.Identity.Application.Exceptions;
using FileGuard.Identity.Application.Queries.FileManagement;
using FileGuard.Identity.DataAccess;
using FileGuard.Identity.DataAccess.Models;
using FileGuard.Identity.Models;
using FileGuard.Shared;
using FileGuard.Shared.Extensions;
using FileGuard.Storage.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FileGuard.Identity.Application.Commands.FileManagement;

public class DeleteFolderCommand(Guid folderId, string userId) : IRequest
{
    public Guid FolderId { get; set; } = folderId;
    public string UserId { get; set; } = userId;
}

public class DeleteFolderCommandHandler(
    FileGuardDbContext db,
    IFolderService folderService,
    IMediator mediator,
    ILogger<DeleteFolderCommandHandler> logger)
    : IRequestHandler<DeleteFolderCommand>
{
    public async Task Handle(DeleteFolderCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var folder = await db.Folders
                             .AsSplitQuery()
                             .Include(f => f.UserFolders)
                             .Include(f => f.SubFolders)
                             .Include(f => f.Files)
                             .FirstOrDefaultAsync(f => f.Id == request.FolderId, cancellationToken)
                         ?? throw new FolderNotFoundException($"Unable to delete folder. Folder with Id = '{request.FolderId}' not found.");

            var userFolder = folder.UserFolders.FirstOrDefault(u => u.AccessLevel == AccessLevel.Owner);
            if (userFolder == null)
            {
                throw new UserInvalidOperationException(
                    $"Folder with ID {request.FolderId} has no owner.",
                    ErrorCodes.UserCannotDeleteFolder);
            }

            if (userFolder.UserId != userId)
            {
                throw new UserInvalidOperationException(
                    $"The user = '{userId}' does not have the right to delete this folder because he is not its Owner.",
                    ErrorCodes.UserCannotDeleteFolder);
            }

            await DeleteFolderFromFileSystemAsync(folder.Id, userId, cancellationToken);

            var foldersToDelete = await GetAllDescendantFoldersAsync(folder.Id, cancellationToken);
            foldersToDelete.Add(folder);

            var folderIds = foldersToDelete.Select(f => f.Id).ToList();
            var userFolders = await db.UserFolders
                .Where(uf => folderIds.Contains(uf.FolderId))
                .ToListAsync(cancellationToken);

            db.UserFolders.RemoveRange(userFolders);

            var files = await db.Files
                .Where(file => file.FolderId.HasValue && folderIds.Contains(file.FolderId.Value))
                .ToListAsync(cancellationToken);

            db.Files.RemoveRange(files);

            db.Folders.RemoveRange(foldersToDelete);

            await db.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("Folder with ID {FolderId} and all its descendants were successfully deleted.", request.FolderId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "An error occurred while deleting folder with ID: {FolderId}", request.FolderId);
            throw;
        }
    }

    private async Task DeleteFolderFromFileSystemAsync(Guid folderId, string userId, CancellationToken cancellationToken)
    {
        try
        {
            var folderPath = await mediator.Send(new GetFolderPathQuery(folderId, userId), cancellationToken);

            var fullPath = Path.Combine(userId.ToUserRootFolderName(), folderPath);

            folderService.DeleteAndEmptyUserFolder(fullPath);

        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured during deletion folder = '{FolderId}' from file system", folderId);
            throw;
        }
    }

    private async Task<List<Folder>> GetAllDescendantFoldersAsync(Guid folderId, CancellationToken cancellationToken)
    {
        var foldersToDelete = new List<Folder>();
        var foldersQueue = new Queue<Guid>();
        foldersQueue.Enqueue(folderId);

        while (foldersQueue.Any())
        {
            var currentFolderId = foldersQueue.Dequeue();

            var childFolders = await db.Folders
                .Where(f => f.ParentFolderId == currentFolderId)
                .ToListAsync(cancellationToken);

            foreach (var child in childFolders)
            {
                foldersToDelete.Add(child);
                foldersQueue.Enqueue(child.Id);
            }
        }

        return foldersToDelete;
    }
}

