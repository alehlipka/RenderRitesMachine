using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.Assets;
using RenderRitesMachine.Configuration;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.ECS;

public class MainRenderSystem : IEcsRunSystem
{
    // MaxStencilValue moved to RenderConstants
    
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        
        var transforms = world.GetPool<Transform>();
        var meshes = world.GetPool<Mesh>();
        var textures = world.GetPool<ColorTexture>();
        
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .Inc<ColorTexture>()
            .End();
        
        GL.Enable(EnableCap.StencilTest);
        GL.StencilMask(0xFF);
        
        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);
            
            if (!mesh.IsVisible) continue;
            
            Transform transform = transforms.Get(entity);
            ColorTexture colorTexture = textures.Get(entity);
            SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
            MeshAsset meshAsset = shared.Assets.GetMesh(mesh.Name);
            ShaderAsset shaderAsset = shared.Assets.GetShader("cel");
            TextureAsset textureAsset = shared.Assets.GetTexture(colorTexture.Name);
            
            shared.MarkShaderActive(shaderAsset.Id);
            
            int stencilId = entity % RenderConstants.MaxStencilValue + 1;
            GL.StencilFunc(StencilFunction.Gequal, stencilId, 0xFF);
            
            RenderService.Render(meshAsset, shaderAsset, transform.ModelMatrix, textureAsset);
        }
        
        GL.Disable(EnableCap.StencilTest);
    }
}