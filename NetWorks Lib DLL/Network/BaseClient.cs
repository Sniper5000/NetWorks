using NetWorks.Security;
using System.Diagnostics;
using System.Xml.Linq;

namespace NetWorks.Network;

public class BaseClient
{
    private Client client;

    public BaseClient()
    {
        client = new(this);
    }


    public int Id => client.Id;
    public SecurityKeypair Keys => client.Keys;
    public SecurityKey? ServerPublicKey => client.RemotePublicKey;
    /// <summary>
    /// Sets Transmitter limit
    /// </summary>
    /// <param name="limit">Transmitter buffer max size</param>
    public void SetTransmissionLimit(int? limit)
    {
        client.TxLengthLimit = limit;
    }
    /// <summary>
    /// Sets Receiver limit
    /// </summary>
    /// <param name="limit">Receiver buffer max size</param>
    public void SetReceptionLimit(int? limit)
    {
        client.RxLengthLimit = limit;
    }
    /// <summary>
    /// Connects the client to <see cref="string"/> hostname with <see cref="int"/> port
    /// </summary>
    /// <param name="hostname"><see cref="string"/> IP address to connect to</param>
    /// <param name="port"><see cref="int"/> Port</param>
    public void Connect(string hostname, int port)
    {
        client.Connect(hostname, port);
    }
    /// <summary>
    /// Disconnects the client.
    /// </summary>
    public void Disconnect()
    {
        client.Disconnect();
    }
    /// <summary>
    /// Sends <see cref="byte"/>[] data over <see cref="NetworkProtocol"/> protocol
    /// </summary>
    /// <param name="data"><see cref="byte"/>[] to be sent</param>
    /// <param name="protocol"><see cref="NetworkProtocol"/> to be used</param>
    /// <param name="encrypt"><see cref="bool"/> encrypt?</param>
    public void Send(byte[] data, NetworkProtocol protocol, bool encrypt = false)
    {
        client.Send(data, protocol, encrypt);
    }
    /// <summary>
    /// Fires when the Client has connected
    /// </summary>
    public virtual void ClientReady() { }
    /// <summary>
    /// Fires when the Client has disconnected
    /// </summary>
    public virtual void ClientLeave() { }
    /// <summary>
    /// Fires after all data has been received.
    /// </summary>
    /// <param name="data"> <see cref="byte"/>[] that has been received.</param>
    /// <param name="protocol"> Reception protocol.</param>
    /// <param name="IsEncrypted"> Encryption Enabled?</param>
    public virtual void DataReceived(byte[] data, NetworkProtocol protocol, bool IsEncrypted = false) { }
    /// <summary>
    /// Fires whenever data was dropped.
    /// </summary>
    /// <param name="length"><see cref="int"/> data length dropped</param>
    /// <param name="protocol"><see cref="NetworkProtocol"/> received from</param>
    public virtual void DataDropped(int length, NetworkProtocol protocol) { }
}