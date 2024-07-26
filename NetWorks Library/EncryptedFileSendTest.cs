using NetWorks.FileEx;
using NetWorks.Network;
using NetWorks.Security;

public class EncryptedFileSendTest
{
    public void Run()
    {
        using MemoryStream exchange = new();
        SecurityKeypair keys = new();

        FileSender send = new(exchange, keys.PublicKey);
        send.SendFile("Files/TestFile.jpg", encrypted: true);
        
        File.WriteAllBytes("traffic.bin", exchange.ToArray());
        exchange.Position = 0;

        FileReceiver recv = new(exchange, keys.PrivateKey);
        recv.DataAmountUpdated += (read, size) =>
        {
            Console.WriteLine("{0}/{1}", read, size);
        };
        byte[] data = recv.ReceiveFile(out string _, out int _);
        Console.WriteLine(data.Length);
    }
}