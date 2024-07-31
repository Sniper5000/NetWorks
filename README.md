# NetWorks
C# Networking library with encryption support, allowing developers to quickly setup a server and client. with Unity IL2CPP support.

## Quick Start [WIP]

To quickly create a custom client, inherit BaseClient and override BaseClient methods with your own.
```
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
```


[WIP]: Missing Unity examples and additional documentation.


<a href='https://ko-fi.com/A0A110TLP9' target='_blank'><img height='36' style='border:0px;height:36px;' src='https://storage.ko-fi.com/cdn/kofi2.png?v=3' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a>
