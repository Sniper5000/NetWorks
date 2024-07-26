using System.Net;
using System.Net.Sockets;
using NetWorks.FileEx;
using NetWorks.Network;
using NetWorks.Security;

public static class NetworkTests
{
    public static void TestFileExchange()
    {
        using MemoryStream exchange = new();
        SecurityKeypair keys = new();

        FileSender send = new(exchange, keys.PublicKey);
        FileReceiver recv = new(exchange, keys.PrivateKey);

        send.SendFile("Files/TestFile.jpg");
        exchange.Seek(0, SeekOrigin.Begin);
        byte[] file = recv.ReceiveFile(out string _, out int tag);
        Console.WriteLine("Received {0} bytes", file.Length);
    }

    public static void TestFileExchangeConnection()
    {
        IPEndPoint endpoint = new(IPAddress.Parse("127.0.0.1"), 9999);
        TcpListener server = new(endpoint);
        
        TcpClient client = new();
        SecurityKeypair clientKeys = new();

        Task.Run(() =>
        {
            server.Start();
            TcpClient sClient = server.AcceptTcpClient();
            SecurityChallenge.PerformChallengeServer(sClient, clientKeys.PublicKey);
            FileSender send = new(sClient.GetStream(), clientKeys.PublicKey);
            send.SendFile("Files/TestFile.jpg");
        });

        client.Connect(endpoint);
        SecurityChallenge.PerformChallengeClient(client, clientKeys.PrivateKey);
        FileReceiver recv = new(client.GetStream(), clientKeys.PrivateKey);

        using FileStream test = new("TEST.jpg", FileMode.Create);
        recv.ReceiveFile(test, out string filename, out int tag);
    }

    public static void TestSecurityChallengeFail()
    {
        IPEndPoint endpoint = new(IPAddress.Parse("127.0.0.1"), 9999);
        TcpListener server = new(endpoint);
        TcpClient client = new();
        SecurityKeypair clientKeys = new();
        SecurityKeypair clientKeys2 = new();
        
        Task.Run(() =>
        {
            server.Start();
            TcpClient sClient = server.AcceptTcpClient();
            bool authSuccess = SecurityChallenge.PerformChallengeServer(sClient, clientKeys.PublicKey);
            Console.WriteLine("Client connected with non-matching key pairs. Success? " + authSuccess);
        });
        
        client.Connect(endpoint);
        client.GetStream().Write(new byte[128]);
    }
}