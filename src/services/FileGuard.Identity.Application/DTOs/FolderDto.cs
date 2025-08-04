using FileGuard.Identity.Models;

namespace FileGuard.Identity.Application.DTOs;

public class FolderDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentFolderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? SizeInBytes { get; set; }
    public IList<FolderDto> SubFolders { get; set; } = new List<FolderDto>();
    public IList<FileDto> Files { get; set; } = new List<FileDto>();
    public int? FileCount { get; set; }
}
