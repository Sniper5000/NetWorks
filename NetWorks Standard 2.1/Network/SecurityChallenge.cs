using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using MathThreader;

namespace NetWorks.Security
{
    /// <summary>
    /// Verifies that a client has a valid Decryption/Encryption stream.
    /// If a client fails the challenge, it's removed.
    /// If a client passes the challenge, it's ready.
    /// Failing the challenge usually means the public key exchange failed
    /// </summary>
    public static class SecurityChallenge
    {
        // CLIENT: connects to server
        // CLIENT: sends its LID
        // SERVER: receives LID
        // SERVER: converts LID to Public Key
        // SERVER: encrypts data using Public Key
        // SERVER: sends encrypted data to the client
        // CLIENT: receives encrypted data
        // CLIENT: decrypts it using its Private Key
        // CLIENT: sends the original data back to the server
        // SERVER: receives the original data
        // SERVER: verifies the two datas match
        // SERVER: has a verified client now, do whatever with it

        // TODO: dual verification isn't added yet
        public static void PerformChallengeClient(TcpClient tcpClient, SecurityKey privateKey)
        {
            Stream stream = tcpClient.GetStream();

            int encryptedKeySize = BitConverter.ToInt32(stream.ReadExactly(4));
            byte[] encryptedKey = stream.ReadExactly(encryptedKeySize);
            byte[] uniqueKey = AedmEncryption.Decrypt(privateKey, encryptedKey);
            stream.Write(uniqueKey);
        }

        public static bool PerformChallengeServer(TcpClient tcpClient, SecurityKey clientPublicKey)
        {
            Stream stream = tcpClient.GetStream();

            byte[] uniqueKey = Encoding.ASCII.GetBytes(MathHX.KeyGenerator.GetUniqueKey(30));
            byte[] encryptedKey = AedmEncryption.Encrypt(clientPublicKey, uniqueKey);
            stream.Write(BitConverter.GetBytes(encryptedKey.Length));
            stream.Write(encryptedKey);
            byte[] receivedKey = stream.ReadExactly(uniqueKey.Length);
            bool authSuccess = uniqueKey.SequenceEqual(receivedKey);

            if (!authSuccess) tcpClient.Close();
            return authSuccess;
        }
    }
}