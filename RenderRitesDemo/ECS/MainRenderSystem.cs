using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.ECS;

public class MainRenderSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        var transforms = world.GetPool<Transform>();
        var meshes = world.GetPool<Mesh>();
        var textures = world.GetPool<ColorTexture>();
        
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .Inc<ColorTexture>()
            .End();
        
        GL.Enable(EnableCap.StencilTest);
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
        GL.StencilFunc(StencilFunction.Always, 1, 1);
        GL.StencilMask(1);

        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);
            
            if (!mesh.IsVisible) continue;
            
            Transform transform = transforms.Get(entity);
            ColorTexture colorTexture = textures.Get(entity);
            MeshAsset meshAsset = AssetsService.GetMesh(mesh.Name);
            ShaderAsset shaderAsset = AssetsService.GetShader("cel");
            TextureAsset textureAsset = AssetsService.GetTexture(colorTexture.Name);
            
            RenderService.Render(meshAsset, shaderAsset, transform.ModelMatrix, textureAsset);
        }
    }
}