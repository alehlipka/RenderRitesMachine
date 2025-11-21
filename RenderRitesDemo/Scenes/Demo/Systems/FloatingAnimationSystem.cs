using Leopotam.EcsLite;
using OpenTK.Mathematics;
using RenderRitesDemo.Scenes.Demo.Components;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesDemo.Scenes.Demo.Systems;

internal sealed class FloatingAnimationSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        EcsPool<Transform> transforms = world.GetPool<Transform>();
        EcsPool<FloatingAnimation> floatingAnimations = world.GetPool<FloatingAnimation>();
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<FloatingAnimation>()
            .End();

        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        float delta = shared.Time.UpdateDeltaTime;

        foreach (int entity in filter)
        {
            ref FloatingAnimation animation = ref floatingAnimations.Get(entity);
            ref Transform transform = ref transforms.Get(entity);

            animation.ElapsedTime += delta;

            float angle = (MathF.Tau * animation.Frequency * animation.ElapsedTime) + animation.Phase;
            float offset = animation.Amplitude * MathF.Sin(angle);

            Vector3 position = animation.BasePosition + new Vector3(0.0f, offset, 0.0f);
            transform.Position = position;
        }
    }
}
