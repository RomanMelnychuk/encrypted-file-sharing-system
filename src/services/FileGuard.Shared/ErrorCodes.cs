namespace FileGuard.Shared;

public static class ErrorCodes
{
    public const string InternalServerError = "internal_server_error";
    public const string Unauthorized = "unauthorized_error";

    public const string UserNotFound = "user_not_found_error";
    public const string FolderNotFound = "folder_not_found_error";
    public const string UserCannotDeleteFolder = "user_cannot_delete_folder";
}