namespace NetWorks.Network;

using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using NetWorks.Security;

public class Server
{
    public readonly BaseServer ServerHandler;
    private int clientIdCounter;

    public Server(BaseServer serverHandler)
    {
        ServerHandler = serverHandler;
    }

    public void Run(string hostname, int port)
    {
        TcpListener listener = new(IPAddress.Parse(hostname), port);
        listener.Start();

        bool keepListening = true;
        while(keepListening) 
        {
            TcpClient tcpClient = listener.AcceptTcpClient();
            Task.Run(() => HandleClient(tcpClient));
        }
    }

    private void HandleClient(TcpClient tcpClient)
    {
        SecurityKeypair keys = new();
        int clientId = clientIdCounter++;
        
        UdpClient udpClient = new(0);
        IPEndPoint localEndPoint = (udpClient.Client.LocalEndPoint as IPEndPoint) ?? throw new NullReferenceException();
        IPEndPoint remoteEndPoint = (tcpClient.Client.RemoteEndPoint as IPEndPoint) ?? throw new NullReferenceException();
        int udpPort = localEndPoint.Port;
        
        PacketProtocol.Send(tcpClient.GetStream(), JsonSerializer.SerializeToUtf8Bytes(new ServerHandshakeData(clientId, keys.PublicKey.XmlString, udpPort)));
        
        // TODO hardcoded
        const int rxLimit = 4096;
        PacketProtocol.Receive(tcpClient.GetStream(), rxLimit, out int _, out byte[]? handshakeData);
        ClientHandshakeData handshake = JsonSerializer.Deserialize<ClientHandshakeData>(handshakeData)
            ?? throw new NullReferenceException();
        
        SecurityKey publicKey = SecurityKey.FromXmlString(handshake.PublicKey);
        udpClient.Connect(remoteEndPoint.Address.MapToIPv4(), handshake.ClientUdpPort);
        ServerClient client = new(clientId, this, keys, publicKey, tcpClient, udpClient);

        ServerHandler.ClientReady(client);

        client.StartReceiving();
    }
}
