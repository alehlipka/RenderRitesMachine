using OpenTK.Mathematics;
using RenderRitesMachine.Configuration;

namespace RenderRitesMachine.Output;

/// <summary>
///     Perspective camera with adjustable position, orientation, and projection parameters.
/// </summary>
public class PerspectiveCamera : CameraBase
{
    private float _fov = MathHelper.PiOver2;

    /// <summary>
    ///     Field of view in degrees, clamped between 1 and 90.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is outside the allowed range.</exception>
    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set
        {
            if (value < RenderConstants.CameraMinFov || value > RenderConstants.CameraMaxFov)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    $"FOV must be between {RenderConstants.CameraMinFov} and {RenderConstants.CameraMaxFov} degrees.");
            }

            float angle = MathHelper.Clamp(value, RenderConstants.CameraMinFov, RenderConstants.CameraMaxFov);
            float newFov = MathHelper.DegreesToRadians(angle);
            if (Math.Abs(_fov - newFov) > RenderConstants.FloatEpsilon)
            {
                _fov = newFov;
                MarkProjectionMatrixDirty();
            }
        }
    }

    /// <inheritdoc />
    protected override Matrix4 CreateProjectionMatrix() => Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio,
        RenderConstants.CameraNearPlane, RenderConstants.CameraFarPlane);
}
