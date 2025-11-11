using Leopotam.EcsLite;
using OpenTK.Mathematics;
using RenderRitesMachine.Configuration;

namespace RenderRitesMachine.ECS.Components;

public struct Transform : IEcsAutoReset<Transform>
{
    private Vector3 _position;
    private Vector3 _scale;
    private Vector3 _rotationAxis;
    private float _rotationAngle;
    
    private Matrix4 _cachedModelMatrix;
    private bool _modelMatrixDirty;

    public Vector3 Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                _position = value;
                _modelMatrixDirty = true;
            }
        }
    }

    public Vector3 Scale
    {
        get => _scale;
        set
        {
            if (_scale != value)
            {
                _scale = value;
                _modelMatrixDirty = true;
            }
        }
    }

    public Vector3 RotationAxis
    {
        get => _rotationAxis;
        set
        {
            if (_rotationAxis != value)
            {
                _rotationAxis = value;
                _modelMatrixDirty = true;
            }
        }
    }

    public float RotationAngle
    {
        get => _rotationAngle;
        set
        {
            if (Math.Abs(_rotationAngle - value) > RenderConstants.FloatEpsilon)
            {
                _rotationAngle = value;
                _modelMatrixDirty = true;
            }
        }
    }

    public Matrix4 ModelMatrix
    {
        get
        {
            if (_modelMatrixDirty)
            {
                _cachedModelMatrix = 
                    Matrix4.CreateScale(_scale) *
                    Matrix4.CreateFromQuaternion(Quaternion.FromAxisAngle(_rotationAxis, _rotationAngle)) *
                    Matrix4.CreateTranslation(_position);
                _modelMatrixDirty = false;
            }
            return _cachedModelMatrix;
        }
    }

    public void AutoReset(ref Transform c)
    {
        c._position = Vector3.Zero;
        c._scale = Vector3.One;
        c._rotationAxis = Vector3.Zero;
        c._rotationAngle = 0.0f;
        c._modelMatrixDirty = true;
        c._cachedModelMatrix = Matrix4.Identity;
    }
}

