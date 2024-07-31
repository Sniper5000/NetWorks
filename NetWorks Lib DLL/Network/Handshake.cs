using MemoryPack;

[MemoryPackable]
partial record ServerHandshakeData(int ClientId, string PublicKey, int ServerUdpPort);
[MemoryPackable]
partial record ClientHandshakeData(string PublicKey, int ClientUdpPort);