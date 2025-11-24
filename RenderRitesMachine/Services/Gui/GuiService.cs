using OpenTK.Mathematics;

namespace RenderRitesMachine.Services.Gui;

/// <summary>
/// Default GUI service implementation that exposes a software surface, input events, and rendering pipeline.
/// </summary>
public sealed class GuiService : IGuiService
{
    private readonly GuiSurface _surface = new();
    private readonly IGuiRenderer _renderer;
    private readonly GuiTextRenderer _textRenderer = new();
    private readonly ILogger _logger;
    private bool _initialized;
    private bool _frameInProgress;
    private bool _frameUploaded;
    private bool _hasDrawCommands;

    public GuiService(ILogger logger, IGuiRenderer? renderer = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _renderer = renderer ?? new GuiRenderer();
        Events = new GuiEventQueue();
    }

    public GuiEventQueue Events { get; }

    public bool HasContent => _hasDrawCommands && _frameUploaded;

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
        _hasDrawCommands = false;
    }

    public void EndFrame()
    {
        if (!_frameInProgress)
        {
            return;
        }

        if (!_hasDrawCommands)
        {
            _surface.MarkClean();
            _frameInProgress = false;
            _frameUploaded = false;
            return;
        }

        _renderer.UploadSurface(_surface);
        _frameUploaded = true;
        _frameInProgress = false;
    }

    public void Render()
    {
        if (!HasContent)
        {
            return;
        }

        _renderer.Render();
    }

    public void FillRectangle(int x, int y, int width, int height, Color4 color)
    {
        EnsureFrame();
        _surface.FillRectangle(x, y, width, height, color);
        _hasDrawCommands = true;
    }

    public void DrawHorizontalLine(int x, int y, int length, int thickness, Color4 color)
    {
        EnsureFrame();
        _surface.DrawHorizontalLine(x, y, length, thickness, color);
        _hasDrawCommands = true;
    }

    public void DrawVerticalLine(int x, int y, int length, int thickness, Color4 color)
    {
        EnsureFrame();
        _surface.DrawVerticalLine(x, y, length, thickness, color);
        _hasDrawCommands = true;
    }

    public void DrawPixel(int x, int y, Color4 color)
    {
        EnsureFrame();
        _surface.DrawPixel(x, y, color);
        _hasDrawCommands = true;
    }

    public void DrawText(GuiFont font, string text, int x, int y, Color4 color)
    {
        EnsureFrame();
        _textRenderer.DrawText(_surface, font, text, x, y, color);
        _hasDrawCommands = true;
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

