using OpenTK.Mathematics;

namespace RenderRitesMachine.Services.Gui;

/// <summary>
/// Default GUI service implementation that exposes a software surface, input events, and rendering pipeline.
/// </summary>
public sealed class GuiService : IGuiService
{
    private readonly GuiSurface _surface = new();
    private readonly GuiRenderer _renderer = new();
    private readonly ILogger _logger;
    private bool _initialized;
    private bool _frameInProgress;
    private bool _frameUploaded;

    public GuiService(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        Events = new GuiEventQueue();
    }

    public GuiEventQueue Events { get; }

    public int Width => _surface.Width;

    public int Height => _surface.Height;

    public void EnsureInitialized(int width, int height)
    {
        if (_initialized)
        {
            return;
        }

        _renderer.Initialize();
        _surface.Resize(Math.Max(1, width), Math.Max(1, height));
        _renderer.EnsureTextureSize(_surface.Width, _surface.Height);
        _initialized = true;
        _logger.LogInfo($"GUI service initialized ({_surface.Width}x{_surface.Height})");
    }

    public void Resize(int width, int height)
    {
        ThrowIfNotInitialized();

        width = Math.Max(1, width);
        height = Math.Max(1, height);

        _surface.Resize(width, height);
        _renderer.EnsureTextureSize(width, height);
    }

    public void BeginFrame(Color4 clearColor)
    {
        ThrowIfNotInitialized();

        if (_frameInProgress)
        {
            throw new InvalidOperationException("BeginFrame called before previous frame ended.");
        }

        _surface.Clear(clearColor);
        _frameInProgress = true;
        _frameUploaded = false;
    }

    public void EndFrame()
    {
        if (!_frameInProgress)
        {
            return;
        }

        _renderer.UploadSurface(_surface);
        _frameUploaded = true;
        _frameInProgress = false;
    }

    public void Render()
    {
        if (!_frameUploaded)
        {
            return;
        }

        _renderer.Render();
    }

    public void FillRectangle(int x, int y, int width, int height, Color4 color)
    {
        EnsureFrame();
        _surface.FillRectangle(x, y, width, height, color);
    }

    public void DrawHorizontalLine(int x, int y, int length, int thickness, Color4 color)
    {
        EnsureFrame();
        _surface.DrawHorizontalLine(x, y, length, thickness, color);
    }

    public void DrawVerticalLine(int x, int y, int length, int thickness, Color4 color)
    {
        EnsureFrame();
        _surface.DrawVerticalLine(x, y, length, thickness, color);
    }

    public void DrawPixel(int x, int y, Color4 color)
    {
        EnsureFrame();
        _surface.DrawPixel(x, y, color);
    }

    public void Dispose()
    {
        _renderer.Dispose();
    }

    private void ThrowIfNotInitialized()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("GUI service is not initialized. Call EnsureInitialized first.");
        }
    }

    private void EnsureFrame()
    {
        ThrowIfNotInitialized();

        if (!_frameInProgress)
        {
            throw new InvalidOperationException("Drawing operations require an active frame. Call BeginFrame first.");
        }
    }
}

