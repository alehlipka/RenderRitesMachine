using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace RenderRitesMachine.Assets;

/// <summary>
/// Ресурс меша, содержащий VAO, VBO, EBO и метаданные. Реализует IDisposable для правильного освобождения ресурсов OpenGL.
/// </summary>
public class MeshAsset : IDisposable
{
    public int Vao { get; set; }
    public int Vbo { get; set; }
    public int Ebo { get; set; }
    public int IndicesCount { get; set; }
    public Vector3 Minimum { get; set; }
    public Vector3 Maximum { get; set; }

    private bool _disposed;

    public void Dispose()
    {
        if (_disposed) return;

        GL.DeleteBuffer(Vbo);
        GL.DeleteBuffer(Ebo);
        GL.DeleteVertexArray(Vao);

        _disposed = true;
    }
}
