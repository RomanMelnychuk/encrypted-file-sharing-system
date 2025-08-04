using FileGuard.Identity.Models;
using File = FileGuard.Identity.Models.File;

namespace FileGuard.Identity.Application.DTOs;

public class FileDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string Path { get; set; }
    public DateTime CreatedAt { get; set; }
    public long SizeInBytes { get; set; }
    public Guid? FolderId { get; set; }
    public string? OwnerEmail { get; set; }

    public static FileDto MapFileToDto(File file)
    {
        return new FileDto
        {
            Id = file.Id,
            FileName = file.Name,
            SizeInBytes = file.SizeInBytes,
            CreatedAt = file.CreatedAt,
            FolderId = file.FolderId,
        };
    }
}
