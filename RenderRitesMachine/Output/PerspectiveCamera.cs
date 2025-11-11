using OpenTK.Mathematics;
using RenderRitesMachine.Configuration;

namespace RenderRitesMachine.Output;

/// <summary>
/// Представляет перспективную камеру с возможностью управления позицией, ориентацией и параметрами проекции.
/// </summary>
public class PerspectiveCamera
{
    
    private Vector3 _position = Vector3.Zero;
    
    /// <summary>
    /// Позиция камеры в мировом пространстве.
    /// </summary>
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
    
    private float _aspectRatio = 1.0f;
    
    /// <summary>
    /// Соотношение сторон окна (ширина / высота). Используется для расчета матрицы проекции.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если значение меньше или равно 0.</exception>
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
                _projectionMatrixDirty = true;
            }
        }
    }
    
    /// <summary>
    /// Направление взгляда камеры (нормализованный вектор).
    /// </summary>
    public Vector3 Front => _front;
    
    /// <summary>
    /// Вектор "вверх" камеры (нормализованный вектор).
    /// </summary>
    public Vector3 Up => _up;
    
    /// <summary>
    /// Вектор "вправо" камеры (нормализованный вектор).
    /// </summary>
    public Vector3 Right => _right;
    
    /// <summary>
    /// Скорость движения камеры в единицах в секунду.
    /// </summary>
    public float Speed { get; set; } = 30.0f;
    
    /// <summary>
    /// Угловая скорость поворота камеры в градусах в секунду.
    /// </summary>
    public float AngularSpeed { get; set; } = 90.0f;
    
    private Vector3 _front = -Vector3.UnitZ;
    private Vector3 _up = Vector3.UnitY;
    private Vector3 _right = Vector3.UnitX;
    private float _pitch;
    private float _yaw = -MathHelper.PiOver2;
    private float _fov = MathHelper.PiOver2;
    
    private Matrix4 _cachedViewMatrix;
    private Matrix4 _cachedProjectionMatrix;
    private bool _viewMatrixDirty = true;
    private bool _projectionMatrixDirty = true;
    
    /// <summary>
    /// Матрица вида камеры. Кэшируется и обновляется только при изменении позиции или ориентации.
    /// </summary>
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
    
    /// <summary>
    /// Матрица проекции камеры. Кэшируется и обновляется только при изменении FOV или AspectRatio.
    /// </summary>
    public Matrix4 ProjectionMatrix
    {
        get
        {
            if (_projectionMatrixDirty)
            {
                _cachedProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, RenderConstants.CameraNearPlane, RenderConstants.CameraFarPlane);
                _projectionMatrixDirty = false;
            }
            return _cachedProjectionMatrix;
        }
    }
    
    /// <summary>
    /// Угол наклона камеры вверх/вниз в градусах. Ограничен диапазоном от -89 до 89 градусов.
    /// </summary>
    public float Pitch
    {
        get => MathHelper.RadiansToDegrees(_pitch);
        set
        {
            float angle = MathHelper.Clamp(value, RenderConstants.CameraMinPitch, RenderConstants.CameraMaxPitch);
            _pitch = MathHelper.DegreesToRadians(angle);
            UpdateVectors();
        }
    }

    /// <summary>
    /// Угол поворота камеры влево/вправо в градусах.
    /// </summary>
    public float Yaw
    {
        get => MathHelper.RadiansToDegrees(_yaw);
        set
        {
            _yaw = MathHelper.DegreesToRadians(value);
            UpdateVectors();
        }
    }

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
                _projectionMatrixDirty = true;
            }
        }
    }

    private void UpdateVectors()
    {
        _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
        _front.Y = MathF.Sin(_pitch);
        _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);
        _front = Vector3.Normalize(_front);
        _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
        _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        _viewMatrixDirty = true;
    }
}