using OpenTK.Graphics.OpenGL4;

namespace RenderRitesMachine.Assets;

/// <summary>
///     Texture resource that releases its OpenGL handle when disposed.
/// </summary>
public sealed class TextureAsset : IDisposable
{
    private bool _disposed;
    public int Id { get; set; }
    public TextureType Type { get; set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~TextureAsset()
    {
        Dispose(false);
    }

    public void Bind() => GL.BindTextureUnit((int)Type, Id);

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
