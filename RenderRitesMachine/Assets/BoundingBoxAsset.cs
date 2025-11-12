using OpenTK.Graphics.OpenGL4;

namespace RenderRitesMachine.Assets;

/// <summary>
/// Ресурс bounding box, содержащий VAO, VBO, EBO. Реализует IDisposable для правильного освобождения ресурсов OpenGL.
/// </summary>
public class BoundingBoxAsset : IDisposable
{
    public int Vao { get; set; }
    public int Vbo { get; set; }
    public int Ebo { get; set; }
    public int IndicesCount { get; set; }

    private bool _disposed;

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            GL.DeleteBuffer(Vbo);
            GL.DeleteBuffer(Ebo);
            GL.DeleteVertexArray(Vao);
        }
        catch
        {
            // Игнорируем ошибки при освобождении ресурсов OpenGL
            // Ресурсы могут быть уже освобождены или контекст OpenGL может быть недоступен
        }

        _disposed = true;
    }
}
