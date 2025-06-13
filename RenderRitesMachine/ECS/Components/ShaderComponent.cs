using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Output;

namespace RenderRitesMachine.ECS.Components;

public struct ShaderComponent(string path) : IComponent
{
    public int Handle { get; private set; } = LoadShaders(path);

    private readonly Dictionary<string, int> _uniformLocations = new();

    private static int LoadShaders(string programPath)
    {
        string vertexShaderPath = Path.Combine(programPath, "vertex.glsl");
        string fragmentShaderPath = Path.Combine(programPath, "fragment.glsl");
        
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
    
    public void Use()
    {
        GL.UseProgram(Handle);
    }
    
    public void SetInt(string name, int value)
    {
        GL.Uniform1(GetUniformLocation(name), value);
    }

    public void SetFloat(string name, float value)
    {
        GL.Uniform1(GetUniformLocation(name), value);
    }
    
    public void SetVector3(string name, Vector3 vector)
    {
        GL.Uniform3(GetUniformLocation(name), vector);
    }
    
    public void SetMatrix4(string name, Matrix4 matrix)
    {
        GL.UniformMatrix4(GetUniformLocation(name), true, ref matrix);
    }
    
    private int GetUniformLocation(string name)
    {
        if (_uniformLocations.TryGetValue(name, out int location))
        {
            return location;
        }

        location = GL.GetUniformLocation(Handle, name);
        _uniformLocations[name] = location;
        
        return location;
    }

    public void Dispose()
    {
        GL.UseProgram(0);
        GL.DeleteProgram(Handle);
    }
}