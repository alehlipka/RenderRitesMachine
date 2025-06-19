using Leopotam.EcsLite;
using OpenTK.Mathematics;

namespace RenderRitesDemo.ECS;

public struct TransformComponent : IEcsAutoReset<TransformComponent>
{
    public Vector3 Position;
    public Vector3 Scale;
    public float Angle;
    public Vector3 Axis;
    
    public void AutoReset(ref TransformComponent component)
    {
        component.Position = Vector3.Zero;
        component.Scale = Vector3.One;
        component.Angle = 0.0f;
        component.Axis = Vector3.Zero;
    }
}