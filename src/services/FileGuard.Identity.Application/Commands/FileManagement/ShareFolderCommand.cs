using FileGuard.Identity.Application.Exceptions;
using FileGuard.Identity.DataAccess;
using FileGuard.Identity.DataAccess.Models;
using FileGuard.Identity.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FileGuard.Identity.Application.Commands.FileManagement;

public class ShareFolderCommand(Guid folderId, string currentUserId, string sharedWithUserId) : IRequest
{
    public Guid FolderId { get; set; } = folderId;
    public string CurrentUserId { get; set; } = currentUserId;
    public string SharedWithUserId { get; set; } = sharedWithUserId;
}

internal class ShareFolderCommandHandler(FileGuardDbContext db) : IRequestHandler<ShareFolderCommand>
{
    public async Task Handle(ShareFolderCommand request, CancellationToken cancellationToken)
    {
        var userFolder = await db.UserFolders
            .Where(uf => uf.FolderId == request.FolderId && uf.UserId == request.CurrentUserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userFolder == null)
            throw new FolderNotFoundException($"Folder with id = '{request.FolderId}' was not found");

        if (userFolder.AccessLevel != AccessLevel.Owner)
            throw new UserInvalidOperationException("User cannot share this folder", "folder_access_restricted_error");

        var sharedWithUser = await db.Users
            .FirstOrDefaultAsync(u => u.Id == request.SharedWithUserId, cancellationToken)
            ?? throw new UserNotFoundException("The user you want to share the folder with is not found");

        var allFolderIds = await GetAllSubFolderIds(request.FolderId, cancellationToken);

        var allFiles = await db.Files
            .Where(f => f.FolderId.HasValue && allFolderIds.Contains(f.FolderId.Value))
            .ToListAsync(cancellationToken);

        var existingFolderAccesses = await db.UserFolders
            .Where(uf => allFolderIds.Contains(uf.FolderId) && uf.UserId == request.SharedWithUserId)
            .Select(uf => uf.FolderId)
            .ToListAsync(cancellationToken);

        var newUserFolders = allFolderIds
            .Where(fid => !existingFolderAccesses.Contains(fid))
            .Select(fid => new UserFolder
            {
                UserId = sharedWithUser.Id,
                AccessLevel = AccessLevel.Read,
                FolderId = fid,
            })
            .ToList();

        await db.UserFolders.AddRangeAsync(newUserFolders, cancellationToken);

        var existingFileAccesses = await db.UserFiles
            .Where(uf => allFiles.Select(f => f.Id).Contains(uf.FileId) && uf.UserId == request.SharedWithUserId)
            .Select(uf => uf.FileId)
            .ToListAsync(cancellationToken);

        var newUserFiles = allFiles
            .Where(f => !existingFileAccesses.Contains(f.Id))
            .Select(f => new UserFile
            {
                UserId = sharedWithUser.Id,
                AccessLevel = AccessLevel.Read,
                FileId = f.Id,
            })
            .ToList();

        await db.UserFiles.AddRangeAsync(newUserFiles, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<Guid>> GetAllSubFolderIds(Guid folderId, CancellationToken cancellationToken)
    {
        var result = new List<Guid>();
        var stack = new Stack<Guid>();
        stack.Push(folderId);

        while (stack.Count > 0)
        {
            var currentId = stack.Pop();
            result.Add(currentId);

            var subFolders = await db.Folders
                .Where(f => f.ParentFolderId == currentId)
                .Select(f => f.Id)
                .ToListAsync(cancellationToken);

            foreach (var subFolderId in subFolders)
            {
                stack.Push(subFolderId);
            }
        }

        return result;
    }
}