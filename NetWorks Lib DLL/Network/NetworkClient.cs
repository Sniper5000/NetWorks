namespace NetWorks.Network;

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using NetWorks.Security;

/// <summary>
/// Utilitary class to send and receive byte[] via UDP and TCP
/// </summary>
public class NetworkClient
{
    private readonly TcpClient tcpClient;
    private readonly UdpClient udpClient;
    private readonly SecurityKey localPrivateKey;
    private readonly SecurityKey remotePublicKey;
    private readonly Action<byte[], NetworkProtocol, bool> dataReceiveCallback;
    private readonly Func<int, NetworkProtocol, bool> allowDataCallback;
    private readonly Stopwatch timer = new();
    private long TxThroughput = -1;
    private long RxThroughput = -1;
    private bool Metrics = false; //If enabled.. Send and Receive will be measured.
    public NetworkClient(TcpClient tcpClient, UdpClient udpClient, SecurityKey localPrivateKey, SecurityKey remotePublicKey, Action<byte[], NetworkProtocol, bool> dataReceiveCallback, Func<int, NetworkProtocol, bool> allowDataCallback, bool EnableMetrics = false)
    {
        this.tcpClient = tcpClient;
        this.udpClient = udpClient;
        this.localPrivateKey = localPrivateKey;
        this.remotePublicKey = remotePublicKey;
        this.dataReceiveCallback = dataReceiveCallback;
        this.allowDataCallback = allowDataCallback;
        Metrics = EnableMetrics;
        if (EnableMetrics)
            timer.Start();
    }
    /// <summary>
    /// Starts listening on both TCP & UDP
    /// </summary>
    public void Listen()
    {
        Task tcpTask = Task.Run(ReceiveTcp);
        Task udpTask = Task.Run(ReceiveUdp);
        Task.WaitAll(tcpTask, udpTask);
    }
    /// <summary>
    /// Closes both TCP & UDP clients
    /// </summary>
    public void Close()
    {
        tcpClient.Close();
        udpClient.Close();
    }
    /// <summary>
    /// Get current TCP IP Address
    /// </summary>
    /// <returns>IP Address </returns>
    public IPAddress GetAddress()
    {
        return ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
    }
    /// <summary>
    /// Sends <see cref="byte"/>[] through <see cref="NetworkProtocol"/> protocol
    /// </summary>
    /// <param name="data"><see cref="byte"/>[] to be sent</param>
    /// <param name="protocol"><see cref="NetworkProtocol"/> to be used</param>
    /// <param name="encrypt">Should the data be encrypted or not (TCP ONLY)</param>
    public void Send(byte[] data, NetworkProtocol protocol, bool encrypt)
    {
        // TODO look into using slices/spans/streams if memory issues persist
        if(encrypt) 
            data = AedmEncryption.Encrypt(remotePublicKey, data);

        switch(protocol)
        {
            case NetworkProtocol.TCP:
                if(!tcpClient.Connected)
                    break;
                SendPacket(tcpClient.GetStream(), data, encrypt);
                break;
            case NetworkProtocol.UDP:
                {
                    using MemoryStream stream = new();
                    SendPacket(stream, data, encrypt);
                    udpClient.Send(stream.ToArray());
                }
                break;
        }
    }

    private void SendPacket(Stream stream, byte[] data, bool encrypted)
    {
        stream.Write(BitConverter.GetBytes(data.Length));
        stream.WriteByte((byte)(encrypted ? 1 : 0));
        stream.Write(data);

        if (Metrics) TxThroughput += data.Length + 5; //5 due to bool & int
    }

    private void ReceivePacket(Stream stream, NetworkProtocol protocol)
    {
        int dataLength = BitConverter.ToInt32(stream.ReadExactly(4));
        bool encrypted = stream.ReadByte() == 1;
        bool allow = allowDataCallback(dataLength, protocol);
        if(!allow) return;

        if(Metrics) RxThroughput += dataLength + 5;
        
        byte[] data = stream.ReadExactly(dataLength);
        if(encrypted) data = AedmEncryption.Decrypt(localPrivateKey, data);
        dataReceiveCallback(data, protocol, encrypted);
    }

    private void ReceiveTcp()
    {
        while(tcpClient.Connected)
        {
            bool disconnected = true;

            try
            {
                ReceivePacket(tcpClient.GetStream(), NetworkProtocol.TCP);
                disconnected = false;
            }
            catch(EndOfStreamException) { }
            catch(IOException) { }

            if(disconnected)
            {
                Close();
            }
        }
    }

    private void ReceiveUdp()
    {
        while(udpClient.Client != null)
        {
            try
            {
                IPEndPoint? remoteEP = null;
                byte[] data = udpClient.Receive(ref remoteEP);
                using MemoryStream stream = new(data);
                ReceivePacket(stream, NetworkProtocol.UDP);
            }
            catch(SocketException ex) when (ex.ErrorCode == 10004)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Provides the overall Rx & Tx data sent during Interval
    /// </summary>
    /// <param name="Interval"> Elapsed Milliseconds since last call. </param>
    /// <returns> Rx (Receiver) & Tx (Transmitter) throughput</returns>
    public long[] GetSpeeds(out long Interval)
    {
        Interval = timer.ElapsedMilliseconds;
        long[] Throughput = { RxThroughput, TxThroughput };
        timer.Reset();
        RxThroughput = 0;
        TxThroughput = 0;
        
        return Throughput;
    }
}


public static class PacketProtocol
{

    public static void Receive(Stream stream, int? rxLimit, out int packetLength, out byte[]? packetData)
    {
        packetLength = BitConverter.ToInt32(stream.ReadExactly(4));

        if(rxLimit != null && packetLength > rxLimit)
        {
            packetData = null;
            return;
        }
        
        packetData = stream.ReadExactly(packetLength);
    }

    public static void Send(Stream stream, byte[] packetData)
    {
        stream.Write(BitConverter.GetBytes(packetData.Length));
        stream.Write(packetData);
    }
}
