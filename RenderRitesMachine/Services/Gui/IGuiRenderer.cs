namespace RenderRitesMachine.Services.Gui;

/// <summary>
/// Abstraction over the GPU-specific GUI renderer to simplify testing.
/// </summary>
public interface IGuiRenderer : IDisposable
{
    void Initialize();
    void EnsureTextureSize(int width, int height);
    void UploadSurface(GuiSurface surface);
    void Render();
}

