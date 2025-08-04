using FileGuard.Identity.Application.DTOs;
using FileGuard.Identity.DataAccess;
using FileGuard.Identity.DataAccess.Models;
using FileGuard.Identity.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using File = FileGuard.Identity.Models.File;

namespace FileGuard.Identity.Application.Queries.FileManagement;

public class GetUserTopLevelFileSystemQuery(string userId) : IRequest<UserFileSystemDto>
{
    public string UserId { get; set; } = userId;
}

internal class GetUserTopLevelFilesQueryHandler(FileGuardDbContext db, ILogger<GetUserTopLevelFileSystemQuery> logger)
    : IRequestHandler<GetUserTopLevelFileSystemQuery, UserFileSystemDto>
{
    public async Task<UserFileSystemDto> Handle(GetUserTopLevelFileSystemQuery request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var userFilesDto = new UserFileSystemDto();

        try
        {
            userFilesDto.MyFiles = await GetMyFilesAsync(userId, cancellationToken);

            userFilesDto.SharedWithMe = await GetSharedWithMeFilesAsync(userId, cancellationToken);

            userFilesDto.SharedByMe = await GetSharedByMeFilesAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving the file system for user {UserId}", userId);
            throw;
        }

        return userFilesDto;
    }

    private async Task<FileSection> GetMyFilesAsync(string userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving My Files for user {UserId}", userId);

        var myFolders = await db.UserFolders
            .AsSplitQuery()
            .Include(f => f.Folder)
            .Where(uf => uf.UserId == userId && !uf.Folder.ParentFolderId.HasValue && uf.AccessLevel == AccessLevel.Owner)
            .AsNoTracking()
            .Select(f => f.Folder)
            .ToListAsync(cancellationToken);

        var topLevelFiles = await db.UserFiles
            .Include(uf => uf.File)
            .Where(uf => uf.UserId == userId && uf.AccessLevel == AccessLevel.Owner && !uf.File.FolderId.HasValue)
            .Select(f => f.File)
            .ToListAsync(cancellationToken);

        var folderDtos = myFolders.Select(MapFolderToDto).ToList();

        return new FileSection(folderDtos, topLevelFiles.Select(FileDto.MapFileToDto).ToList());
    }

    private async Task<FileSection> GetSharedWithMeFilesAsync(string userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving Shared With Me Files for user {UserId}", userId);

        var sharedWithMeTopFolders = await db.UserFolders
            .AsSplitQuery()
            .Include(f => f.Folder)
            .Where(uf => uf.UserId == userId && !uf.Folder.ParentFolderId.HasValue && uf.AccessLevel != AccessLevel.Owner)
            .AsNoTracking()
            .Select(f => f.Folder)
            .ToListAsync(cancellationToken);

        var topLevelFiles = await db.UserFiles
            .Include(uf => uf.File)
            .Where(uf => uf.UserId == userId && uf.AccessLevel != AccessLevel.Owner && !uf.File.FolderId.HasValue)
            .Select(f => f.File)
            .ToListAsync(cancellationToken);

        var folderDtos = sharedWithMeTopFolders.Select(MapFolderToDto).ToList();

        return new FileSection(folderDtos, topLevelFiles.Select(FileDto.MapFileToDto).ToList());
    }

    private async Task<FileSection> GetSharedByMeFilesAsync(string userId, CancellationToken cancellationToken)
    {
        var myFolders = await db.UserFolders
            .AsSplitQuery()
            .Include(f => f.Folder)
            .Where(uf => uf.UserId == userId
                         && !uf.Folder.ParentFolderId.HasValue
                         && uf.AccessLevel == AccessLevel.Owner
                         && uf.Folder.UserFolders.Count > 1)
            .AsNoTracking()
            .Select(f => f.Folder)
            .ToListAsync(cancellationToken);

        var topLevelFiles = await db.UserFiles
            .Include(uf => uf.File)
            .ThenInclude(x => x.UserFiles)
            .Where(uf => uf.UserId == userId
                         && uf.AccessLevel == AccessLevel.Owner
                         && !uf.File.FolderId.HasValue
                         && uf.File.UserFiles.Count > 1)
            .Select(f => f.File)
            .ToListAsync(cancellationToken);

        var folderDtos = myFolders.Select(MapFolderToDto).ToList();

        return new FileSection(folderDtos, topLevelFiles.Select(FileDto.MapFileToDto).ToList());
    }

    private FolderDto MapFolderToDto(Folder folder)
    {
        return new FolderDto
        {
            Id = folder.Id,
            Name = folder.Name,
            ParentFolderId = folder.ParentFolderId,
            SubFolders = [],
            Files = [],
            CreatedAt = folder.CreatedAt,
        };
    }
}

