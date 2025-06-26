using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine;
using RenderRitesMachine.ECS;

namespace RenderRitesDemo.ECS;

public class MainUpdateSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        RenderRitesMachine.Output.Window window = RenderRites.Machine.Window!;

        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();

        var transforms = world.GetPool<Transform>();

        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .End();

        foreach (int entity in filter)
        {
            ref Transform transform = ref transforms.Get(entity);
            transform.RotationAngle += 1.0f * shared.Time.UpdateDeltaTime;
        }

        if (window.IsKeyPressed(Keys.W))
        {
            PolygonMode currentMode = (PolygonMode)GL.GetInteger(GetPName.PolygonMode);
            GL.PolygonMode(TriangleFace.FrontAndBack,
                currentMode == PolygonMode.Fill ? PolygonMode.Line : PolygonMode.Fill
            );
        }

        if (window.IsKeyPressed(Keys.F))
        {
            window.WindowState = window.WindowState != WindowState.Fullscreen
                ? WindowState.Fullscreen
                : WindowState.Normal;
        }
    }
}