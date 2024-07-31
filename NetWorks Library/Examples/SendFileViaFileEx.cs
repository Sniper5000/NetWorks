using System.Net;
using NetWorks.FileEx;
using NetWorks.Security;
using NetWorks_Lib_DLL.FileEx;
using NetWorks_Library.FileGenerator;

namespace NetWorks_Library.Examples
{
    /// <summary>
    /// Showcases how to send a file via FileEx, both with no encryption and with encryption.
    /// </summary>
    internal class SendFileViaFileEx
    {
        /// <summary>
        /// The endpoint defines over what IP & port the data should be transmitted.
        /// We will use localhost for the purposes of this example.
        /// </summary>
        readonly IPEndPoint endPoint = new(IPAddress.Parse("127.0.0.1"), 9999);

        /// <summary>
        /// To encrypt the exchanged data, we make use of a SecurityKeypair.
        /// This class stores a matching set of public and private keys, which
        /// are used by the sender and receiver to encrypt and decrypt the
        /// exchanged data, respectively.
        /// </summary>
        readonly SecurityKeypair exchangeKeypair = new();

        public void Run()
        {
            /// For this example we will send garbage, but in your case
            /// you can send any meaningful data (if you have any.)
            byte[] data = UselessFileGenerator.RandomBytes(16);
            Console.WriteLine($"The data to be sent is: {BitConverter.ToString(data)}");

            var task = Task.Run(RunReceive);
            
            /// Despite the name of the class, FileEx actually works with streams,
            /// not just files. You can send any stream you have over FileEx,
            /// as long as the end of the stream matches the end of the data.
            using var dataStream = new MemoryStream(data);

            Console.WriteLine("Sending a non-encrypted file over FileEx.");
            dataStream.Seek(0, SeekOrigin.Begin);
            FileEx.SendStream(dataStream, endPoint);

            Console.WriteLine("Sending an encrypted file over FileEx.");
            dataStream.Seek(0, SeekOrigin.Begin);
            FileEx.SendStreamEncrypted(dataStream, endPoint, exchangeKeypair.PublicKey);

            task.Wait();
            Console.WriteLine("The files have been successfully sent.");
        }
        
        private void RunReceive()
        {
            using MemoryStream outputStream = new();

            Console.WriteLine("Receiving a non-encrypted file over FileEx.");
            outputStream.SetLength(0);
            FileEx.ReceiveStream(outputStream, endPoint);
            Console.WriteLine($"Received data: {BitConverter.ToString(outputStream.ToArray())}");

            Console.WriteLine("Receiving an encrypted file over FileEx.");
            outputStream.SetLength(0);
            FileEx.ReceiveStreamEncrypted(outputStream, endPoint, exchangeKeypair.PrivateKey);
            Console.WriteLine($"Received data: {BitConverter.ToString(outputStream.ToArray())}");
        }
    }
}
