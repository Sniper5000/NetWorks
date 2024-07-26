record ServerHandshakeData(int ClientId, string PublicKey, int ServerUdpPort);
record ClientHandshakeData(string PublicKey, int ClientUdpPort);