using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using RenderRitesDemo.ECS.Features.Outline.Components;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.ECS.Features.Outline.Systems;

public class OutlineRenderSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        var transforms = world.GetPool<Transform>();
        var meshes = world.GetPool<Mesh>();
        var outlines = world.GetPool<OutlineTag>();
        
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .Inc<OutlineTag>()
            .End();
        
        GL.Enable(EnableCap.StencilTest);
        GL.StencilMask(0);
        
        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);
            if (!mesh.IsVisible) continue;
            
            OutlineTag outline = outlines.Get(entity);
            if (!outline.IsVisible) continue;
            
            Transform transform = transforms.Get(entity);
            MeshAsset meshAsset = AssetsService.GetMesh(mesh.Name);
            ShaderAsset outlineShaderAsset = AssetsService.GetShader("outline");
            
            int stencilId = (entity % 255) + 1;
            GL.StencilFunc(StencilFunction.Notequal, stencilId, 1);
            
            RenderService.RenderOutline(meshAsset, outlineShaderAsset, transform.ModelMatrix, shared.Camera.Position);
        }
        
        GL.Disable(EnableCap.StencilTest);
    }
}
