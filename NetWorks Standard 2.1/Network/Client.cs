using System;
using System.Net;
using System.Net.Sockets;
using NetWorks.Security;

namespace NetWorks.Network
{
    class Client
    {
        public int Id;
        public readonly SecurityKeypair Keys = new SecurityKeypair();
        public SecurityKey? RemotePublicKey;

        public int? TxLengthLimit;
        public int? RxLengthLimit;

        private readonly BaseClient baseClient;
        private NetworkClient? networkClient;

        public Client(BaseClient baseClient)
        {
            this.baseClient = baseClient;
        }
        /// <summary>
        /// Connects to a server using hostname and port
        /// </summary>
        /// <param name="hostname">IP address to connect to</param>
        /// <param name="port">Port to use</param>
        /// <exception cref="NullReferenceException"></exception>
        public void Connect(string hostname, int port)
        {
            TcpClient tcpClient = new TcpClient(hostname, port);
            UdpClient udpClient = new UdpClient(0);

            IPEndPoint localEndPoint = (udpClient.Client.LocalEndPoint as IPEndPoint) ?? throw new NullReferenceException();
            IPEndPoint remoteEndPoint = (tcpClient.Client.RemoteEndPoint as IPEndPoint) ?? throw new NullReferenceException();
            int udpPort = localEndPoint.Port;

            PacketProtocol.Send(tcpClient.GetStream(), Transports.SerializeBClass(new ClientHandshakeData(Keys.PublicKey.XmlString, udpPort)));
            // TODO hardcoded
            const int rxLimit = 4096;
            PacketProtocol.Receive(tcpClient.GetStream(), rxLimit, out int _, out byte[]? handshakeData);
            ServerHandshakeData handshake = Transports.DeserializeBClass<ServerHandshakeData>(handshakeData)
                ?? throw new NullReferenceException();

            RemotePublicKey = SecurityKey.FromXmlString(handshake.PublicKey);
            udpClient.Connect(remoteEndPoint.Address.MapToIPv4(), handshake.ServerUdpPort);
            Id = handshake.ClientId;

            // TODO dropped data virtual method unused
            networkClient = new NetworkClient(tcpClient, udpClient, Keys.PrivateKey, RemotePublicKey,
                (data, protocol, enc) => baseClient.DataReceived(data, protocol, enc),
                (dataLength, protocol) =>
                {
                    if (dataLength < 0) return false;
                    if (RxLengthLimit != null && dataLength > RxLengthLimit) return false;
                    return true;
                });

            baseClient.ClientReady();
            networkClient.Listen();
            baseClient.ClientLeave();
        }
        /// <summary>
        /// Sends <see cref="byte"/>[] data over <see cref="NetworkProtocol"/>
        /// </summary>
        /// <param name="data"><see cref="byte"/>[] to be sent</param>
        /// <param name="protocol"><see cref="NetworkProtocol"/> to use</param>
        /// <param name="encrypt">Encrypt?</param>
        public void Send(byte[] data, NetworkProtocol protocol, bool encrypt = false)
        {
            networkClient?.Send(data, protocol, encrypt);
        }
        /// <summary>
        /// Provides the overall Rx & Tx data sent during Interval
        /// Interval should be used to take throughput with higher precision
        /// </summary>
        /// <param name="Interval"> Elapsed Milliseconds since last call</param>
        /// <returns> Rx (Receiver) & Tx (Transmitter) throughput</returns>
        public long[] GetSpeeds(out long Interval)
        {
            if (networkClient == null)
            {
                Interval = 0;
                return new long[] { -1, -1 };
            }

            return networkClient.GetSpeeds(out Interval);
        }
        /// <summary>
        /// Disconnects the client
        /// </summary>
        public void Disconnect()
        {
            networkClient?.Close();
        }
    }
}