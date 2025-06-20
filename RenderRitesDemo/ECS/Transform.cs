using Leopotam.EcsLite;
using OpenTK.Mathematics;

namespace RenderRitesDemo.ECS;

public struct Transform : IEcsAutoReset<Transform>
{
    public Vector3 Position;
    public Vector3 Scale;
    public Quaternion Quaternion;

    public void AutoReset(ref Transform component)
    {
        component.Position = Vector3.Zero;
        component.Scale = Vector3.One;
        component.Quaternion = Quaternion.Identity;
    }
}