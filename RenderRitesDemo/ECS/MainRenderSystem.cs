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
            
            int stencilId = (entity % 255) + 1;
            Console.WriteLine(stencilId);
            GL.StencilFunc(StencilFunction.Always, stencilId, 1);
            
            RenderService.Render(meshAsset, shaderAsset, transform.ModelMatrix, textureAsset);
        }
        
        GL.Disable(EnableCap.StencilTest);
    }
}