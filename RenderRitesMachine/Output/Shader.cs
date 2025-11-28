using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.Exceptions;

namespace RenderRitesMachine.Output;

internal sealed class Shader(string path, ShaderType type)
{
    internal readonly int Handle = GL.CreateShader(type);

    internal void Create()
    {
        string source = File.Exists(path) ? File.ReadAllText(path) : path;

        GL.ShaderSource(Handle, source);
        GL.CompileShader(Handle);

        GL.GetShader(Handle, ShaderParameter.CompileStatus, out int compiled);
        if (compiled == 1)
        {
            return;
        }

        string infoLog = GL.GetShaderInfoLog(Handle);
        throw new ShaderCompilationException(type.ToString(), path, infoLog);
    }

    internal void Delete() => GL.DeleteShader(Handle);
}
