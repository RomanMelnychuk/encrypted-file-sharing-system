using FileGuard.Identity.Application.DTOs;
using FileGuard.Identity.Application.Exceptions;
using FileGuard.Identity.DataAccess;
using FileGuard.Identity.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FileGuard.Identity.Application.Queries.FileManagement;

public class GetFolderQuery(Guid folderId, string userId) : IRequest<FileSection>
{
    public Guid FolderId { get; set; } = folderId;
    public string UserId { get; set; } = userId;
}

internal class GetFolderQueryHandler(FileGuardDbContext db) : IRequestHandler<GetFolderQuery, FileSection>
{
    public async Task<FileSection> Handle(GetFolderQuery request, CancellationToken cancellationToken)
    {
        var folder = await db.UserFolders
            .Include(f => f.Folder)
            .ThenInclude(f => f.SubFolders)
            .Include(f => f.Folder)
            .ThenInclude(f => f.Files)
            .Where(f => f.UserId == request.UserId && f.FolderId == request.FolderId)
            .Select(f => f.Folder)
            .FirstOrDefaultAsync(cancellationToken);

        if (folder is null)
            throw new FolderNotFoundException($"Folder with Id = '{request.FolderId}' is not found");

        var folderTopFolders = folder.SubFolders.Select(MapFolderToDto).ToList();

        var folderFiles = folder.Files.Select(FileDto.MapFileToDto).ToList();

        return new FileSection(folderTopFolders, folderFiles);
    }


    private FolderDto MapFolderToDto(Folder folder)
    {
        return new FolderDto
        {
            Id = folder.Id,
            Name = folder.Name,
            ParentFolderId = folder.ParentFolderId,
        };
    }
}
