namespace FileGuard.Storage.Interfaces;

/// <summary>
/// Interface that should provide RSA keys from FileStorage
/// </summary>
public interface IKeysProvider
{
    Task<byte[]> GetPublicKeyAsync();
    Task<byte[]> GetPrivateKeyAsync();
}
