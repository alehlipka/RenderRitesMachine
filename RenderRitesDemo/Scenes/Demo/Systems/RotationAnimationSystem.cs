using Leopotam.EcsLite;
using OpenTK.Mathematics;
using RenderRitesDemo.Scenes.Demo.Components;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesDemo.Scenes.Demo.Systems;

internal sealed class RotationAnimationSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        EcsPool<Transform> transforms = world.GetPool<Transform>();
        EcsPool<RotationAnimation> rotations = world.GetPool<RotationAnimation>();
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<RotationAnimation>()
            .End();

        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        float delta = shared.Time.UpdateDeltaTime;

        foreach (int entity in filter)
        {
            ref RotationAnimation rotation = ref rotations.Get(entity);
            if (rotation.Axis.LengthSquared < 0.0001f || Math.Abs(rotation.Speed) < float.Epsilon)
            {
                continue;
            }

            ref Transform transform = ref transforms.Get(entity);
            if (rotation.Axis.LengthSquared is > 1.001f or < 0.999f)
            {
                rotation.Axis = Vector3.Normalize(rotation.Axis);
            }

            transform.RotationAxis = rotation.Axis;
            transform.RotationAngle += rotation.Speed * delta;
        }
    }
}
