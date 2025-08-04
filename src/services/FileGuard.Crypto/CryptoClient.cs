using FileGuard.Crypto.Interfaces;
using FileGuard.Storage.Interfaces;

namespace FileGuard.Crypto;

internal class CryptoClient(
    IFileDecryptor decryptor,
    IFileEncryptor encryptor,
    IFileReader reader,
    IFolderService folder) : ICryptoClient
{
    // will be combined with default storage path;
    private const string TempDecryptedFilesPath = "DecryptedFiles";

    public async Task<byte[]> DecryptAndDownloadAsync(string filePath, CancellationToken cancellationToken)
    {
        var folderPath = folder.Create(TempDecryptedFilesPath);

        var decryptedFilePath = Path.Combine(folderPath, Path.GetFileName(filePath));

        await decryptor.DecryptAsync(filePath, decryptedFilePath);

        var decryptedData = await reader.ReadAndDeleteAsync(decryptedFilePath);

        return decryptedData;
    }

    public Task EncryptAsync(byte[] fileData, string outputPath, CancellationToken cancellationToken)
    {
        return encryptor.EncryptAsync(fileData, outputPath);
    }
}
