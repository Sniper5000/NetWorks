using NetWorks.Security;
using System;
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
        public Action<long>? DataAmountUpdated;

        public SecureDataSender(Stream outputStream, SecurityKey publicKey, int BufferSize = 8 * 1024)
        {
            this.outputStream = outputStream;
            this.publicKey = publicKey;
            this.BufferSize = BufferSize;
        }

        public void SendStream(Stream dataStream)
        {
            DelimitedOutputStream delimitedOutputStream = new(outputStream, BufferSize);

            if (!UseEncryption)
            {
                //dataStream.CopyTo(delimitedOutputStream);
                CopyTo(dataStream, delimitedOutputStream);
            }
            else
            {
                aedmStream = EncryptionAedmStream.SetupEncryption(publicKey, delimitedOutputStream);
                //dataStream.CopyTo(aedmStream
                CopyTo(dataStream, aedmStream);
                aedmStream.FlushFinalBlock();
            }

            delimitedOutputStream.Close();
        }

        private void CopyTo(Stream from, Stream to)
        {
            from.CopyToWithProgress(to, amount => DataAmountUpdated?.Invoke(amount));
        }
    }
}