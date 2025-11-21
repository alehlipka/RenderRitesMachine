using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace RenderRitesMachine.Assets;

/// <summary>
/// Ресурс шейдерной программы. Реализует IDisposable для правильного освобождения ресурсов OpenGL.
/// </summary>
public sealed class ShaderAsset : IDisposable
{
    private readonly Dictionary<string, int> _uniformLocations = [];

    public int Id { get; set; }

    private bool _disposed;

    ~ShaderAsset()
    {
        Dispose(false);
    }

    public void Use()
    {
        GL.UseProgram(Id);
    }

    public void SetInt(string name, int value)
    {
        GL.Uniform1(GetUniformLocation(name), value);
    }

    public void SetFloat(string name, float value)
    {
        GL.Uniform1(GetUniformLocation(name), value);
    }

    public void SetVector2(string name, Vector2 vector)
    {
        GL.Uniform2(GetUniformLocation(name), vector);
    }

    public void SetVector3(string name, Vector3 vector)
    {
        GL.Uniform3(GetUniformLocation(name), vector);
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        GL.UniformMatrix4(GetUniformLocation(name), true, ref matrix);
    }

    public void SetBool(string name, bool value)
    {
        GL.Uniform1(GetUniformLocation(name), value ? 1 : 0);
    }

    private int GetUniformLocation(string name)
    {
        if (_uniformLocations.TryGetValue(name, out int location))
        {
            return location;
        }

        location = GL.GetUniformLocation(Id, name);
        _uniformLocations[name] = location;

        return location;
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

        GL.DeleteProgram(Id);

        _disposed = true;
    }
}
