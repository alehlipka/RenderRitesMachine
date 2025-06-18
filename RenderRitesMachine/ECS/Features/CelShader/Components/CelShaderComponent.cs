using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.Utilities;

namespace RenderRitesMachine.ECS.Features.CelShader.Components;

public readonly struct CelShaderComponent(string path) : IComponent
{
    private readonly int _handle = ShaderCreator.Create(path);

    private readonly Dictionary<string, int> _uniformLocations = new();
    
    public void Use()
    {
        GL.UseProgram(_handle);
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

        location = GL.GetUniformLocation(_handle, name);
        _uniformLocations[name] = location;
        
        return location;
    }

    public void Dispose()
    {
        GL.UseProgram(0);
        GL.DeleteProgram(_handle);
    }
}