namespace NetWorks.Security;

using System.Security.Cryptography;

/// <summary>
/// Allows Encrypting data streams using <see cref="Aes"/> + <see cref="RSA"/>
/// </summary>
public class EncryptionAedmStream : Stream
{
    private readonly Stream outputStream;
    private readonly Aes aes = Aes.Create();
    private readonly CryptoStream cryptoStream;

    private EncryptionAedmStream(Stream outputStream)
    {
        this.outputStream = outputStream;
        cryptoStream = new(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write, true);
    }

    public static EncryptionAedmStream SetupEncryption(SecurityKey publicKey, Stream outputStream)
    {
        EncryptionAedmStream aedmStream = new(outputStream);
        WriteByteArray(outputStream, RsaEncryption.Encrypt(publicKey, aedmStream.aes.Key));
        WriteByteArray(outputStream, RsaEncryption.Encrypt(publicKey, aedmStream.aes.IV));
        return aedmStream;
    }

    private static void WriteByteArray(Stream stream, byte[] bytes)
    {
        stream.Write(BitConverter.GetBytes(bytes.Length));
        stream.Write(bytes);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        cryptoStream.Write(buffer, offset, count);
    }

    public void FlushFinalBlock() { cryptoStream.FlushFinalBlock(); }
    public override void Flush() => outputStream.Flush();

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        aes.Dispose();
    }
    
    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
}