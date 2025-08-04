using FileGuard.Storage.Interfaces;

namespace FileGuard.Storage.Files.Providers;
public class FileStorageKeysProvider(FileStorageSettings settings) : IKeysProvider
{
    private readonly string _generalPublicKeyPath = $"{settings.KeysBaseDirectory}/public.key";
    private readonly string _generalPrivateKeyPath = $"{settings.KeysBaseDirectory}/private.key";

    public async Task<byte[]> GetPublicKeyAsync()
    {
        if (!File.Exists(_generalPublicKeyPath))
            throw new FileNotFoundException($"Public key not found at {_generalPublicKeyPath}.");

        return await File.ReadAllBytesAsync(_generalPublicKeyPath);
    }

    public async Task<byte[]> GetPrivateKeyAsync()
    {
        if (!File.Exists(_generalPrivateKeyPath))
            throw new FileNotFoundException($"Private key not found at {_generalPrivateKeyPath}.");

        return await File.ReadAllBytesAsync(_generalPrivateKeyPath);
    }
}
