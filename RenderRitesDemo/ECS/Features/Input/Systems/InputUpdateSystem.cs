using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesDemo.ECS.Features.Rotation.Components;
using RenderRitesMachine;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.ECS.Features.Input.Systems;

public class InputUpdateSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        RenderRitesMachine.Output.Window window = RenderRites.Machine.Window!;

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

        if (window.IsKeyPressed(Keys.R))
        {
            EcsFilter rotationFilter = world.Filter<RotationTag>().End();
            var rotations = world.GetPool<RotationTag>();
            
            foreach (int entity in rotationFilter)
            {
                ref RotationTag rotation = ref rotations.Get(entity);
                rotation.Speed = rotation.Speed == 0 ? 1.0f : 0;
            }
        }

        if (window.IsKeyDown(Keys.Up))
        {
            shared.Camera.Position -= Vector3.UnitZ * 10 * shared.Time.UpdateDeltaTime;
            var shaders = AssetsService.GetAllShaders();
            foreach (ShaderAsset shaderAsset in shaders)
            {
                shaderAsset.Use();
                shaderAsset.SetMatrix4("view", shared.Camera.ViewMatrix);
                shaderAsset.SetMatrix4("projection", shared.Camera.ProjectionMatrix);
            }
        }
        else if (window.IsKeyDown(Keys.Down))
        {
            shared.Camera.Position += Vector3.UnitZ * 10 * shared.Time.UpdateDeltaTime;
            var shaders = AssetsService.GetAllShaders();
            foreach (ShaderAsset shaderAsset in shaders)
            {
                shaderAsset.Use();
                shaderAsset.SetMatrix4("view", shared.Camera.ViewMatrix);
                shaderAsset.SetMatrix4("projection", shared.Camera.ProjectionMatrix);
            }
        }
    }
}