using NetWorks.Security;
using System.IO;

namespace NetWorks.Network
{
    /// <summary>
    /// Encrypts data stream ready to send
    /// </summary>
    public class SecureDataSender
    {
        private readonly Stream outputStream;
        private readonly SecurityKey publicKey;
        private EncryptionAedmStream? aedmStream;
        private int BufferSize;
        public bool UseEncryption;

        public SecureDataSender(Stream outputStream, SecurityKey publicKey, int BufferSize = 8 * 1024)
        {
            this.outputStream = outputStream;
            this.publicKey = publicKey;
            this.BufferSize = BufferSize;
        }

        public void SendStream(Stream dataStream)
        {
            DelimitedOutputStream delimitedOutputStream = new DelimitedOutputStream(outputStream, BufferSize);

            if (!UseEncryption)
            {
                dataStream.CopyTo(delimitedOutputStream);
            }
            else
            {
                aedmStream = EncryptionAedmStream.SetupEncryption(publicKey, delimitedOutputStream);
                dataStream.CopyTo(aedmStream);
                aedmStream.FlushFinalBlock();
            }

            delimitedOutputStream.Close();
        }
    }
}