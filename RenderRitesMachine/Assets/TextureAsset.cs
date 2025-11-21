using OpenTK.Graphics.OpenGL4;

namespace RenderRitesMachine.Assets;

/// <summary>
/// Ресурс текстуры. Реализует IDisposable для правильного освобождения ресурсов OpenGL.
/// </summary>
public sealed class TextureAsset : IDisposable
{
    public int Id { get; set; }
    public TextureType Type { get; set; }

    private bool _disposed;

    ~TextureAsset()
    {
        Dispose(false);
    }

    public void Bind()
    {
        GL.BindTextureUnit((int)Type, Id);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        GL.DeleteTexture(Id);

        _disposed = true;
    }
}
