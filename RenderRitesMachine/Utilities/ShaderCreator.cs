using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.Output;

namespace RenderRitesMachine.Utilities;

public abstract class ShaderCreator
{
    public static int Create(string path)
    {
        string vertexShaderPath = Path.Combine(path, "vertex.glsl");
        string fragmentShaderPath = Path.Combine(path, "fragment.glsl");
        
        Shader vertexShader = new(vertexShaderPath, ShaderType.VertexShader);
        Shader fragmentShader = new(fragmentShaderPath, ShaderType.FragmentShader);

        vertexShader.Create();
        fragmentShader.Create();

        int handle = GL.CreateProgram();

        GL.AttachShader(handle, vertexShader.Handle);
        GL.AttachShader(handle, fragmentShader.Handle);

        GL.LinkProgram(handle);

        GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int linked);
        if (linked != 1)
        {
            string infoLog = GL.GetProgramInfoLog(handle);
            throw new Exception($"Shader link error: {infoLog}");
        }

        GL.DetachShader(handle, vertexShader.Handle);
        GL.DetachShader(handle, fragmentShader.Handle);

        vertexShader.Delete();
        fragmentShader.Delete();

        return handle;
    }
}