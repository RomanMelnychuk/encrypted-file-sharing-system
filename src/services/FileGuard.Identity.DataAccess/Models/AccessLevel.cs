namespace FileGuard.Identity.DataAccess.Models;

[Flags]
public enum AccessLevel : byte
{
    None = 0,
    Read = 1,
    Owner = 255,
}
