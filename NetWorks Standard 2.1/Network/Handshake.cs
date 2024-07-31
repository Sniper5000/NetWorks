using MemoryPack;

[MemoryPackable]
public partial class ServerHandshakeData 
{
    public int ClientId;
    public string PublicKey;
    public int ServerUdpPort;

    public ServerHandshakeData(int ClientId, string PublicKey, int ServerUdpPort)
    {
        this.ClientId = ClientId;
        this.PublicKey = PublicKey;
        this.ServerUdpPort = ServerUdpPort;
    }
};
[MemoryPackable]
public partial class ClientHandshakeData
{
    public string PublicKey;
    public int ClientUdpPort;
    public ClientHandshakeData(string PublicKey, int ClientUdpPort)
    {
        this.PublicKey= PublicKey;
        this.ClientUdpPort = ClientUdpPort;
    }
};