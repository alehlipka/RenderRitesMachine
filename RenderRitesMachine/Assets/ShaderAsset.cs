using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace RenderRitesMachine.Assets;

public struct ShaderAsset()
{
    private readonly Dictionary<string, int> _uniformLocations = [];
    
    public int Id;
    
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

        location = GL.GetUniformLocation(Id, name);
        _uniformLocations[name] = location;
        
        return location;
    }
}