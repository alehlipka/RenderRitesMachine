using Leopotam.EcsLite;
using OpenTK.Mathematics;
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
        var shaders = world.GetPool<Shader>();
        var textures = world.GetPool<ColorTexture>();
        var outlines = world.GetPool<OutlineTag>();
        var boundings = world.GetPool<BoundingBoxTag>();
        
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .Inc<Shader>()
            .Inc<ColorTexture>()
            .End();

        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);
            
            if (!mesh.IsVisible) continue;
            
            Transform transform = transforms.Get(entity);
            Shader shader = shaders.Get(entity);
            ColorTexture colorTexture = textures.Get(entity);

            MeshAsset meshAsset = AssetsService.GetMesh(mesh.Name);
            ShaderAsset shaderAsset = AssetsService.GetShader(shader.Name);
            TextureAsset textureAsset = AssetsService.GetTexture(colorTexture.Name);
            
            Matrix4 meshModelMatrix =
                Matrix4.CreateScale(transform.Scale) *
                Matrix4.CreateFromQuaternion(Quaternion.FromAxisAngle(transform.RotationAxis, transform.RotationAngle)) *
                Matrix4.CreateTranslation(transform.Position);

            if (outlines.Has(entity) && outlines.Get(entity).IsVisible)
            {
                ShaderAsset outlineShaderAsset = AssetsService.GetShader("outline");
                RenderService.RenderOutline(meshAsset, outlineShaderAsset, meshModelMatrix, shared.Camera.Position);
            }

            RenderService.Render(meshAsset, shaderAsset, meshModelMatrix, textureAsset);
            
            if (boundings.Has(entity) && boundings.Get(entity).IsVisible)
            {
                ShaderAsset boundingShaderAsset = AssetsService.GetShader("bounding");
                BoundingBoxAsset boundingBoxAsset = AssetsService.GetBoundingBox(mesh.Name);
                RenderService.Render(boundingBoxAsset, boundingShaderAsset, meshModelMatrix);
            }
        }
    }
}