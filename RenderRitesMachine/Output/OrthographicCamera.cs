using OpenTK.Mathematics;
using RenderRitesMachine.Configuration;

namespace RenderRitesMachine.Output;

/// <summary>
/// Камера с ортографической проекцией, удобная для 2D сцен или изометрического вида.
/// </summary>
public class OrthographicCamera : CameraBase
{
    private float _height = 10.0f;

    /// <summary>
    /// Высота ортографического объема (единицы мира). Ширина вычисляется на основе <see cref="CameraBase.AspectRatio"/>.
    /// </summary>
    public float Height
    {
        get => _height;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Orthographic height must be greater than zero.");
            }

            if (Math.Abs(_height - value) > RenderConstants.FloatEpsilon)
            {
                _height = value;
                MarkProjectionMatrixDirty();
            }
        }
    }

    /// <summary>
    /// Ширина ортографического объема (единицы мира).
    /// </summary>
    public float Width => _height * AspectRatio;

    /// <inheritdoc />
    protected override Matrix4 CreateProjectionMatrix()
    {
        float width = Width;
        float height = _height;
        return Matrix4.CreateOrthographic(width, height, RenderConstants.CameraNearPlane, RenderConstants.CameraFarPlane);
    }
}

