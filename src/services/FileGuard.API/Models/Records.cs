namespace FileGuard.API.Models;

public record CreateFolderRequest(string FolderName, Guid? ParentFolderId);