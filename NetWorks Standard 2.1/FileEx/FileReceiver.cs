using System;
using System.IO;
using System.Text;
using NetWorks.Network;
using NetWorks.Security;
namespace NetWorks.FileEx
{

    public class FileReceiver
    {
        private readonly SecureDataReceiver dataReceiver;
        private int BufferSize;
        public Action<long, long>? DataAmountUpdated;

        public FileReceiver(Stream inputStream, SecurityKey privateKey, int bufferSize = 8 * 1024)
        {
            dataReceiver = new SecureDataReceiver(inputStream, privateKey);
            BufferSize = bufferSize;
        }

        /// <summary>
        /// Receives a single file and writes it to the specified directory
        /// </summary>
        /// <param name="directory"><see cref="string"/> File save location</param>
        public void ReceiveFile(string directory, out int tag)
        {
            dataReceiver.DataAmountUpdated = null;
            dataReceiver.IsEncrypted = false;
            byte[] header = dataReceiver.ReceiveData();
            using MemoryStream headerStream = new MemoryStream(header);
            long fileSize = BitConverter.ToInt64(headerStream.ReadExactly(8));
            tag = BitConverter.ToInt32(headerStream.ReadExactly(4));
            bool encrypted = headerStream.ReadByte() == 1;
            int filenameLen = BitConverter.ToInt32(headerStream.ReadExactly(4));
            string filename = Encoding.ASCII.GetString(headerStream.ReadExactly(filenameLen));
            string outputPath = Path.Join(directory, filename);
            using FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.Read, BufferSize);

            dataReceiver.DataAmountUpdated = amount => DataAmountUpdated?.Invoke(amount, fileSize);
            dataReceiver.IsEncrypted = encrypted;
            dataReceiver.ReceiveStream(fileStream);
        }
        /// <summary>
        /// Receives a single file and writes it to the specified <see cref="Stream"/>
        /// </summary>
        /// <param name="outputStream"><see cref="Stream"/> to write in</param>
        /// <param name="filename">Received <see cref="string"/> file name</param>
        /// <param name="tag">Received <see cref="int"/> tag</param>
        public void ReceiveFile(Stream outputStream, out string filename, out int tag)
        {
            dataReceiver.DataAmountUpdated = null;
            dataReceiver.IsEncrypted = false;
            byte[] header = dataReceiver.ReceiveData();
            using MemoryStream headerStream = new MemoryStream(header);
            long fileSize = BitConverter.ToInt64(headerStream.ReadExactly(8));
            tag = BitConverter.ToInt32(headerStream.ReadExactly(4));
            bool encrypted = headerStream.ReadByte() == 1;
            int filenameLen = BitConverter.ToInt32(headerStream.ReadExactly(4));
            filename = Encoding.ASCII.GetString(headerStream.ReadExactly(filenameLen));

            dataReceiver.DataAmountUpdated = amount => DataAmountUpdated?.Invoke(amount, fileSize);
            dataReceiver.IsEncrypted = encrypted;
            dataReceiver.ReceiveStream(outputStream);
        }
        /// <summary>
        /// Receives a single file
        /// </summary>
        /// <param name="filename">Received <see cref="string"/> filename</param>
        /// <param name="tag">Received <see cref="int"/> tag</param>
        /// <returns>Received <see cref="byte"/>[]</returns>
        public byte[] ReceiveFile(out string filename, out int tag)
        {
            using MemoryStream fileContents = new MemoryStream();
            ReceiveFile(fileContents, out filename, out tag);
            return fileContents.ToArray();
        }
    }
}