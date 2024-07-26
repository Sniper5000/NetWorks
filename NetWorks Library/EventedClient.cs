using NetWorks.Network;

class EventedClient : BaseClient
{
    public event Action? OnClientReady;
    public event Action? OnClientLeave;
    public event Action<byte[], NetworkProtocol, bool>? OnDataReceive;

    public override void ClientReady()
    {
        OnClientReady?.Invoke();
    }

    public override void ClientLeave()
    {
        OnClientLeave?.Invoke();
    }

    public override void DataReceived(byte[] data, NetworkProtocol protocol, bool IsEncrypted = false)
    {
        OnDataReceive?.Invoke(data, protocol, IsEncrypted);
    }
}