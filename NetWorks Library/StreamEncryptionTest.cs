using System.Text;
using NetWorks.Security;

internal class StreamEncryptionTest
{
    public void Run()
    {
        using MemoryStream exchange = new();
        SecurityKeypair keys = new();

        EncryptionAedmStream encrypt = EncryptionAedmStream.SetupEncryption(keys.PublicKey, exchange);
        byte[] encoded = Encoding.ASCII.GetBytes("A very valuable data indeed, it would be a shame if it was leaked as plain text somewhere...");
        encrypt.Write(encoded);
        encrypt.FlushFinalBlock();

        exchange.Seek(0, SeekOrigin.Begin);

        DecryptionAedmStream decrypt = DecryptionAedmStream.SetupDecryption(keys.PrivateKey, exchange);
        byte[] decoded = decrypt.ReadExactly(encoded.Length);
        string decodedMessage = Encoding.ASCII.GetString(decoded);
        Console.WriteLine(decodedMessage);
    }
}