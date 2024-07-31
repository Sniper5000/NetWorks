namespace NetWorks.Network
{

    public class BaseServer
    {
        /// <summary>
        /// Starts a server with <see cref="string"/> hostname on <see cref="int"/> port
        /// </summary>
        /// <param name="hostname">IP address</param>
        /// <param name="port">Port</param>
        public void Run(string hostname, int port)
        {
            Server server = new Server(this);
            server.Run(hostname, port);
        }
        /// <summary>
        /// Fires when a Client has connected
        /// </summary>
        /// <param name="client"><see cref="ServerClient"/> that connected</param>
        public virtual void ClientReady(ServerClient client) { }
        /// <summary>
        /// Fires when a Client has disconnected
        /// </summary>
        /// <param name="client"><see cref="ServerClient"/> that disconnected</param>
        public virtual void ClientLeave(ServerClient client) { }
        /// <summary>
        /// Fires when data has been received
        /// </summary>
        /// <param name="client"><see cref="ServerClient"/> data received from</param>
        /// <param name="data"><see cref="byte"/>[] received</param>
        /// <param name="protocol"><see cref="NetworkProtocol"/> received from</param>
        /// <param name="IsEncrypted"><see cref="bool"/> If data was encrypted</param>
        public virtual void DataReceived(ServerClient client, byte[] data, NetworkProtocol protocol, bool IsEncrypted) { }
        /// <summary>
        /// Fires when data has been dropped
        /// </summary>
        /// <param name="client"><see cref="ServerClient"/> data received from</param>
        /// <param name="length"><see cref="int"/> data length</param>
        /// <param name="protocol"><see cref="NetworkProtocol"/> protocol received from</param>
        public virtual void DataDropped(ServerClient client, int length, NetworkProtocol protocol) { }
    }
}