using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesDemo.ECS.Features.Rotation.Components;
using RenderRitesMachine.ECS;

namespace RenderRitesDemo.ECS.Features.Input.Systems;

public class InputUpdateSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        if (shared.Window == null) return;
        
        var window = shared.Window;

        if (window.IsKeyPressed(Keys.P))
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
            shared.Camera.Position += shared.Camera.Front * shared.Camera.Speed * shared.Time.UpdateDeltaTime;
        }
        else if (window.IsKeyDown(Keys.Down))
        {
            shared.Camera.Position -= shared.Camera.Front * shared.Camera.Speed * shared.Time.UpdateDeltaTime;
        }
        
        if (window.IsKeyDown(Keys.Left))
        {
            shared.Camera.Position -= shared.Camera.Right * shared.Camera.Speed * shared.Time.UpdateDeltaTime;
        }
        else if (window.IsKeyDown(Keys.Right))
        {
            shared.Camera.Position += shared.Camera.Right * shared.Camera.Speed * shared.Time.UpdateDeltaTime;
        }
        
        if (window.IsKeyDown(Keys.A))
        {
            shared.Camera.Position += shared.Camera.Up * shared.Camera.Speed * shared.Time.UpdateDeltaTime;
        }
        else if (window.IsKeyDown(Keys.D))
        {
            shared.Camera.Position -= shared.Camera.Up * shared.Camera.Speed * shared.Time.UpdateDeltaTime;
        }
        
        if (window.IsKeyDown(Keys.Q))
        {
            shared.Camera.Yaw -= shared.Camera.AngularSpeed * shared.Time.UpdateDeltaTime;
        }
        else if (window.IsKeyDown(Keys.E))
        {
            shared.Camera.Yaw += shared.Camera.AngularSpeed * shared.Time.UpdateDeltaTime;
        }
        
        if (window.IsKeyDown(Keys.W))
        {
            shared.Camera.Pitch -= shared.Camera.AngularSpeed * shared.Time.UpdateDeltaTime;
        }
        else if (window.IsKeyDown(Keys.S))
        {
            shared.Camera.Pitch += shared.Camera.AngularSpeed * shared.Time.UpdateDeltaTime;
        }

        // Матрицы камеры обновляются лениво, а шейдеры обновляются автоматически
        // в конце рендеринга через SystemSharedObject.UpdateActiveShaders()
    }
}