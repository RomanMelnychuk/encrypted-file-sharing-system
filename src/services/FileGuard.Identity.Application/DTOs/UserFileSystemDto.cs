namespace FileGuard.Identity.Application.DTOs;

public record FileSection(IList<FolderDto> Folders, IList<FileDto> Files);

public class UserFileSystemDto
{
    public FileSection MyFiles { get; set; }
    public FileSection SharedWithMe { get; set; }
    public FileSection SharedByMe { get; set; }
}

