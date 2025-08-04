using FileGuard.Identity.DataAccess.Models;

namespace FileGuard.Identity.Models;

public class UserFolder
{
    public string UserId { get; set; }
    public User User { get; set; }

    public Guid FolderId { get; set; }
    public Folder Folder { get; set; }

    public AccessLevel AccessLevel { get; set; }
}
