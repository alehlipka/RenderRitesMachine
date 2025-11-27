using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.Exceptions;

namespace RenderRitesMachine.Output;

internal sealed class ShaderProgram
{
    internal readonly int Handle = GL.CreateProgram();

    internal void AttachShader(Shader shader)
    {
        GL.AttachShader(Handle, shader.Handle);
    }

    internal void Link()
    {
        GL.LinkProgram(Handle);
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linked);
        if (linked != 1)
        {
            string infoLog = GL.GetProgramInfoLog(Handle);
            Delete();
            throw new ShaderLinkingException("render_rites_machine_gui", infoLog);
        }
    }

    internal void DetachShader(Shader shader)
    {
        GL.DetachShader(Handle, shader.Handle);
    }

    internal void Delete()
    {
        GL.DeleteProgram(Handle);
    }
}
