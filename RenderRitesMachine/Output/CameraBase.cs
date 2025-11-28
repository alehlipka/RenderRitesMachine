using OpenTK.Mathematics;
using RenderRitesMachine.Configuration;

namespace RenderRitesMachine.Output;

/// <summary>
///     Base camera implementation that houses shared logic for perspective and orthographic projections.
/// </summary>
public abstract class CameraBase : ICamera
{
    private float _aspectRatio = 1.0f;
    private Matrix4 _cachedProjectionMatrix;

    private Matrix4 _cachedViewMatrix;
    private Vector3 _front = -Vector3.UnitZ;
    private float _pitch;
    private Vector3 _position = Vector3.Zero;
    private bool _projectionMatrixDirty = true;
    private bool _viewMatrixDirty = true;
    private float _yaw = -MathHelper.PiOver2;

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
    public Vector3 Up { get; private set; } = Vector3.UnitY;

    public Vector3 Right { get; private set; } = Vector3.UnitX;

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
                _cachedViewMatrix = Matrix4.LookAt(Position, Position + _front, Up);
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

    protected void MarkProjectionMatrixDirty() => _projectionMatrixDirty = true;

    private void UpdateOrientationVectors()
    {
        _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
        _front.Y = MathF.Sin(_pitch);
        _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);
        _front = Vector3.Normalize(_front);
        Right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, _front));
        _viewMatrixDirty = true;
    }

    /// <summary>
    ///     Creates the projection matrix specific to the concrete camera type.
    /// </summary>
    protected abstract Matrix4 CreateProjectionMatrix();
}
