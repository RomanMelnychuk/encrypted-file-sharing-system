using System.Security.Cryptography;

// ****************************************************************************
// This is the file that generates the public and private key
// using the RSA algorithm, which is required by the FileGuard application.
// ----------------------------------------------------------------------------
// --------------------------- Welcome to FileGuard ---------------------------
// ----------------------------------------------------------------------------
// ****************************************************************************

var keysDirectory = @"C:\KeysStorage";

var publicKeyPath = Path.Combine(keysDirectory, "public.key");
var privateKeyPath = Path.Combine(keysDirectory, "private.key");

if (!Directory.Exists(keysDirectory))
{
    Directory.CreateDirectory(keysDirectory);
}

if (Path.Exists(privateKeyPath) || Path.Exists(publicKeyPath))
{
    Console.WriteLine("Public or Private key already exist in storage. Skipping creation...");
    return;
}

using var rsa = RSA.Create(2048);

var publicKey = rsa.ExportRSAPublicKey();
File.WriteAllBytes(publicKeyPath, publicKey);
Console.WriteLine($"Public key saved to {publicKeyPath}");

var privateKey = rsa.ExportRSAPrivateKey();
File.WriteAllBytes(privateKeyPath, privateKey);
Console.WriteLine($"Private key saved to {privateKeyPath}");

Console.WriteLine("RSA keys successfully generated!");