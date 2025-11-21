using OpenTK.Mathematics;
using RenderRitesMachine.Configuration;

namespace RenderRitesMachine.Output;

/// <summary>
/// Представляет перспективную камеру с возможностью управления позицией, ориентацией и параметрами проекции.
/// </summary>
public class PerspectiveCamera : CameraBase
{
    private float _fov = MathHelper.PiOver2;

    /// <summary>
    /// Поле зрения камеры (Field of View) в градусах. Ограничено диапазоном от 1 до 90 градусов.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если значение вне допустимого диапазона.</exception>
    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set
        {
            if (value < RenderConstants.CameraMinFov || value > RenderConstants.CameraMaxFov)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"FOV must be between {RenderConstants.CameraMinFov} and {RenderConstants.CameraMaxFov} degrees.");
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
    protected override Matrix4 CreateProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, RenderConstants.CameraNearPlane, RenderConstants.CameraFarPlane);
    }
}
