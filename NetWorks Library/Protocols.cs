using NetWorks.Network;

record ServerToClientProtocol(BaseClient Client)
{
    public record TextMessage(string Message)
    {
        public void Receive(ServerToClientProtocol protocol)
        {
            Console.WriteLine($"<C#{protocol.Client.Id} <- S> {Message}");
        }
    }
}

record ClientToServerProtocol(ServerClient Client, NetworkProtocol Protocol)
{
    public record TextMessage(string Message)
    {
        public void Receive(ClientToServerProtocol protocol)
        {
            Console.WriteLine($"<C#{protocol.Client.Id} -> S> {Message}");
        }
    }

    public record BinaryArray(byte[] Bytes)
    {
        public void Receive(ClientToServerProtocol protocol)
        {
            Console.WriteLine($"<C#{protocol.Client.Id} -> S> Received {Bytes.Length} bytes");
        }
    }
}