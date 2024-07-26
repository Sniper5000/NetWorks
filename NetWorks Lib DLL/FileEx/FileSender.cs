using System.Text;
using NetWorks.Network;
using NetWorks.Security;
namespace NetWorks.FileEx;

public class FileSender
{
    private readonly SecureDataSender dataSender;

    public FileSender(Stream outputStream, SecurityKey publicKey, int BufferSize = 8 * 1024)
    {
        dataSender = new(outputStream, publicKey, BufferSize);
    }
    /// <summary>
    /// Sends a file from <see cref="string"/> path
    /// </summary>
    /// <param name="path"><see cref="string"/> file path</param>
    /// <param name="tag"><see cref="int"/> tag</param>
    /// <param name="encrypted"><see cref="bool"/> encrypt file?</param>
    public void SendFile(string path, int tag = -1, bool encrypted = false)
    {
        using FileStream fileStream = new(path, FileMode.Open);
        string filename = Path.GetFileName(path);
        SendFile(fileStream, Encoding.ASCII.GetBytes(filename), tag, encrypted);
    }
    /// <summary>
    /// Sends a file from <see cref="Stream"/>
    /// </summary>
    /// <param name="fileContentsStream"><see cref="Stream"/> file content</param>
    /// <param name="filename"><see cref="byte"/>[] file name</param>
    /// <param name="tag"><see cref="int"/> tag</param>
    /// <param name="encrypted"><see cref="bool"/> Encrypt?</param>
    private void SendFile(Stream fileContentsStream, byte[] filename, int tag, bool encrypted)
    {
        using MemoryStream headerStream = new();
        headerStream.Write(BitConverter.GetBytes(fileContentsStream.Length));
        headerStream.Write(BitConverter.GetBytes(tag));
        headerStream.WriteByte((byte)(encrypted ? 1 : 0));
        headerStream.Write(BitConverter.GetBytes(filename.Length));
        headerStream.Write(filename);
        headerStream.Seek(0, SeekOrigin.Begin);
        // TODO this is not required
        dataSender.UseEncryption = false;
        dataSender.SendStream(headerStream);
        dataSender.UseEncryption = encrypted;
        dataSender.SendStream(fileContentsStream);
    }
}