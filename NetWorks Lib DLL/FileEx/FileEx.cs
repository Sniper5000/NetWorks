using System.IO;
using System.Net;
using System.Net.Sockets;
using NetWorks.FileEx;
using NetWorks.Network;
using NetWorks.Security;

namespace NetWorks_Lib_DLL.FileEx
{
    /// <summary>
    /// The FileEx class provides methods for easily sharing large amounts of data, 
    /// with encryption supported.
    /// </summary>
    public static class FileEx
    {

        /// <summary>
        /// Send all of the data in the stream to the specified endpoint.
        /// </summary>
        /// <param name="dataStream">The stream containing all the data to be sent.</param>
        /// <param name="endPoint">The remote endpoint to deliver the data to.</param>
        public static void SendStream(Stream dataStream, IPEndPoint endPoint)
        {
            using TcpClient tcpClient = new();
            tcpClient.Connect(endPoint);
            Stream outputStream = tcpClient.GetStream();
            dataStream.CopyTo(outputStream);
        }

        /// <summary>
        /// Send all of the data in the stream to the specified endpoint,
        /// encrypted using the provided public key.
        /// </summary>
        /// <param name="dataStream">The stream containing all the data to be sent.</param>
        /// <param name="endPoint">The remote endpoint to deliver the data to.</param>
        /// <param name="publicKey">The key to encrypt the data with.</param>
        public static void SendStreamEncrypted(Stream dataStream, IPEndPoint endPoint, SecurityKey publicKey)
        {
            using TcpClient tcpClient = new();
            tcpClient.Connect(endPoint);
            Stream outputStream = tcpClient.GetStream();
            SecureDataSender sender = new(outputStream, publicKey);
            sender.SendStream(dataStream);
        }

        /// <summary>
        /// Read a single file from the endpoint into the output stream.
        /// </summary>
        /// <param name="outputStream">The stream where the received data will be written to.</param>
        /// <param name="endPoint">The remote endpoint to receive data from.</param>
        public static void ReceiveStream(Stream outputStream, IPEndPoint endPoint)
        {
            TcpListener tcpListener = new(endPoint);
            tcpListener.Start();
            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            Stream inputStream = tcpClient.GetStream();
            inputStream.CopyTo(outputStream);
            tcpListener.Stop();
        }

        /// <summary>
        /// Read a single file from the endpoint into the output stream,
        /// decrypting the data using the provided private key.
        /// </summary>
        /// <param name="outputStream">The stream where the received data will be written to.</param>
        /// <param name="endPoint">The remote endpoint to receive data from.</param>
        /// <param name="privateKey">The key to decrypt the data with.</param>
        public static void ReceiveStreamEncrypted(Stream outputStream, IPEndPoint endPoint, SecurityKey privateKey)
        {
            TcpListener tcpListener = new(endPoint);
            tcpListener.Start();
            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            Stream inputStream = tcpClient.GetStream();
            SecureDataReceiver receiver = new(inputStream, privateKey);
            receiver.ReceiveStream(outputStream);
            tcpListener.Stop();
        }
    }
}
