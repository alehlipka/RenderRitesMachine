using OpenTK.Mathematics;
using RenderRitesMachine.Configuration;

namespace RenderRitesMachine.Output;

/// <summary>
///     Orthographic camera suited for 2D scenes or isometric views.
/// </summary>
public class OrthographicCamera : CameraBase
{
    private float _height = 10.0f;

    /// <summary>
    ///     Height of the orthographic volume (world units). Width is derived from <see cref="CameraBase.AspectRatio" />.
    /// </summary>
    public float Height
    {
        get => _height;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "Orthographic height must be greater than zero.");
            }

            if (Math.Abs(_height - value) > RenderConstants.FloatEpsilon)
            {
                _height = value;
                MarkProjectionMatrixDirty();
            }
        }
    }

    /// <summary>
    ///     Width of the orthographic volume (world units).
    /// </summary>
    public float Width => _height * AspectRatio;

    /// <inheritdoc />
    protected override Matrix4 CreateProjectionMatrix()
    {
        float width = Width;
        float height = _height;
        return Matrix4.CreateOrthographic(width, height, RenderConstants.CameraNearPlane,
            RenderConstants.CameraFarPlane);
    }
}
