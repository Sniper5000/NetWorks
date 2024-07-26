namespace NetWorks.Security;

using System.Security.Cryptography;

/// <summary>
/// Allows Decrypting data streams using <see cref="Aes"/> + <see cref="RSA"/>
/// </summary>
public class DecryptionAedmStream : Stream
{
    private readonly Stream inputStream;
    private readonly Aes aes;
    private readonly CryptoStream cryptoStream;

    private DecryptionAedmStream(Stream inputStream, Aes aes)
    {
        this.inputStream = inputStream;
        this.aes = aes;
        cryptoStream = new(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read, true);
    }

    public static DecryptionAedmStream SetupDecryption(SecurityKey privateKey, Stream inputStream)
    {
        Aes aes = Aes.Create();
        aes.Key = RsaEncryption.Decrypt(privateKey, ReadByteArray(inputStream));
        aes.IV = RsaEncryption.Decrypt(privateKey, ReadByteArray(inputStream));

        DecryptionAedmStream aedmStream = new(inputStream, aes);
        return aedmStream;
    }

    private static byte[] ReadByteArray(Stream stream)
    {
        int size = BitConverter.ToInt32(stream.ReadExactly(4));
        return stream.ReadExactly(size);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return cryptoStream.Read(buffer, offset, count);
    }

    public override void Flush() => inputStream.Flush();

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        aes.Dispose();
    }
    

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}