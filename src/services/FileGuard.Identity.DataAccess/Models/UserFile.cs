using FileGuard.Identity.Models;
using File = FileGuard.Identity.Models.File;

namespace FileGuard.Identity.DataAccess.Models;

public class UserFile
{
    public string UserId { get; set; }
    public User User { get; set; }

    public Guid FileId { get; set; }
    public File File { get; set; }

    public AccessLevel AccessLevel { get; set; }
}