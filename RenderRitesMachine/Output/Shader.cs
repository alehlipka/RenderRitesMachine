using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.Exceptions;

namespace RenderRitesMachine.Output;

internal sealed class Shader(string path, ShaderType type)
{
    private readonly string _path = path;
    private readonly ShaderType _type = type;

    internal readonly int Handle = GL.CreateShader(type);

    internal void Create()
    {
        GL.ShaderSource(Handle, File.ReadAllText(_path));
        GL.CompileShader(Handle);

        GL.GetShader(Handle, ShaderParameter.CompileStatus, out int compiled);
        if (compiled == 1)
        {
            return;
        }

        string infoLog = GL.GetShaderInfoLog(Handle);
        throw new ShaderCompilationException(_type.ToString(), _path, infoLog);
    }

    internal void Delete()
    {
        GL.DeleteShader(Handle);
    }
}
