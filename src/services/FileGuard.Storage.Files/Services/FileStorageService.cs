using FileGuard.Storage.Interfaces;

namespace FileGuard.Storage.Files.Services;

public class FileStorageService(FileStorageSettings settings) : IFileReader, IFileWriter
{
    private readonly string _filesBaseDirectory = settings.FilesBaseDirectory;

    public async Task<byte[]> ReadAsync(string filePath)
    {
        var fullPath = Path.Combine(_filesBaseDirectory, filePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File by path {fullPath} not found.");

        return await File.ReadAllBytesAsync(fullPath);
    }

    public async Task<byte[]> ReadAndDeleteAsync(string filePath)
    {
        var fileData = await ReadAsync(filePath);

        var fullPath = Path.Combine(_filesBaseDirectory, filePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return fileData;
    }

    public async Task WriteAsync(string filePath, byte[] data)
    {
        if (!Directory.Exists(_filesBaseDirectory))
            Directory.CreateDirectory(_filesBaseDirectory);

        var fullPath = Path.Combine(_filesBaseDirectory, filePath);

        if (File.Exists(fullPath))
            throw new FileLoadException($"File with path = {fullPath} already exist in storage.");

        await File.WriteAllBytesAsync(fullPath, data);
    }
}
