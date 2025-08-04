using System.Security.Cryptography;
using FileGuard.Crypto.Interfaces;
using FileGuard.Storage.Interfaces;

namespace FileGuard.Crypto;

public class CryptoService(IFileReader fileReader, IFileWriter fileWriter, IKeysProvider keysProvider)
    : IFileDecryptor, IFileEncryptor
{
    public async Task DecryptAsync(string encryptedFilePath, string outputFilePath)
    {
        var encryptedData = await fileReader.ReadAsync(encryptedFilePath);

        var keyFilePath = $"{encryptedFilePath}.key";
        var encryptedKeyAndIV = await fileReader.ReadAsync(keyFilePath);

        var rsaKeySizeInBytes = 256; // 2048 bit / 8 = 256 bytes

        if (encryptedKeyAndIV.Length < rsaKeySizeInBytes * 2)
            throw new InvalidDataException("The encrypted key or IV is the wrong size.");

        var encryptedAesKey = new byte[rsaKeySizeInBytes];
        var encryptedIV = new byte[rsaKeySizeInBytes];

        Buffer.BlockCopy(encryptedKeyAndIV, 0, encryptedAesKey, 0, rsaKeySizeInBytes);
        Buffer.BlockCopy(encryptedKeyAndIV, rsaKeySizeInBytes, encryptedIV, 0, rsaKeySizeInBytes);

        var privateKey = await keysProvider.GetPrivateKeyAsync();

        byte[] aesKey;
        byte[] iv;

        using (var rsa = RSA.Create())
        {
            rsa.ImportRSAPrivateKey(privateKey, out _);
            aesKey = rsa.Decrypt(encryptedAesKey, RSAEncryptionPadding.OaepSHA256);
            iv = rsa.Decrypt(encryptedIV, RSAEncryptionPadding.OaepSHA256);
        }

        byte[] decryptedData;

        using var aes = Aes.Create();

        aes.Key = aesKey;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var msDecrypt = new MemoryStream();

        await using (var cryptoStream = new CryptoStream(msDecrypt, aes.CreateDecryptor(), CryptoStreamMode.Write))
        {
            await cryptoStream.WriteAsync(encryptedData, 0, encryptedData.Length);
            await cryptoStream.FlushFinalBlockAsync();
            decryptedData = msDecrypt.ToArray();
        }

        await fileWriter.WriteAsync(outputFilePath, decryptedData);
    }

    public async Task EncryptAsync(byte[] fileData, string outputFilePath)
    {
        using var aes = Aes.Create();

        aes.KeySize = 256;
        aes.GenerateKey();
        aes.GenerateIV();

        byte[] encryptedData;

        using var msEncrypt = new MemoryStream();

        await using (var cryptoStream = new CryptoStream(msEncrypt, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            await cryptoStream.WriteAsync(fileData, 0, fileData.Length);
            await cryptoStream.FlushFinalBlockAsync();
            encryptedData = msEncrypt.ToArray();
        }

        var publicKey = await keysProvider.GetPublicKeyAsync();

        byte[] encryptedAesKey;
        byte[] encryptedIV;

        using (var rsa = RSA.Create())
        {
            rsa.ImportRSAPublicKey(publicKey, out _);
            encryptedAesKey = rsa.Encrypt(aes.Key, RSAEncryptionPadding.OaepSHA256);
            encryptedIV = rsa.Encrypt(aes.IV, RSAEncryptionPadding.OaepSHA256);
        }

        var encryptedKeyAndIV = Combine(encryptedAesKey, encryptedIV);

        await fileWriter.WriteAsync(outputFilePath, encryptedData);

        var keyFilePath = $"{outputFilePath}.key";
        await fileWriter.WriteAsync(keyFilePath, encryptedKeyAndIV);
    }

    private static byte[] Combine(byte[] first, byte[] second)
    {
        var ret = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
        return ret;
    }
}
