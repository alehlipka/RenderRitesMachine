using OpenTK.Mathematics;

namespace RenderRitesMachine.ECS;

/// <summary>
///     Abstraction over OpenGL calls to enable testing without a real context.
/// </summary>
public interface IOpenGLWrapper
{
    /// <summary>
    ///     Activates a shader program.
    /// </summary>
    void UseProgram(int program);

    /// <summary>
    ///     Retrieves the location of a uniform variable.
    /// </summary>
    int GetUniformLocation(int program, string name);

    /// <summary>
    ///     Sets the value of a 4x4 uniform matrix.
    /// </summary>
    void UniformMatrix4(int location, bool transpose, ref Matrix4 matrix);
}
