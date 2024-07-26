using NetWorks.FileEx;
using NetWorks.Network;
using NetWorks.Security;
using NetWorks_Library.FileGenerator;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class FileExTest
{
    private const int mainServerPort = 9999;

    public void Run()
    {
        
        Server server = new();
        Task.Run(() => server.Run("127.0.0.1", mainServerPort));
        Client client = new();
        client.Connect("localhost", mainServerPort);
    }

    private class Server : BaseServer
    {
        private readonly Dictionary<int, ServerClient> clients = new();
        private int fileExPort;

        public Server()
        {
            Task.Run(ListenFileEx);
        }

        public override void ClientReady(ServerClient client)
        {
            clients.Add(client.Id, client);
            Console.WriteLine("<S> Client connected!");
            client.Send(Encoding.ASCII.GetBytes("port " + fileExPort));
        }

        public override void ClientLeave(ServerClient client)
        {
            clients.Remove(client.Id);
        }

        public void ClientConnectFileEx(int clientId, FileExClient fileExClient)
        {
            //Check if the file exists! else, create it
            if (!File.Exists("Files/TestFile.Useless"))
            {
                //Generate!
                Console.WriteLine("Please input the file size in MB");
                long Response = long.Parse(Console.ReadLine() ?? throw new NullReferenceException());
                if (Response * 1048576 > 1073741824)
                {
                    Console.WriteLine($"Your desired file size {Response} exceeds the 1GB limit and thus has been set to 1GB");
                    Response = 1024;
                }

                Console.WriteLine("Generating file..");
                UselessFileGenerator.CreateUselessFile("Files/TestFile.Useless", Response * 1048576);
                Console.WriteLine("Complete!");
            }
            fileExClient.StreamFile("Files/TestFile.Useless", encrypt: true);
        }

        private void ListenFileEx()
        {   
            int connectionTimeout = 10000;
            int receiveTimeout = 10000;

            TcpListener listener = new(IPAddress.Parse("127.0.0.1"), 0);
            listener.Start();
            fileExPort = (listener.LocalEndpoint as IPEndPoint ?? throw new NullReferenceException()).Port;

            Task.Run(() => 
            {   
                while(true)
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();

                    Task.Run(() => 
                    {
                        // This protocol is very "unique" to our application (this specific test in this case)
                        // since we're making choices such as "exchange server-dependant id" or the challenge we pick
                        // so we could put it in its own class if we wanted to reuse it, but it prob.
                        // doesnt belong in the FileEx class
                        tcpClient.ReceiveTimeout = connectionTimeout;
                        int clientId = BitConverter.ToInt32(tcpClient.GetStream().ReadExactly(4));
                        SecurityKey remotePublicKey = GetClientSecurityKey(clientId);
                        SecurityKey localPrivateKey = GetServerClientPrivateKey(clientId);
                        if(!SecurityChallenge.PerformChallengeServer(tcpClient, remotePublicKey)) return;
                        tcpClient.ReceiveTimeout = receiveTimeout;
                        ClientConnectFileEx(clientId, new(tcpClient, remotePublicKey, localPrivateKey));
                    });
                }
            });
        }

        public SecurityKey GetClientSecurityKey(int clientId)
        {
            return clients[clientId].ClientPublicKey;
        }

        public SecurityKey GetServerClientPrivateKey(int clientId)
        {
            return clients[clientId].Keys.PrivateKey;
        }
    }

    private class Client : BaseClient
    {
        public override void DataReceived(byte[] data, NetworkProtocol protocol, bool IsEncrypted = false)
        {
            string message = Encoding.ASCII.GetString(data);
            var parts = message.Split(" ");
            
            switch(parts[0])
            {
                case "port":
                    ConnectFileEx(int.Parse(parts[1]));
                    break;
            }
        }

        private void ConnectFileEx(int port)
        {
            IPEndPoint endPoint = new(IPAddress.Parse("127.0.0.1"), port);
            TcpClient tcpClient = new();
            tcpClient.Connect(endPoint);
            tcpClient.GetStream().Write(BitConverter.GetBytes(Id));
            // The server already knows our public key :D
            SecurityChallenge.PerformChallengeClient(tcpClient, Keys.PrivateKey);
            SecurityKey remotePublicKey = ServerPublicKey ?? throw new NullReferenceException();
            FileExClient client = new(tcpClient, Keys.PrivateKey, remotePublicKey);

            using MemoryStream fileContents = new();
            client.ReceiveFile(fileContents, out string filename, out int tag, (read, size) =>
            {
                Console.WriteLine($"Received {read}/{size} bytes ({(read*100f)/size}%)");
            });
            Console.WriteLine($"<C> Received {fileContents.Length} bytes {filename} {tag}");
        }
    }
}