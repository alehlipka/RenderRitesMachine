using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace RenderRitesMachine.Assets;

/// <summary>
///     Mesh resource that stores VAO/VBO/EBO handles and metadata, disposing OpenGL resources as needed.
/// </summary>
public sealed class MeshAsset : IDisposable
{
    private bool _disposed;
    public int Vao { get; set; }
    public int Vbo { get; set; }
    public int Ebo { get; set; }
    public int IndicesCount { get; set; }
    public Vector3 Minimum { get; set; }
    public Vector3 Maximum { get; set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~MeshAsset()
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
