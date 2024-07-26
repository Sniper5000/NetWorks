public class DelimitedOutputStream : Stream
{
    private readonly Stream stream;
    private readonly byte[] window;
    private int windowStoredAmount;
    
    public DelimitedOutputStream(Stream stream, int windowSize = 8 * 1024)
    {
        this.stream = stream;
        window = new byte[windowSize];
    }

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => stream.Length;
    public override long Position { get => stream.Position; set => throw new NotSupportedException(); }

    public override void Flush()
    {
        if(windowStoredAmount != 0)
        {
            stream.Write(BitConverter.GetBytes(windowStoredAmount));
            stream.Write(window, 0, windowStoredAmount);
            windowStoredAmount = 0;
        }

        stream.Flush();
    }

    public override void Close()
    {
        Flush();
        stream.Write(BitConverter.GetBytes(0));
        stream.Flush();
        base.Close();
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => stream.SetLength(value);
    public override void Write(byte[] buffer, int offset, int count)
    {
        while(count > 0)
        {
            int start = windowStoredAmount;
            int end = Math.Min(windowStoredAmount + count, window.Length);
            int size = end - start;
            Array.Copy(buffer, offset, window, start, size);
            windowStoredAmount += size;
            if(windowStoredAmount == window.Length) Flush();
            offset += size;
            count -= size;
        }
    }
}