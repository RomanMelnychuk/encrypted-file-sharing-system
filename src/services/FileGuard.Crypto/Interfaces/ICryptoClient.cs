namespace FileGuard.Crypto.Interfaces;
public interface ICryptoClient
{
    Task<byte[]> DecryptAndDownloadAsync(string filePath, CancellationToken cancellationToken);
    Task EncryptAsync(byte[] fileData, string outputPath, CancellationToken cancellationToken);
}
