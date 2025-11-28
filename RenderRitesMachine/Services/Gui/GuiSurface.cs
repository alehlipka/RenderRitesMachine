using OpenTK.Mathematics;

namespace RenderRitesMachine.Services.Gui;

/// <summary>
///     CPU-side RGBA surface used for GUI drawing.
/// </summary>
public sealed class GuiSurface
{
    private byte[] _buffer = Array.Empty<byte>();

    public int Width { get; private set; }
    public int Height { get; private set; }

    public bool IsDirty { get; private set; }

    public ReadOnlySpan<byte> Buffer => _buffer;

    public void Resize(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Surface dimensions must be positive.");
        }

        if (width == Width && height == Height)
        {
            return;
        }

        Width = width;
        Height = height;
        _buffer = new byte[Width * Height * 4];
        IsDirty = true;
    }

    public void Clear(Color4 color)
    {
        EnsureInitialized();

        (byte r, byte g, byte b, byte a) = ToRgba(color);

        for (int i = 0; i < _buffer.Length; i += 4)
        {
            _buffer[i] = r;
            _buffer[i + 1] = g;
            _buffer[i + 2] = b;
            _buffer[i + 3] = a;
        }

        IsDirty = true;
    }

    public void DrawPixel(int x, int y, Color4 color)
    {
        if (!IsInside(x, y))
        {
            return;
        }

        int index = (y * Width + x) * 4;
        (byte r, byte g, byte b, byte a) = ToRgba(color);
        _buffer[index] = r;
        _buffer[index + 1] = g;
        _buffer[index + 2] = b;
        _buffer[index + 3] = a;
        IsDirty = true;
    }

    public void FillRectangle(int x, int y, int width, int height, Color4 color)
    {
        EnsureInitialized();

        if (width <= 0 || height <= 0)
        {
            return;
        }

        int xStart = Math.Clamp(x, 0, Width);
        int yStart = Math.Clamp(y, 0, Height);
        int xEnd = Math.Clamp(x + width, 0, Width);
        int yEnd = Math.Clamp(y + height, 0, Height);

        if (xStart >= xEnd || yStart >= yEnd)
        {
            return;
        }

        (byte r, byte g, byte b, byte a) = ToRgba(color);
        int stride = Width * 4;

        for (int row = yStart; row < yEnd; row++)
        {
            int rowStart = row * stride + xStart * 4;
            for (int col = xStart; col < xEnd; col++)
            {
                int index = rowStart + (col - xStart) * 4;
                _buffer[index] = r;
                _buffer[index + 1] = g;
                _buffer[index + 2] = b;
                _buffer[index + 3] = a;
            }
        }

        IsDirty = true;
    }

    public void DrawHorizontalLine(int x, int y, int length, int thickness, Color4 color)
    {
        if (thickness <= 0 || length <= 0)
        {
            return;
        }

        FillRectangle(x, y, length, thickness, color);
    }

    public void DrawVerticalLine(int x, int y, int length, int thickness, Color4 color)
    {
        if (thickness <= 0 || length <= 0)
        {
            return;
        }

        FillRectangle(x, y, thickness, length, color);
    }

    public void MarkClean() => IsDirty = false;

    public void BlendPixel(int x, int y, Color4 color, float alpha)
    {
        EnsureInitialized();

        if (!IsInside(x, y) || alpha <= 0f)
        {
            return;
        }

        float srcAlpha = Math.Clamp(color.A * alpha, 0f, 1f);
        if (srcAlpha <= 0f)
        {
            return;
        }

        int index = (y * Width + x) * 4;
        float invAlpha = 1f - srcAlpha;

        float srcR = color.R * 255f;
        float srcG = color.G * 255f;
        float srcB = color.B * 255f;

        float dstR = _buffer[index];
        float dstG = _buffer[index + 1];
        float dstB = _buffer[index + 2];
        float dstA = _buffer[index + 3] / 255f;

        _buffer[index] = (byte)Math.Clamp(srcR * srcAlpha + dstR * invAlpha, 0f, 255f);
        _buffer[index + 1] = (byte)Math.Clamp(srcG * srcAlpha + dstG * invAlpha, 0f, 255f);
        _buffer[index + 2] = (byte)Math.Clamp(srcB * srcAlpha + dstB * invAlpha, 0f, 255f);

        float outAlpha = srcAlpha + dstA * invAlpha;
        _buffer[index + 3] = (byte)Math.Clamp(outAlpha * 255f, 0f, 255f);
    }

    private void EnsureInitialized()
    {
        if (Width == 0 || Height == 0 || _buffer.Length == 0)
        {
            throw new InvalidOperationException("GUI surface is not initialized. Call Resize before drawing.");
        }
    }

    private static (byte r, byte g, byte b, byte a) ToRgba(Color4 color)
    {
        return (
            (byte)Math.Clamp(color.R * 255f, 0, 255),
            (byte)Math.Clamp(color.G * 255f, 0, 255),
            (byte)Math.Clamp(color.B * 255f, 0, 255),
            (byte)Math.Clamp(color.A * 255f, 0, 255)
        );
    }

    private bool IsInside(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
}
