using OpenTK.Mathematics;
using RenderRitesMachine.Configuration;

namespace RenderRitesMachine.Output;

/// <summary>
/// Базовая реализация камеры, содержащая общий функционал для перспективных и ортографических проекций.
/// </summary>
public abstract class CameraBase : ICamera
{
    private Vector3 _position = Vector3.Zero;
    private Vector3 _front = -Vector3.UnitZ;
    private Vector3 _up = Vector3.UnitY;
    private Vector3 _right = Vector3.UnitX;
    private float _pitch;
    private float _yaw = -MathHelper.PiOver2;
    private float _aspectRatio = 1.0f;

    private Matrix4 _cachedViewMatrix;
    private Matrix4 _cachedProjectionMatrix;
    private bool _viewMatrixDirty = true;
    private bool _projectionMatrixDirty = true;

    public Vector3 Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                _position = value;
                _viewMatrixDirty = true;
            }
        }
    }

    public Vector3 Front => _front;
    public Vector3 Up => _up;
    public Vector3 Right => _right;

    public float Speed { get; set; } = 30.0f;
    public float AngularSpeed { get; set; } = 90.0f;

    public float AspectRatio
    {
        get => _aspectRatio;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Aspect ratio must be greater than 0.");
            }

            if (Math.Abs(_aspectRatio - value) > RenderConstants.FloatEpsilon)
            {
                _aspectRatio = value;
                MarkProjectionMatrixDirty();
            }
        }
    }

    public float Pitch
    {
        get => MathHelper.RadiansToDegrees(_pitch);
        set
        {
            float angle = MathHelper.Clamp(value, RenderConstants.CameraMinPitch, RenderConstants.CameraMaxPitch);
            float newPitch = MathHelper.DegreesToRadians(angle);

            if (Math.Abs(_pitch - newPitch) > RenderConstants.FloatEpsilon)
            {
                _pitch = newPitch;
                UpdateOrientationVectors();
            }
        }
    }

    public float Yaw
    {
        get => MathHelper.RadiansToDegrees(_yaw);
        set
        {
            float newYaw = MathHelper.DegreesToRadians(value);

            if (Math.Abs(_yaw - newYaw) > RenderConstants.FloatEpsilon)
            {
                _yaw = newYaw;
                UpdateOrientationVectors();
            }
        }
    }

    public Matrix4 ViewMatrix
    {
        get
        {
            if (_viewMatrixDirty)
            {
                _cachedViewMatrix = Matrix4.LookAt(Position, Position + _front, _up);
                _viewMatrixDirty = false;
            }

            return _cachedViewMatrix;
        }
    }

    public Matrix4 ProjectionMatrix
    {
        get
        {
            if (_projectionMatrixDirty)
            {
                _cachedProjectionMatrix = CreateProjectionMatrix();
                _projectionMatrixDirty = false;
            }

            return _cachedProjectionMatrix;
        }
    }

    protected void MarkProjectionMatrixDirty()
    {
        _projectionMatrixDirty = true;
    }

    private void UpdateOrientationVectors()
    {
        _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
        _front.Y = MathF.Sin(_pitch);
        _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);
        _front = Vector3.Normalize(_front);
        _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
        _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        _viewMatrixDirty = true;
    }

    /// <summary>
    /// Создает матрицу проекции, специфичную для конкретного типа камеры.
    /// </summary>
    protected abstract Matrix4 CreateProjectionMatrix();
}

