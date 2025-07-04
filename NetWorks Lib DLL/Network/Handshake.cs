using MemoryPack;

[MemoryPackable]
partial record ServerHandshakeData(int ClientId, string PublicKey, int ServerUdpPort, string Version);
[MemoryPackable]
partial record ClientHandshakeData(string PublicKey, int ClientUdpPort, string Version);