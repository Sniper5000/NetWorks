# NetWorks
C# Networking library with encryption support, allowing developers to quickly setup a server and client. with Unity IL2CPP support.

## Installation
1. Download the source code or .dll
2. Add a reference on your C# project for NetWorks.dll
3. Verify by using NetWorks
```
using NetWorks.Network;
```


## Quick Start [WIP]

### Client
To quickly create a custom client, inherit BaseClient and override BaseClient methods with your own.

```
using NetWorks.Network;

class EventedClient : BaseClient
{
    public event Action? OnClientReady;
    public event Action? OnClientLeave;
    public event Action<byte[], NetworkProtocol, bool>? OnDataReceive;

    //When the client has successfully connected to the server.
    public override void ClientReady()
    {
        OnClientReady?.Invoke();
    }
    //When the client disconnects or lost connection, this will fire.
    public override void ClientLeave()
    {
        OnClientLeave?.Invoke();
    }
    //data = byte[] to send to the connected server
    //protocol = determines the whether TCP, UDP should be used to send
    //IsEncrypted = whether this data should be encrypted(AES + RSA) before sending or not.
    public override void DataReceived(byte[] data, NetworkProtocol protocol, bool IsEncrypted = false)
    {
        OnDataReceive?.Invoke(data, protocol, IsEncrypted);
    }
}
```
Instantiate your client, and connect to your server program by providing it an IP & Port
```
//Example
var Client = new EventedClient(); //Change it to your custom client class.
Task.Run(() => 
{ 
    Client.Connect("127.0.0.1", 7777); 
});
```
Once the client is ready, data can be sent by using
```
//Client will send the byte[] using the protocol specified and encryption (if true)
Client.Send(Data, protocol, encrypted); //Data is a byte[], protocol (NetworkProtocol.TCP or NetworkProtocol.UDP), Encrypted (true or false)
```

### Server
To create a custom server, inherit from BaseServer and override BaseServer methods with your own.
```
using NetWorks.Network;

class EventedServer : BaseServer
{
    public event Action<ServerClient>? OnClientReady;
    public event Action<ServerClient>? OnClientLeave;
    public event Action<ServerClient, byte[], NetworkProtocol, bool>? OnDataReceive;

    //When a client has successfully connected, this will be fired.
    //client = client that has connected.
    public override void ClientReady(ServerClient client)
    {
        OnClientReady?.Invoke(client);
    }
    //When a client has disconnected, this will be fired.
    //client = client that has disconnected.
    public override void ClientLeave(ServerClient client)
    {
        OnClientLeave?.Invoke(client);
    }
    //When data has been fully received from a client, this will be fired.
    //client = client that sent the data
    //data = byte[] received
    //protocol = TCP or UDP
    //IsEncrypted = Whether the data received was encrypted with AES + RSA
    public override void DataReceived(ServerClient client, byte[] data, NetworkProtocol protocol, bool IsEncrypted)
    {
        OnDataReceive?.Invoke(client, data, protocol, IsEncrypted);
    }
}
```
Instantiate your server and run the listener on another thread. (Otherwise the main thread will hang)
```
//Example
var Listener = new EventedServer(); //Change it to your custom server class.
Task.Run(() => Listener.Run("127.0.0.1", 7777));
```
Note: NetWorks runs on multiple threads, as such methods fired by NetWorks will not be running on the main thread.

## Examples

All standalone examples using NetWorks library can be found here: 
https://github.com/Sniper5000/NetWorks/tree/main/NetWorks%20Library

## Unity
Note: NetWorks runs on multiple threads, as such methods fired by NetWorks will not be running on the main thread.
1. Download NetWorks Standard 2.1 source code or NetWorks(Unity) DLL for both Server & Client.
    NetWorks Standard 2.1: https://github.com/Sniper5000/NetWorks/tree/main/NetWorks%20Standard%202.1
2. Drag the .dll or source code to your Unity project.
3. Start using NetWorks.

If it's required to run a method on the main thread from an event fired by NetWorks.

Create a C# Monobehaviour script, Use a custom name or the one used below "RunMainThread"
```
using System.Collections.Concurrent;
using System;
using UnityEngine;

//If you have used a custom name, change the class name to your custom one
public class RunMainThread : MonoBehaviour
{
    private static ConcurrentQueue<Action> RunOnMainThread = new();

    // Either FixedUpdate or Update will work
    void Update()
    {
        if (!RunOnMainThread.IsEmpty)
        {
            lock (RunOnMainThread)
            {
                while (RunOnMainThread.TryDequeue(out var action))
                {
                    action?.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Runs <see cref="Action"/> on the Main Thread.
    /// </summary>
    /// <param name="action">Action to execute</param>
    public static void Enqueue(Action action)
    {
        RunOnMainThread.Enqueue(action);
    }
}
```
After the script is done, drag it into an empty GameObject. (Keep only 1 instance active)

To run a method on the main thread use
```
RunMainThread.Enqueue(() => SomeMethod());
```

[WIP]: Missing Unity examples and additional documentation.


<a href='https://ko-fi.com/A0A110TLP9' target='_blank'><img height='36' style='border:0px;height:36px;' src='https://storage.ko-fi.com/cdn/kofi2.png?v=3' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a>
