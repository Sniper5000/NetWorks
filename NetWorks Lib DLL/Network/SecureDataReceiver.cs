using NetWorks.Security;
namespace NetWorks.Network;

/// <summary>
/// Receives encrypted data and decrypts it for use.
/// </summary>
public class SecureDataReceiver
{
    private readonly Stream inputStream;
    private readonly SecurityKey privateKey;
    private DecryptionAedmStream? aedmStream;
    public bool IsEncrypted = false;
    public Action<long>? DataAmountUpdated;

    public SecureDataReceiver(Stream inputStream, SecurityKey privateKey)
    {
        this.inputStream = inputStream;
        this.privateKey = privateKey;
    }

    public void ReceiveStream(Stream outputStream)
    {
        DelimitedInputStream delimitedInputStream = new(inputStream);

        if(!IsEncrypted)
        {
            CopyTo(delimitedInputStream, outputStream);    
            return;
        }
        
        aedmStream = DecryptionAedmStream.SetupDecryption(privateKey, delimitedInputStream);
        CopyTo(aedmStream, outputStream);
    }

    private void CopyTo(Stream from, Stream to)
    {
        from.CopyToWithProgress(to, amount => DataAmountUpdated?.Invoke(amount));
    }

    public byte[] ReceiveData()
    {
        using MemoryStream bufferStream = new();
        ReceiveStream(bufferStream);
        return bufferStream.ToArray();
    }
}