using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

public sealed class JSBrowserFile : IBrowserFile
{
    private readonly IJSRuntime _js;
    private readonly long _maxAllowedSize;

    public JSBrowserFile(
        IJSRuntime js,
        string name,
        long size,
        string contentType,
        DateTimeOffset lastModified,
        long maxAllowedSize = long.MaxValue)
    {
        _js = js;
        Name = name;
        Size = size;
        ContentType = contentType;
        LastModified = lastModified;
        _maxAllowedSize = maxAllowedSize;
    }

    public string Name { get; }
    public DateTimeOffset LastModified { get; }
    public long Size { get; }
    public string ContentType { get; }

    public Stream OpenReadStream(long maxAllowedSize = long.MaxValue, CancellationToken cancellationToken = default)
    {
        long limit = Math.Min(maxAllowedSize, _maxAllowedSize);
        return new JSFileStream(_js, limit);
    }
}

public sealed class JSFileStream : Stream
{
    private readonly IJSRuntime _js;
    private long _position = 0;
    private readonly long _maxSize;

    public JSFileStream(IJSRuntime js, long maxSize)
    {
        _js = js;
        _maxSize = maxSize;
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => _maxSize;
    public override long Position { get => _position; set => throw new NotSupportedException(); }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (_position >= _maxSize)
            return 0;

        byte[] chunk = await _js.InvokeAsync<byte[]>(
            "geauxFileDrop.readChunk",
            _position,
            count);

        if (chunk == null || chunk.Length == 0)
            return 0;

        Array.Copy(chunk, 0, buffer, offset, chunk.Length);
        _position += chunk.Length;

        return chunk.Length;
    }

    public override int Read(byte[] buffer, int offset, int count)
        => ReadAsync(buffer, offset, count).GetAwaiter().GetResult();

    public override void Flush() { }
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
