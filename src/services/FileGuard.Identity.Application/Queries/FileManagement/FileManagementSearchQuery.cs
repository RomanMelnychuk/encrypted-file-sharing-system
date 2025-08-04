using FileGuard.Identity.Application.DTOs;
using FileGuard.Identity.DataAccess;
using FileGuard.Identity.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FileGuard.Identity.Application.Queries.FileManagement;

public class FileManagementSearchQuery(string searchTerm, string userId) : IRequest<FileSection>
{
    public string SearchTerm { get; set; } = searchTerm;
    public string UserId { get; set; } = userId;
}

internal class FIleManagementSearchQueryHandler(FileGuardDbContext db) : IRequestHandler<FileManagementSearchQuery, FileSection>
{
    public async Task<FileSection> Handle(FileManagementSearchQuery request, CancellationToken cancellationToken)
    {
        var searchTerm = request.SearchTerm.ToLower();

        var files = await db.UserFiles
            .AsNoTracking()
            .AsSplitQuery()
            .Include(uf => uf.File)
            .Include(f => f.User)
            .Where(uf => uf.File.Name.ToLower().Contains(searchTerm) && uf.UserId == request.UserId)
            .Select(f => new FileDto
            {
                CreatedAt = f.File.CreatedAt,
                FileName = f.File.Name,
                FolderId = f.File.FolderId,
                Id = f.FileId,
                Path = f.File.Path,
                SizeInBytes = f.File.SizeInBytes,
            }).ToListAsync(cancellationToken);

        var folders = await db.UserFolders
            .AsNoTracking()
            .AsSplitQuery()
            .Include(f => f.Folder)
            .ThenInclude(f => f.Files)
            .Where(uf => uf.Folder.Name.ToLower().Contains(searchTerm) && uf.UserId == request.UserId)
            .Select(f => new FolderDto()
            {
                Id = f.FolderId,
                Name = f.Folder.Name,
                CreatedAt = f.Folder.CreatedAt,
                FileCount = f.Folder.Files.Count(),
                SizeInBytes = f.Folder.Files.Sum(file => file.SizeInBytes),
            }).ToListAsync(cancellationToken);

        var result = new FileSection(folders, files);

        return result;
    }
}