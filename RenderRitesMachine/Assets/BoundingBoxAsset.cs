using OpenTK.Graphics.OpenGL4;

namespace RenderRitesMachine.Assets;

/// <summary>
///     Bounding box resource that encapsulates VAO/VBO/EBO handles and disposes OpenGL objects.
/// </summary>
public sealed class BoundingBoxAsset : IDisposable
{
    private bool _disposed;
    public int Vao { get; set; }
    public int Vbo { get; set; }
    public int Ebo { get; set; }
    public int IndicesCount { get; set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~BoundingBoxAsset()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        GL.DeleteBuffer(Vbo);
        GL.DeleteBuffer(Ebo);
        GL.DeleteVertexArray(Vao);

        _disposed = true;
    }
}
