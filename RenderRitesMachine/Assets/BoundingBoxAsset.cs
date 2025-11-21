using OpenTK.Graphics.OpenGL4;

namespace RenderRitesMachine.Assets;

/// <summary>
/// Ресурс bounding box, содержащий VAO, VBO, EBO. Реализует IDisposable для правильного освобождения ресурсов OpenGL.
/// </summary>
public sealed class BoundingBoxAsset : IDisposable
{
    public int Vao { get; set; }
    public int Vbo { get; set; }
    public int Ebo { get; set; }
    public int IndicesCount { get; set; }

    private bool _disposed;

    ~BoundingBoxAsset()
    {
        Dispose(false);
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

        GL.DeleteBuffer(Vbo);
        GL.DeleteBuffer(Ebo);
        GL.DeleteVertexArray(Vao);

        _disposed = true;
    }
}
