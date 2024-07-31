using System.IO;

namespace NetWorks.Security
{
    /// <summary>
    /// Asymmetric Encryption Decryption Manager
    /// <para> Allows securing and unsecuring large amounts of data by using both RSA and AES </para>
    /// </summary>
    public static class AedmEncryption
    {
        /// <summary>
        /// Encrypts <see cref="byte"/>[] using <see cref="SecurityKey"/> Target Public Key
        /// </summary>
        /// <param name="publicKey"> Target Public Key</param>
        /// <param name="data"><see cref="byte"/>[] data</param>
        /// <returns>Encrypted <see cref="byte"/>[]</returns>
        public static byte[] Encrypt(SecurityKey publicKey, byte[] data)
        {
            using MemoryStream memoryStream = new MemoryStream();
            EncryptionAedmStream encryptionAedmStream = EncryptionAedmStream.SetupEncryption(publicKey, memoryStream);
            encryptionAedmStream.Write(data);
            encryptionAedmStream.FlushFinalBlock();
            return memoryStream.ToArray();
        }
        /// <summary>
        /// Decrypts <see cref="byte"/>[] using <see cref="SecurityKey"/> Private Key
        /// </summary>
        /// <param name="privateKey"><see cref="SecurityKey"/> Private Key</param>
        /// <param name="data">Encrypted <see cref="byte"/>[] data</param>
        /// <returns>Decrypted <see cref="byte"/>[]</returns>
        public static byte[] Decrypt(SecurityKey privateKey, byte[] data)
        {
            using MemoryStream inputStream = new MemoryStream(data);
            using MemoryStream outputStream = new MemoryStream();
            DecryptionAedmStream decryptionAedmStream = DecryptionAedmStream.SetupDecryption(privateKey, inputStream);
            decryptionAedmStream.CopyTo(outputStream);
            return outputStream.ToArray();
        }
    }
}