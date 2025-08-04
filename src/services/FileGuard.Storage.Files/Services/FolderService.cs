using FileGuard.Storage.Interfaces;
using Microsoft.Extensions.Logging;

namespace FileGuard.Storage.Files.Services;

public class FolderService(FileStorageSettings settings, ILogger<FolderService> logger) : IFolderService
{
    private readonly string _filesBaseDirectory = settings.FilesBaseDirectory;

    public string Create(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            throw new ArgumentException("Folder path cannot be null or empty.", nameof(folderPath));

        var fullPath = Path.Combine(_filesBaseDirectory, folderPath);

        if(!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        return fullPath;
    }

    public void Delete(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            throw new ArgumentException("Folder path cannot be null or empty.", nameof(folderPath));

        var fullPath = Path.Combine(_filesBaseDirectory, folderPath);

        if (Directory.Exists(fullPath))
            Directory.Delete(fullPath);
    }

    public void DeleteAndEmptyUserFolder(string folderPath)
    {
        var fullPath = Path.Combine(_filesBaseDirectory, folderPath);

        if (!Directory.Exists(fullPath))
            return;

        var dir = new DirectoryInfo(fullPath);

        foreach (var fi in dir.GetFiles())
        {
            try
            {
                fi.Delete();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured during deletion files from Directory with path = '{DirPath}'", fullPath);
            }
        }

        foreach (var di in dir.GetDirectories())
        {
            DeleteAndEmptyUserFolder(di.FullName);
            try
            {
                di.Delete();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured during deletion Directory from Directory with path = '{DirPath}'", fullPath);
            }
        }

        Directory.Delete(fullPath);
    }
}
