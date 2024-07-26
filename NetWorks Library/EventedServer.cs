using NetWorks.Network;

class EventedServer : BaseServer
{
    public event Action<ServerClient>? OnClientReady;
    public event Action<ServerClient>? OnClientLeave;
    public event Action<ServerClient, byte[], NetworkProtocol, bool>? OnDataReceive;


    public override void ClientReady(ServerClient client)
    {
        OnClientReady?.Invoke(client);
    }

    public override void ClientLeave(ServerClient client)
    {
        OnClientLeave?.Invoke(client);
    }

    public override void DataReceived(ServerClient client, byte[] data, NetworkProtocol protocol, bool IsEncrypted)
    {
        OnDataReceive?.Invoke(client, data, protocol, IsEncrypted);
    }
}
