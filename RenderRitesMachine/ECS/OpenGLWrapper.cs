using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace RenderRitesMachine.ECS;

/// <summary>
///     Real OpenGL implementation of <see cref="IOpenGLWrapper" />.
/// </summary>
public class OpenGLWrapper : IOpenGLWrapper
{
    public void UseProgram(int program) => GL.UseProgram(program);

    public int GetUniformLocation(int program, string name) => GL.GetUniformLocation(program, name);

    public void UniformMatrix4(int location, bool transpose, ref Matrix4 matrix) =>
        GL.UniformMatrix4(location, transpose, ref matrix);
}
