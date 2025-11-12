using Leopotam.EcsLite;

namespace RenderRitesDemo.ECS.Features.Rotation.Components;

public struct RotationTag : IEcsAutoReset<RotationTag>
{
    public float Speed;

    public void AutoReset(ref RotationTag c)
    {
        c.Speed = 0.0f;
    }
}
