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
        
        var shaders = world.GetPool<Shader>();
        var cameras = world.GetPool<PerspectiveCamera>();

        EcsFilter filter = world.Filter<Shader>().Inc<PerspectiveCamera>().End();
        Vector2i clientSize = RenderRites.Machine.Window!.ClientSize;
        
        GL.Viewport(0, 0, clientSize.X, clientSize.Y);

        foreach (int entity in filter)
        {
            Shader shader = shaders.Get(entity);
            ref PerspectiveCamera camera = ref cameras.Get(entity);
            
            camera.AspectRatio = clientSize.X / (float)clientSize.Y;
            ShaderAsset shaderAsset = AssetsService.GetShader(shader.Name);
            shaderAsset.Use();
            shaderAsset.SetMatrix4("view", camera.ViewMatrix);
            shaderAsset.SetMatrix4("projection", camera.ProjectionMatrix);

            if (world.GetPool<OutlineTag>().Has(entity))
            {
                ShaderAsset outlineShaderAsset = AssetsService.GetShader("outline");
                outlineShaderAsset.Use();
                outlineShaderAsset.SetMatrix4("view", camera.ViewMatrix);
                outlineShaderAsset.SetMatrix4("projection", camera.ProjectionMatrix);
            }

            if (world.GetPool<BoundingBoxTag>().Has(entity))
            {
                ShaderAsset boundingShaderAsset = AssetsService.GetShader("bounding");
                boundingShaderAsset.Use();
                boundingShaderAsset.SetMatrix4("view", camera.ViewMatrix);
                boundingShaderAsset.SetMatrix4("projection", camera.ProjectionMatrix);
            }
        }
    }
}