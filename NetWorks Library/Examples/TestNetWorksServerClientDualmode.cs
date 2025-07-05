using NetWorks.Network;
using NetWorks_Library.FileGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetWorks_Library.Examples
{
    /// <summary>
    /// This example is for "Evented Server & Client" use.
    /// Notice: All methods fired by the Evented Server & Client might run on different threads. /!/
    /// Encryption increases packet size! It must be used when necessary only and shouldn't be used with UDP
    /// 65502 bytes being sent via UDP (Encryption off) is possible while still not recommended.. (not a single byte more)
    /// </summary>
    internal class TestNetWorksServerClientDualmode
    {
        private EventedClient Client;
        private EventedClient ClientIPv6;
        private EventedServer Server;
        private byte[] TestData;

        public void Run()
        {
            Console.WriteLine("Initiating...");
            /// For this example we will send garbage, but in your case
            /// you can send any meaningful data (if you have any.)
            //1MB of useless data for TCP test below
            try
            {
                TestData = UselessFileGenerator.RandomBytes(1048576); //65502 (UDP MAX allowed test) //Avoid using large values as this test is using RAM, Not file streams
            }
            catch 
            {
                return;
            }
            Console.WriteLine($"Starting Dualmode Server...");
            //Start Server
            Server = new();
            Server.OnDataReceive += Received; //When data is received, this method will be fired.
            Server.OnClientReady += Server_OnClientReady;
            Server.OnClientLeave += Server_OnClientLeave;
            //NOTE: RUN the server on a separate thread.. NEVER run it on the main thread as it will hang.
            Task.Run(() => Server.RunDual(9999));

            Console.WriteLine($"Starting IPv4 Client...");
            //Start Client & Connect!
            Client = new();
            Client.OnDataReceive += Client_OnDataReceive;
            Client.OnClientReady += Client_OnClientReady;
            Client.OnClientLeave += Client_OnClientLeave;
            //NOTE: RUN the client on a separate thread.. Just like the server, not doing so will HANG the main thread.
            Task.Run(() => Client.Connect("127.0.0.1", 9999));

            Console.WriteLine($"Starting IPv6 Client...");
            //Start IPv6 Client & Connect!
            ClientIPv6 = new();
            ClientIPv6.OnDataReceive += Client1_OnDataReceive;
            ClientIPv6.OnClientReady += Client1_OnClientReady;
            ClientIPv6.OnClientLeave += Client1_OnClientLeave;
            //NOTE: RUN the client on a separate thread.. Just like the server, not doing so will HANG the main thread.
            Task.Run(() => Client.Connect("::1", 9999));

            //Wait until the client is ready. Usually this would run inside "Client_OnClientReady()" instead of sleeping the main thread.
            Thread.Sleep(2000);
            //Try sending some data! after the client has connected.
            Client.Send(Encoding.UTF8.GetBytes("Hello World"), NetworkProtocol.TCP, true);
            ClientIPv6.Send(Encoding.UTF8.GetBytes("Hello World"), NetworkProtocol.TCP, true);
            Client.Send(Encoding.UTF8.GetBytes("Some Raw Text"), NetworkProtocol.TCP, false);
            ClientIPv6.Send(Encoding.UTF8.GetBytes("Some Raw Text"), NetworkProtocol.TCP, false);

            //Testing TCP
            for (int i = 0; i < 5; i++)
            {
                Client.Send(Encoding.UTF8.GetBytes($"Repeating... x{i + 1}"), NetworkProtocol.TCP, true);
                ClientIPv6.Send(Encoding.UTF8.GetBytes($"Repeating... x{i + 1}"), NetworkProtocol.TCP, true);
            }

            //Testing UDP
            for (int i = 0; i < 5; i++)
            {
                Client.Send(Encoding.UTF8.GetBytes($"Repeating... x{i + 1}"), NetworkProtocol.UDP, true);
                ClientIPv6.Send(Encoding.UTF8.GetBytes($"Repeating... x{i + 1}"), NetworkProtocol.UDP, true);
            }

            Thread.Sleep(2000);
            Console.WriteLine($"[Client] Attempting to send : {Convert.ToHexString(SHA512.HashData(TestData))}");

            //Do not use UDP for sending more than 65,502 bytes (Encryption increases bytes usage) as that's the UDP limit, 
            //if large amounts of bytes will be sent.. consider using TCP instead
            Client.Send(TestData, NetworkProtocol.TCP, true); //Sending 1MB of useless data encrypted
            ClientIPv6.Send(TestData, NetworkProtocol.TCP, true); //Sending 1MB of useless data encrypted

            Thread.Sleep(1000);
            Console.WriteLine($"Shutting down...");
            Client.Disconnect();
            ClientIPv6.Disconnect();
            Thread.Sleep(1000);
            Server.Shutdown();
            Console.WriteLine($"Test Complete!");
            //GC.Collect();
        }        
        
        //CLIENT

        /// <summary>
        /// Executed after the client has disconnected from the server
        /// </summary>
        private void Client_OnClientLeave()
        {
            Console.WriteLine($"[Client] Client has disconnected!");
        }

        /// <summary>
        /// Executed after the client has connected to the server.
        /// </summary>
        private void Client_OnClientReady()
        {
            Console.WriteLine($"[Client] Client is ready!");
        }

        /// <summary>
        /// Executed after data sent by the server is received.
        /// </summary>
        /// <param name="Data"> <see cref="byte"/>[] received.</param>
        /// <param name="Protocol"> <see cref="NetworkProtocol"/> used for reception.</param>
        /// <param name="Encrypted"></param>
        private void Client_OnDataReceive(byte[] Data, NetworkProtocol Protocol, bool Encrypted)
        {
            //Clients can only send data to the server.
            //NetWorks automatically decrypts data received from the server..
            Console.WriteLine($"[Client] Data Received Encrypted? {Encrypted} | Protocol {Protocol.ToString()}: {Encoding.UTF8.GetString(Data)}");
        }

        //CLIENT IPv6

        /// <summary>
        /// Executed after the client has disconnected from the server
        /// </summary>
        private void Client1_OnClientLeave()
        {
            Console.WriteLine($"[Client - IPv6] Client has disconnected!");
        }

        /// <summary>
        /// Executed after the client has connected to the server.
        /// </summary>
        private void Client1_OnClientReady()
        {
            Console.WriteLine($"[Client - IPv6] Client is ready!");
        }

        /// <summary>
        /// Executed after data sent by the server is received.
        /// </summary>
        /// <param name="Data"> <see cref="byte"/>[] received.</param>
        /// <param name="Protocol"> <see cref="NetworkProtocol"/> used for reception.</param>
        /// <param name="Encrypted"></param>
        private void Client1_OnDataReceive(byte[] Data, NetworkProtocol Protocol, bool Encrypted)
        {
            //Clients can only send data to the server.
            //NetWorks automatically decrypts data received from the server..
            Console.WriteLine($"[Client - IPv6] Data Received Encrypted? {Encrypted} | Protocol {Protocol.ToString()}: {Encoding.UTF8.GetString(Data)}");
        }

        //SERVER

        /// <summary>
        /// Executed after a client has disconnected.
        /// </summary>
        /// <param name="Client"> <see cref="ServerClient"/> Client that has disconnected.</param>
        private void Server_OnClientLeave(ServerClient Client)
        {
            Console.WriteLine($"[Server] Client {Client.Id} has disconnected!");
            //The object is provided, in case it's identifier was being used by code.
            //at this point, the client is disconnected.. sending data will fail.

            //RemovePlayer(Client); 
        }

        /// <summary>
        /// Executed after a client has connected and is ready.
        /// </summary>
        /// <param name="Client"> <see cref="ServerClient"/> Client that has connected.</param>
        private void Server_OnClientReady(ServerClient Client)
        {
            Console.WriteLine($"[Server] Client {Client.Id} has connected!");

            //Using ServerClient object, data can be sent to them
            Client.Send(Encoding.UTF8.GetBytes("[From Server] You've connected successfully."), NetworkProtocol.TCP, true);
            //This object is used until the client disconnects..

            //AddPlayer(Client);
        }

        /// <summary>
        /// Executed when ANY client sends data to the server.
        /// </summary>
        /// <param name="Client"> <see cref="ServerClient"/> Client that sent the data.</param>
        /// <param name="Data"> <see cref="byte"/>[] received.</param>
        /// <param name="Protocol"> <see cref="NetworkProtocol"/> used for reception.</param>
        /// <param name="Encrypted"> <see cref="bool"/> whether the data was encrypted.</param>
        private void Received(ServerClient Client, byte[] Data, NetworkProtocol Protocol, bool Encrypted)
        {
            //NetWorks automatically decrypts data received from clients..

            //To prevent displaying a large string, instead display the Hash
            if(Data.Length > 4096)
            {
                var Hash = Convert.ToHexString(SHA512.HashData(Data));
                Console.WriteLine($"[Server] Data Received Encrypted? {Encrypted} | Protocol {Protocol.ToString()}: {Hash}");

                Console.WriteLine($"[C] Hash {Convert.ToHexString(SHA512.HashData(TestData))}");
                Console.WriteLine($"[S] Hash {Hash}");
                Console.WriteLine($"Same Hash? {String.CompareOrdinal(Convert.ToHexString(SHA512.HashData(TestData)), Hash) == 0}");
            }
            else
                Console.WriteLine($"[Server] Data Received Encrypted? {Encrypted} | Protocol {Protocol.ToString()}: {Encoding.UTF8.GetString(Data)}");
            
        }
    }
}
