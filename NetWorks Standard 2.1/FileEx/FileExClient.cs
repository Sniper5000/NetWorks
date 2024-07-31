using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetWorks.Security;

namespace NetWorks.FileEx
{

    /// <summary>
    /// FileEx allows sending/receiving large files via its own TCP connection using encryption.
    /// FileEx allows both, 1-way or 2-way file exchange.
    /// </summary>
    public class FileExClient
    {
        private readonly FileReceiver fileReceiver;
        private readonly FileSender fileSender;
        private static bool Listening;
        private static TcpListener listener;
        private static int Rx = 33554432;//32768;
        private static int Tx = 33554432;//32768;

        public FileExClient(TcpClient tcpClient, SecurityKey localPrivateKey, SecurityKey remotePublicKey)
        {
            fileReceiver = new FileReceiver(tcpClient.GetStream(), localPrivateKey, Rx);
            fileSender = new FileSender(tcpClient.GetStream(), remotePublicKey, Tx);
        }
        /// <summary>
        /// Streams a file from <see cref="string"/> path
        /// </summary>
        /// <param name="path"><see cref="string"/> File path</param>
        /// <param name="tag"><see cref="int"/> tag</param>
        /// <param name="encrypt"><see cref="bool"/> Encrypt stream?</param>
        public void StreamFile(string path, int tag = -1, bool encrypt = false)
        {
            fileSender.SendFile(path, tag, encrypt);
        }
        /// <summary>
        /// Receives a file from TCP stream and writes it on <see cref="Stream"/> dest
        /// </summary>
        /// <param name="dest"><see cref="Stream"/> Destination to write in</param>
        /// <param name="filename"><see cref="string"/> File name</param>
        /// <param name="tag"><see cref="int"/> Tag</param>
        /// <param name="handleProgress"><see cref="Action{T1, T2}"/> Progress handle</param>
        public void ReceiveFile(Stream dest, out string filename, out int tag, Action<long, long>? handleProgress = null)
        {
            fileReceiver.DataAmountUpdated = handleProgress;
            fileReceiver.ReceiveFile(dest, out filename, out tag);
        }

        /// <summary>
        /// Receives a file from TCP stream and writes it on dest
        /// </summary>
        /// <param name="dest"><see cref="string"/> Destination to write file in</param>
        /// <param name="filename"><see cref="string"/> File name</param>
        /// <param name="tag"><see cref="int"/> Tag</param>
        /// <param name="handleProgress"><see cref="Action{T1, T2}"/> Progress handle</param>
        public void ReceiveFile(string dest, out int tag, Action<long, long>? handleProgress = null)
        {
            fileReceiver.DataAmountUpdated = handleProgress;
            fileReceiver.ReceiveFile(dest, out tag);
        }
        /// <summary>
        /// Connect to <see cref="IPEndPoint"/> using TCP.
        /// </summary>
        /// <param name="endPoint"> Target IP address and port</param>
        /// <returns><see cref="FileExClient"/> client</returns>
        public static FileExClient DirectConnect(IPEndPoint endPoint)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.ReceiveBufferSize = Rx;
            tcpClient.SendBufferSize = Tx;
            tcpClient.Connect(endPoint);
            return DirectConnect(tcpClient);
        }
        /// <summary>
        /// Sends its public key to allow sending & receiving files.
        /// </summary>
        /// <param name="tcpClient"></param>
        /// <returns>Active <see cref="FileExClient"/></returns>
        private static FileExClient DirectConnect(TcpClient tcpClient)
        {
            SecurityKeypair keys = new SecurityKeypair();
            StreamUtils.SendByteArray(tcpClient.GetStream(), keys.PublicKey.Bytes);
            SecurityKey remotePublicKey = SecurityKey.FromBytes(StreamUtils.ReceiveByteArray(tcpClient.GetStream()));

            return new FileExClient(tcpClient, keys.PrivateKey, remotePublicKey);
        }
        /// <summary>
        /// Starts listening for connections
        /// </summary>
        /// <param name="endPoint"><see cref="IPEndPoint"/> to listen from</param>
        /// <param name="handleClient"></param>
        public static void DirectListen(IPEndPoint endPoint, Action<FileExClient> handleClient)
        {
            listener = new TcpListener(endPoint);
            listener.Start();
            Listening = true;
            while (Listening)
            {
                try
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    tcpClient.ReceiveBufferSize = Rx;
                    tcpClient.SendBufferSize = Tx;
                    Task.Run(() =>
                    {
                        FileExClient fileExClient = DirectConnect(tcpClient);
                        handleClient(fileExClient);
                    });
                }
                catch (Exception e) { }
            }
        }

        public static void Shutdown()
        {
            Listening = false;
            listener.Stop();
        }
    }
}