using OpenTK.Mathematics;

namespace RenderRitesMachine.ECS;

/// <summary>
/// Абстракция для OpenGL вызовов, позволяющая тестировать код без реального OpenGL контекста.
/// </summary>
public interface IOpenGLWrapper
{
    /// <summary>
    /// Активирует шейдерную программу.
    /// </summary>
    void UseProgram(int program);

    /// <summary>
    /// Получает расположение uniform переменной в шейдере.
    /// </summary>
    int GetUniformLocation(int program, string name);

    /// <summary>
    /// Устанавливает значение матрицы 4x4 для uniform переменной.
    /// </summary>
    void UniformMatrix4(int location, bool transpose, ref Matrix4 matrix);
}
