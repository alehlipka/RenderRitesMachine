using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine;
using RenderRitesMachine.Assets;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.ECS;

public class RenderSystem : IEcsRunSystem
{
    private bool _isWireFrame;
    
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        
        var transforms = world.GetPool<TransformComponent>();
        var meshes = world.GetPool<MeshComponent>();
        var shaders = world.GetPool<ShaderComponent>();
        var textures = world.GetPool<TextureComponent>();
        
        EcsFilter filter = world
            .Filter<TransformComponent>()
            .Inc<MeshComponent>()
            .Inc<ShaderComponent>()
            .Inc<TextureComponent>()
            .End();

        foreach (int entity in filter)
        {
            TransformComponent transform = transforms.Get(entity);
            MeshComponent mesh = meshes.Get(entity);
            ShaderComponent shader = shaders.Get(entity);
            TextureComponent texture = textures.Get(entity);

            if (!mesh.IsVisible) continue;

            MeshAsset meshAsset = AssetsService.GetMesh(mesh.Name);
            ShaderAsset shaderAsset = AssetsService.GetShader(shader.Name);
            TextureAsset textureAsset = AssetsService.GetTexture(texture.Name);
            
            Matrix4 meshModelMatrix =
                Matrix4.CreateScale(transform.Scale) *
                Matrix4.CreateFromQuaternion(transform.Quaternion) *
                Matrix4.CreateTranslation(transform.Position);

            RenderService.Render(meshAsset, textureAsset, shaderAsset, meshModelMatrix);
            
            if (!RenderRites.Machine.Window!.IsKeyPressed(Keys.W)) return;
        
            _isWireFrame = !_isWireFrame;
            GL.PolygonMode(TriangleFace.FrontAndBack, _isWireFrame ? PolygonMode.Line : PolygonMode.Fill);
        }
    }
}