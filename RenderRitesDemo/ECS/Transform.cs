using Leopotam.EcsLite;
using OpenTK.Mathematics;

namespace RenderRitesDemo.ECS;

public struct Transform : IEcsAutoReset<Transform>
{
    public Vector3 Position;
    public Vector3 Scale;
    public Vector3 RotationAxis;
    public float RotationAngle;

    public Matrix4 ModelMatrix => 
        Matrix4.CreateScale(Scale) *
        Matrix4.CreateFromQuaternion(Quaternion.FromAxisAngle(RotationAxis, RotationAngle)) *
        Matrix4.CreateTranslation(Position);

    public void AutoReset(ref Transform component)
    {
        component.Position = Vector3.Zero;
        component.Scale = Vector3.One;
        component.RotationAxis = Vector3.Zero;
        component.RotationAngle = 0.0f;
    }
}