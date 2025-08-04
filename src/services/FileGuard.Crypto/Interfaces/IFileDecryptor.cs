namespace FileGuard.Crypto.Interfaces;

public interface IFileDecryptor
{
    Task DecryptAsync(string encryptedFilePath, string outputFilePath);
}
