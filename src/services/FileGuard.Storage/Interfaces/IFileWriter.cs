namespace FileGuard.Storage.Interfaces;

public interface IFileWriter
{
    Task WriteAsync(string filePath, byte[] data);
}
