namespace NetWorks.Security;

using System.Security.Cryptography;

/// <summary>
/// Data encryption using RSA, provides better security but has a small (256 bytes) capacity
/// </summary>
public static class RsaEncryption
{
    public static byte[] Encrypt(SecurityKey publicKey, byte[] bytes)
    {
        RSACryptoServiceProvider rsa = new();
        rsa.FromXmlString(publicKey.XmlString);
        return rsa.Encrypt(bytes, false);
    }

    public static byte[] Decrypt(SecurityKey privateKey, byte[] encryptedBytes)
    {
        RSACryptoServiceProvider rsa = new();
        rsa.FromXmlString(privateKey.XmlString);
        return rsa.Decrypt(encryptedBytes, false);
    }
}