using OpenTK.Mathematics;

namespace RenderRitesMachine.Services.Gui;

/// <summary>
/// High-level API that exposes a software GUI surface, drawing helpers,
/// and event queue for UI systems.
/// </summary>
public interface IGuiService : IDisposable
{
    /// <summary>
    /// Queue with input events gathered from the host window.
    /// </summary>
    GuiEventQueue Events { get; }

    /// <summary>
    /// Indicates whether the current frame contains any draw commands.
    /// </summary>
    bool HasContent { get; }

    /// <summary>
    /// Width of the underlying GUI surface in pixels.
    /// </summary>
    int Width { get; }

    /// <summary>
    /// Height of the underlying GUI surface in pixels.
    /// </summary>
    int Height { get; }

    /// <summary>
    /// Ensures that the GUI pipeline is initialized for the current OpenGL context.
    /// Must be called from the render thread after the context becomes current.
    /// </summary>
    void EnsureInitialized(int width, int height);

    /// <summary>
    /// Adjusts the size of the GUI surface to match the window.
    /// </summary>
    void Resize(int width, int height);

    /// <summary>
    /// Begins a new GUI frame by clearing the surface with the supplied color.
    /// </summary>
    void BeginFrame(Color4 clearColor);

    /// <summary>
    /// Uploads surface changes to the GPU and finalizes the frame.
    /// </summary>
    void EndFrame();

    /// <summary>
    /// Renders the GUI texture on top of the current framebuffer.
    /// </summary>
    void Render();

    /// <summary>
    /// Fills a rectangle with the specified color (in surface coordinates).
    /// </summary>
    void FillRectangle(int x, int y, int width, int height, Color4 color);

    /// <summary>
    /// Draws a horizontal line with the specified color and thickness.
    /// </summary>
    void DrawHorizontalLine(int x, int y, int length, int thickness, Color4 color);

    /// <summary>
    /// Draws a vertical line with the specified color and thickness.
    /// </summary>
    void DrawVerticalLine(int x, int y, int length, int thickness, Color4 color);

    /// <summary>
    /// Draws a single pixel on the GUI surface.
    /// </summary>
    void DrawPixel(int x, int y, Color4 color);

    /// <summary>
    /// Renders text using the provided font at the specified location.
    /// </summary>
    void DrawText(GuiFont font, string text, int x, int y, Color4 color);
}

