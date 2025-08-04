namespace FileGuard.Identity.Models;

public class Folder
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public Guid? ParentFolderId { get; set; }
    public Folder ParentFolder { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<Folder> SubFolders { get; set; }
    public ICollection<File> Files { get; set; }
    public ICollection<UserFolder> UserFolders { get; set; }
}
