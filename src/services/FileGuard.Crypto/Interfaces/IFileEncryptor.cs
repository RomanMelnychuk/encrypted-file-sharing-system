namespace FileGuard.Crypto.Interfaces;

public interface IFileEncryptor
{
    Task EncryptAsync(byte[] fileData, string outputFilePath);
}
