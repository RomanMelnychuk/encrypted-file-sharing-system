using FileGuard.Identity.DataAccess.Models;
using Microsoft.AspNetCore.Identity;

namespace FileGuard.Identity.Models;

public class User : IdentityUser
{
    public ICollection<UserFile> UserFiles { get; set; }
    public ICollection<UserFolder> UserFolders { get; set; }
}
