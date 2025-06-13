using OpenTK.Mathematics;

namespace RenderRitesMachine.Utilities;

public class RotationInfo
{
    private float _angle = 0;
    private Vector3 _axis = Vector3.Zero;
    private Quaternion _quaternion = Quaternion.Identity;

    public Matrix4 Matrix => Matrix4.CreateFromQuaternion(_quaternion);

    public float Angle
    {
        get => _angle;
        set
        {
            _angle = value;
            _quaternion = Quaternion.FromAxisAngle(_axis, _angle);
        }
    }

    public Vector3 Axis
    {
        get => _axis;
        set
        {
            _axis = value.Normalized();
            _quaternion = Quaternion.FromAxisAngle(_axis, _angle);
        }
    }

    public void Rotate(float speed, double deltaTime)
    {
        Angle = _angle + speed * (float)deltaTime;
    }
}