using System.Security.Cryptography;
using System.Text;

namespace Flight.Common;

public static class SecurityExtensions
{
    public enum HashType
    {
        MD5,
        SHA1,
        SHA256,
        SHA512
    }
    public static string Hash(this string userName, string password)
    {
        return
            (password.GetHash(HashType.SHA512) + userName.GetHash(HashType.SHA256))
            .GetHash(HashType.MD5);
    }

    public static string GetHash(this string text, HashType hashType)
    {
        string hashString = hashType switch
        {
            HashType.MD5 => GetMD5(text),
            HashType.SHA1 => GetSHA1(text),
            HashType.SHA256 => GetSHA256(text),
            HashType.SHA512 => GetSHA512(text),
            _ => "invalid_hash_key",
        };
        return hashString;
    }

    public static bool CheckHash(string original, string hashString, HashType hashType)
    {
        string originalHash = GetHash(original, hashType);
        return (originalHash == hashString);
    }

    private static string GetMD5(string text)
    {
        var encode = new UTF8Encoding();
        byte[] message = encode.GetBytes(text);
        MD5 hashString = new MD5CryptoServiceProvider();
        byte[] hashValue = hashString.ComputeHash(message);
        return hashValue.Aggregate("", (current, x) => current + $"{x:x2}");
    }

    private static string GetSHA1(string text)
    {
        var encode = new UTF8Encoding();
        byte[] message = encode.GetBytes(text);
        var hashString = new SHA1Managed();
        byte[] hashValue = hashString.ComputeHash(message);
        return hashValue.Aggregate("", (current, x) => current + $"{x:x2}");
    }

    private static string GetSHA256(string text)
    {
        var encode = new UTF8Encoding();
        byte[] message = encode.GetBytes(text);
        var hashString = new SHA256Managed();
        byte[] hashValue = hashString.ComputeHash(message);
        return hashValue.Aggregate("", (current, x) => current + $"{x:x2}");
    }

    private static string GetSHA512(string text)
    {
        var encode = new UTF8Encoding();
        byte[] message = encode.GetBytes(text);
        var hashString = new SHA512Managed();
        byte[] hashValue = hashString.ComputeHash(message);
        return hashValue.Aggregate("", (current, x) => current + $"{x:x2}");
    }
}