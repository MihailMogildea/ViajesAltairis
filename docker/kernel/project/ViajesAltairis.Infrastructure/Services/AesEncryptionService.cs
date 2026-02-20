using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Infrastructure.Services;

public class EncryptionSettings
{
    public string Key { get; set; } = null!;
}

public class AesEncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public AesEncryptionService(IOptions<EncryptionSettings> options)
    {
        _key = SHA256.HashData(Encoding.UTF8.GetBytes(options.Value.Key));
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var result = new byte[aes.IV.Length + cipherBytes.Length];
        aes.IV.CopyTo(result, 0);
        cipherBytes.CopyTo(result, aes.IV.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText)
    {
        var fullCipher = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = _key;

        var iv = fullCipher[..16];
        var cipher = fullCipher[16..];

        aes.IV = iv;
        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
}
