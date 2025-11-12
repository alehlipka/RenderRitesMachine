using Leopotam.EcsLite;
using RenderRitesDemo.ECS.Features.Rotation.Components;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesDemo.ECS.Features.Rotation.Systems;

public class RotationUpdateSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();

        EcsPool<Transform>? transforms = world.GetPool<Transform>();
        EcsPool<RotationTag>? rotations = world.GetPool<RotationTag>();

        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<RotationTag>()
            .End();

        foreach (int entity in filter)
        {
            ref Transform transform = ref transforms.Get(entity);
            RotationTag rotation = rotations.Get(entity);

            transform.RotationAngle += rotation.Speed * shared.Time.UpdateDeltaTime;
        }
    }
}
