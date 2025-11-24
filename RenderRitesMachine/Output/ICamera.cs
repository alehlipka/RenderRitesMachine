using OpenTK.Mathematics;

namespace RenderRitesMachine.Output;

/// <summary>
/// Minimal camera contract that exposes view and projection matrices.
/// </summary>
public interface ICamera
{
    /// <summary>
    /// Camera position in world space.
    /// </summary>
    Vector3 Position { get; set; }

    /// <summary>
    /// Normalized view direction.
    /// </summary>
    Vector3 Front { get; }

    /// <summary>
    /// Normalized up vector.
    /// </summary>
    Vector3 Up { get; }

    /// <summary>
    /// Normalized right vector.
    /// </summary>
    Vector3 Right { get; }

    /// <summary>
    /// Translation speed.
    /// </summary>
    float Speed { get; set; }

    /// <summary>
    /// Rotation speed.
    /// </summary>
    float AngularSpeed { get; set; }

    /// <summary>
    /// Window aspect ratio (width / height).
    /// </summary>
    float AspectRatio { get; set; }

    /// <summary>
    /// Current pitch (degrees around the X axis).
    /// </summary>
    float Pitch { get; set; }

    /// <summary>
    /// Current yaw (degrees around the Y axis).
    /// </summary>
    float Yaw { get; set; }

    /// <summary>
    /// View matrix.
    /// </summary>
    Matrix4 ViewMatrix { get; }

    /// <summary>
    /// Projection matrix.
    /// </summary>
    Matrix4 ProjectionMatrix { get; }
}

