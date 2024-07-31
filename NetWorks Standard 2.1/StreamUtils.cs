
// Power of laziness brings you https://stackoverflow.com/a/24412022
using System;
using System.IO;

public static class StreamUtils
{
    public static byte[] ReadExactly(this Stream stream, int count)
    {
        byte[] buffer = new byte[count];
        int offset = 0;
        while (offset < count)
        {
            int read = stream.Read(buffer, offset, count - offset);
            if (read == 0)
                throw new EndOfStreamException();
            offset += read;
        }
        System.Diagnostics.Debug.Assert(offset == count);
        return buffer;
    }

    public static void CopyToWithProgress(this Stream from, Stream to, Action<long> onProgress, int bufferSize = 81920)
    {
        byte[] buffer = new byte[bufferSize];
        int readAmount;
        int totalAmount = 0;

        while((readAmount = from.Read(buffer, 0, buffer.Length)) != 0)
        {
            to.Write(buffer, 0, readAmount);
            totalAmount += readAmount;
            onProgress(totalAmount);
        }
    }

    public static void SendByteArray(Stream stream, byte[] bytes)
    {
        stream.Write(BitConverter.GetBytes(bytes.Length));
        stream.Write(bytes);
    }

    public static byte[] ReceiveByteArray(Stream stream)
    {
        int length = BitConverter.ToInt32(stream.ReadExactly(4));
        return stream.ReadExactly(length);
    }
}