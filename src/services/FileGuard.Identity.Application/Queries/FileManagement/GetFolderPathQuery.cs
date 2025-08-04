using FileGuard.Identity.Application.Exceptions;
using FileGuard.Identity.DataAccess;
using FileGuard.Identity.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FileGuard.Identity.Application.Queries.FileManagement;
public class GetFolderPathQuery(Guid folderId, string userId) : IRequest<string>
{
    public Guid FolderId { get; set; } = folderId;
    public string UserId { get; set; } = userId;
}

internal class GetFolderPathQueryHandler(FileGuardDbContext db) : IRequestHandler<GetFolderPathQuery, string>
{
    public async Task<string> Handle(GetFolderPathQuery request, CancellationToken cancellationToken)
    {
        var userFolders = await GetUserFoldersAsync(request.UserId, cancellationToken);

        var folderDictionary = userFolders.ToDictionary(f => f.Id, f => f);

        var fullFolderPath = GetFullFolderPath(request.FolderId, folderDictionary);

        return fullFolderPath;
    }

    private async Task<List<Folder>> GetUserFoldersAsync(string ownerId, CancellationToken cancellationToken)
    {
        return await db.UserFolders
            .Where(uf => uf.UserId == ownerId)
            .Include(uf => uf.Folder)
            .Select(uf => uf.Folder)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    private string GetFullFolderPath(Guid folderId, Dictionary<Guid, Folder> folderDictionary, string separator = "/")
    {
        if (!folderDictionary.ContainsKey(folderId))
        {
            throw new FolderNotFoundException($"Folder with ID '{folderId}' not found in the provided dictionary.");
        }

        var pathSegments = new List<string>();
        var currentFolder = folderDictionary[folderId];
        var visitedFolderIds = new HashSet<Guid>();

        while (currentFolder != null)
        {
            if (!visitedFolderIds.Add(currentFolder.Id))
            {
                throw new InvalidOperationException($"Cyclic link in folders. The folder with ID {currentFolder.Id} has already been visited.");
            }

            pathSegments.Add(currentFolder.Name);

            if (currentFolder.ParentFolderId.HasValue)
            {
                if (folderDictionary.TryGetValue(currentFolder.ParentFolderId.Value, out var parentFolder))
                {
                    currentFolder = parentFolder;
                }
                else
                {
                    break;
                }
            }
            else
            {
                currentFolder = null;
            }
        }

        pathSegments.Reverse();
        return string.Join(separator, pathSegments);
    }
}