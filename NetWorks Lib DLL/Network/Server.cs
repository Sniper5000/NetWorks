using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NetWorks.Security;

namespace NetWorks.Network
{
    public class Server
    {
        public readonly BaseServer ServerHandler;
        private int clientIdCounter;
        private int Port;
        private bool keepListening;
        private TcpListener listener;
        private string hostname;
        private bool Dualmode = false;
        private AddressFamily family = AddressFamily.InterNetwork;
        public Server(BaseServer serverHandler)
        {
            ServerHandler = serverHandler;
        }

        
        public void Run(string hostname, int port)
        {
            this.hostname = hostname;
            listener = new(IPAddress.Parse(hostname), port);
            listener.Start();
            var EP = listener.Server.LocalEndPoint as IPEndPoint;
            Port = EP.Port;
            keepListening = true;
            while (keepListening)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                Console.WriteLine($"Connection Established");
                Task.Run(() => HandleClient(tcpClient));
                
            }
            Console.WriteLine("Shutdown complete");
            listener.Stop();
            listener.Dispose();
        }

        public void RunDual(int port)
        {
            Dualmode = true;
            family = AddressFamily.InterNetworkV6;
            listener = new(IPAddress.IPv6Any, port);
            listener.Server.DualMode = true;
            listener.Start();
            var EP = listener.Server.LocalEndPoint as IPEndPoint;
            Port = EP.Port;
            keepListening = true;
            while (keepListening)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                Console.WriteLine($"Connection Established");
                Task.Run(() => HandleClient(tcpClient));

            }
            Console.WriteLine("Shutdown complete");
            listener.Stop();
            listener.Dispose();
            listener = null;
        }

        public void Shutdown()
        {
            keepListening = false;
            TcpClient client = new();    

            if (listener != null)
            {
                if(hostname == null)
                    hostname = "localhost";

                client.Connect(hostname, Port);
                Thread.Sleep(1000);
                
            }
            Console.WriteLine($"Server Shutdown.");
        }

        private void HandleClient(TcpClient tcpClient)
        {
            SecurityKeypair keys = new();
            int clientId = clientIdCounter++;

            UdpClient udpClient = new(family);
            udpClient.Client.DualMode = Dualmode;
            udpClient.Client.Bind(new IPEndPoint(IPAddress.IPv6Any, 0));

            IPEndPoint localEndPoint = (udpClient.Client.LocalEndPoint as IPEndPoint) ?? throw new NullReferenceException();
            IPEndPoint remoteEndPoint = (tcpClient.Client.RemoteEndPoint as IPEndPoint) ?? throw new NullReferenceException();
            int udpPort = localEndPoint.Port;

            PacketProtocol.Send(tcpClient.GetStream(), Transports.SerializeBClass(new ServerHandshakeData(clientId, keys.PublicKey.XmlString, udpPort, GeneralSettings.Version)));

            // TODO hardcoded
            const int rxLimit = 4096;
            PacketProtocol.Receive(tcpClient.GetStream(), rxLimit, out int _, out byte[]? handshakeData);
            ClientHandshakeData handshake = Transports.DeserializeBClass<ClientHandshakeData>(handshakeData)
                ?? throw new NullReferenceException();

            if (!GeneralSettings.VersionChecker(handshake.Version))
            {
                tcpClient.Close();
                return;
            }
            SecurityKey publicKey = SecurityKey.FromXmlString(handshake.PublicKey);
            // TODO TEMPORARY PROBES!
            if (remoteEndPoint.Address.AddressFamily == AddressFamily.InterNetwork)
            {
                udpClient.Connect(remoteEndPoint.Address.MapToIPv4(), handshake.ClientUdpPort);
                Console.WriteLine("IPv4 Client Detected");
            }
            else if (remoteEndPoint.Address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                //udpClient = new(udpPort, AddressFamily.InterNetworkV6);
                //localEndPoint = (udpClient.Client.LocalEndPoint as IPEndPoint) ?? throw new NullReferenceException();
                udpClient.Connect(remoteEndPoint.Address.MapToIPv6(), handshake.ClientUdpPort);
                Console.WriteLine("IPv6 Client Detected");
            }

            ServerClient client = new(clientId, this, keys, publicKey, tcpClient, udpClient);

            ServerHandler.ClientReady(client);

            client.StartReceiving();
        }
    }
}