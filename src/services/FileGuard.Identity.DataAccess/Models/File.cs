using FileGuard.Identity.DataAccess.Models;

namespace FileGuard.Identity.Models;

public class File
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Path { get; set; }

    public DateTime CreatedAt { get; set; }
    public long SizeInBytes { get; set; }

    public Guid? FolderId { get; set; }
    public Folder Folder { get; set; }

    public ICollection<UserFile> UserFiles { get; set; }
}
