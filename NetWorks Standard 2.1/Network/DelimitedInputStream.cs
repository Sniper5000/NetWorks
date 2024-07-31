using System;
using System.IO;

public class DelimitedInputStream : Stream
{
    private readonly Stream stream;
    private int dataLeft;
    private bool endOfStream;
    
    public DelimitedInputStream(Stream stream)
    {
        this.stream = stream;
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => stream.Length;
    public override long Position { get => stream.Position; set => throw new NotSupportedException(); }
    public override void Flush() { }
    public override int Read(byte[] buffer, int offset, int count)
    {
        lock (stream)
        {
            var P = stream.CanRead;
            if (endOfStream)
                return 0;

            if (dataLeft == 0)
            {
                dataLeft = BitConverter.ToInt32(stream.ReadExactly(4));
                if (dataLeft == 0)
                {
                    endOfStream = true;
                    return 0;
                }
            }
            //Console.WriteLine($"Data left {dataLeft}");
            int readAmount = Math.Min(count, dataLeft);
            stream.Read(buffer, offset, readAmount);
            dataLeft -= readAmount;
            return readAmount;
        }
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}