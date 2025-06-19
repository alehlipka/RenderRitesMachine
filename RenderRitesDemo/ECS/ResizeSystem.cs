using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine;
using RenderRitesMachine.Assets;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.ECS;

public class ResizeSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        
        var shaders = world.GetPool<ShaderComponent>();
        var cameras = world.GetPool<PerspectiveCameraComponent>();

        EcsFilter filter = world.Filter<ShaderComponent>().Inc<PerspectiveCameraComponent>().End();
        Vector2i clientSize = RenderRites.Machine.Window!.ClientSize;
        
        GL.Viewport(0, 0, clientSize.X, clientSize.Y);

        foreach (int entity in filter)
        {
            ShaderComponent shader = shaders.Get(entity);
            ref PerspectiveCameraComponent camera = ref cameras.Get(entity);
            
            camera.AspectRatio = clientSize.X / (float)clientSize.Y;
            ShaderAsset shaderAsset = AssetsService.GetShader(shader.Name);
            shaderAsset.Use();
            shaderAsset.SetMatrix4("view", camera.ViewMatrix);
            shaderAsset.SetMatrix4("projection", camera.ProjectionMatrix);
        }
    }
}