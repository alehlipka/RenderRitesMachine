using Leopotam.EcsLite;
using RenderRitesDemo.ECS.Features.Outline.Components;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.ECS;

public class RenderSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        var transforms = world.GetPool<Transform>();
        var meshes = world.GetPool<Mesh>();
        var textures = world.GetPool<ColorTexture>();
        var outlines = world.GetPool<Outline>();
        var boundings = world.GetPool<BoundingBoxTag>();
        
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .Inc<ColorTexture>()
            .End();

        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);
            
            if (!mesh.IsVisible) continue;
            
            Transform transform = transforms.Get(entity);
            ColorTexture colorTexture = textures.Get(entity);

            MeshAsset meshAsset = AssetsService.GetMesh(mesh.Name);
            ShaderAsset shaderAsset = AssetsService.GetShader("cel");
            TextureAsset textureAsset = AssetsService.GetTexture(colorTexture.Name);

            if (outlines.Has(entity) && outlines.Get(entity).IsVisible)
            {
                ShaderAsset outlineShaderAsset = AssetsService.GetShader("outline");
                RenderService.RenderOutline(meshAsset, outlineShaderAsset, transform.ModelMatrix, shared.Camera.Position);
            }

            RenderService.Render(meshAsset, shaderAsset, transform.ModelMatrix, textureAsset);
            
            if (boundings.Has(entity) && boundings.Get(entity).IsVisible)
            {
                ShaderAsset boundingShaderAsset = AssetsService.GetShader("bounding");
                BoundingBoxAsset boundingBoxAsset = AssetsService.GetBoundingBox(mesh.Name);
                RenderService.Render(boundingBoxAsset, boundingShaderAsset, transform.ModelMatrix);
            }
        }
    }
}