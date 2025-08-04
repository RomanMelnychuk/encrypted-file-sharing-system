namespace FileGuard.Shared.Extensions;

public static class StringExtensions
{
    public static string ToUserRootFolderName(this string userId)
    {
        return Guid.TryParse(userId, out var userGuid) ? userGuid.ToString("N") : userId;
    }
}
