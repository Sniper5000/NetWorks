using System.Net;
using System.Net.Sockets;
using NetWorks.Security;

namespace NetWorks.Network
{
    public class ServerClient
    {
        public readonly int Id;
        public readonly Server Server;
        public readonly SecurityKey ClientPublicKey;

        public int? TxLengthLimit;
        public int? RxLengthLimit;

        public readonly SecurityKeypair Keys;
        private readonly NetworkClient networkClient;

        public ServerClient(int id, Server server, SecurityKeypair selfKeys, SecurityKey clientPublicKey, TcpClient tcpClient, UdpClient udpClient)
        {
            Id = id;
            Server = server;
            ClientPublicKey = clientPublicKey;
            Keys = selfKeys;
            networkClient = new NetworkClient(tcpClient, udpClient, selfKeys.PrivateKey, clientPublicKey,
                (data, protocol, enc) => Server.ServerHandler.DataReceived(this, data, protocol, enc),
                (dataLength, protocol) =>
                {
                    if (dataLength < 0) return false;
                    if (RxLengthLimit != null && dataLength > RxLengthLimit) return false;
                    return true;
                });
        }
        /// <summary>
        /// Gets the current client IP Address
        /// </summary>
        /// <returns></returns>
        public IPAddress GetAddress()
        {
            return networkClient.GetAddress();
        }
        /// <summary>
        /// Starts listening for incoming packets
        /// </summary>
        public void StartReceiving()
        {
            networkClient.Listen();
            Server.ServerHandler.ClientLeave(this);
        }
        /// <summary>
        /// Sends a <see cref="byte"/>[] using protocol with encrypt
        /// </summary>
        /// <param name="data"> Data to be sent </param>
        /// <param name="protocol"> Protocol to be used TCP/UDP </param>
        /// <param name="encrypt"> Whether encryption should be used.</param>
        public void Send(byte[] data, NetworkProtocol protocol = NetworkProtocol.TCP, bool encrypt = false)
        {
            networkClient.Send(data, protocol, encrypt);
        }
        /// <summary>
        /// Disconnects the server client
        /// </summary>
        public void Disconnect()
        {
            networkClient.Close();
        }
    }
}