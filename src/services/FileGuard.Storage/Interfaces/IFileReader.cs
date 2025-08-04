namespace FileGuard.Storage.Interfaces;

public interface IFileReader
{
    Task<byte[]> ReadAsync(string filePath);
    Task<byte[]> ReadAndDeleteAsync(string filePath);
}
