using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace RenderRitesMachine.GraphicsResources.Shader;

public class ShaderProgram(
    string name,
    string path,
    string vertexShaderFilename = "vertex.glsl",
    string fragmentShaderFilename = "fragment.glsl"
) : Resource(name)
{
    private readonly string _vertexShaderPath = Path.Combine(path, name, vertexShaderFilename);
    private readonly string _fragmentShaderPath = Path.Combine(path, name, fragmentShaderFilename);
    private int _handle;

    protected override void Load()
    {
        Shader vertexShader = new(_vertexShaderPath, ShaderType.VertexShader);
        Shader fragmentShader = new(_fragmentShaderPath, ShaderType.FragmentShader);

        vertexShader.Create();
        fragmentShader.Create();

        _handle = GL.CreateProgram();

        GL.AttachShader(_handle, vertexShader.Handle);
        GL.AttachShader(_handle, fragmentShader.Handle);

        GL.LinkProgram(_handle);

        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int linked);
        if (linked != 1)
        {
            string infoLog = GL.GetProgramInfoLog(_handle);
            throw new Exception($"Shader link error: {infoLog}");
        }

        GL.DetachShader(_handle, vertexShader.Handle);
        GL.DetachShader(_handle, fragmentShader.Handle);

        vertexShader.Delete();
        fragmentShader.Delete();
    }

    protected override void Unload()
    {
        GL.UseProgram(0);
        GL.DeleteProgram(_handle);
    }

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        int location = GL.GetUniformLocation(_handle, name);
        if (location == -1)
        {
            Console.WriteLine($"Uniform '{name}' not found in shader!");
            return;
        }
        GL.UniformMatrix4(location, true, ref matrix);
    }
}