using OpenTK.Mathematics;

namespace RenderRitesMachine.Output;

/// <summary>
/// Минимальный контракт камеры, обеспечивающий построение матриц вида и проекции.
/// </summary>
public interface ICamera
{
    /// <summary>
    /// Позиция камеры в мировом пространстве.
    /// </summary>
    Vector3 Position { get; set; }

    /// <summary>
    /// Нормализованный вектор направления взгляда.
    /// </summary>
    Vector3 Front { get; }

    /// <summary>
    /// Нормализованный вектор "вверх".
    /// </summary>
    Vector3 Up { get; }

    /// <summary>
    /// Нормализованный вектор "вправо".
    /// </summary>
    Vector3 Right { get; }

    /// <summary>
    /// Скорость поступательного движения.
    /// </summary>
    float Speed { get; set; }

    /// <summary>
    /// Скорость вращения.
    /// </summary>
    float AngularSpeed { get; set; }

    /// <summary>
    /// Соотношение сторон окна (ширина / высота).
    /// </summary>
    float AspectRatio { get; set; }

    /// <summary>
    /// Текущий угол наклона по оси X (в градусах).
    /// </summary>
    float Pitch { get; set; }

    /// <summary>
    /// Текущий угол поворота по оси Y (в градусах).
    /// </summary>
    float Yaw { get; set; }

    /// <summary>
    /// Матрица вида.
    /// </summary>
    Matrix4 ViewMatrix { get; }

    /// <summary>
    /// Матрица проекции.
    /// </summary>
    Matrix4 ProjectionMatrix { get; }
}

