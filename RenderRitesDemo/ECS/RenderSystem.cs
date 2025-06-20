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
        
        var transforms = world.GetPool<Transform>();
        var meshes = world.GetPool<Mesh>();
        var shaders = world.GetPool<Shader>();
        var textures = world.GetPool<ColorTexture>();
        var cameras = world.GetPool<PerspectiveCamera>();
        
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .Inc<Shader>()
            .Inc<ColorTexture>()
            .Inc<PerspectiveCamera>()
            .End();

        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);
            
            if (!mesh.IsVisible) continue;
            
            Transform transform = transforms.Get(entity);
            Shader shader = shaders.Get(entity);
            ColorTexture colorTexture = textures.Get(entity);
            PerspectiveCamera perspectiveCamera = cameras.Get(entity);

            MeshAsset meshAsset = AssetsService.GetMesh(mesh.Name);
            ShaderAsset shaderAsset = AssetsService.GetShader(shader.Name);
            TextureAsset textureAsset = AssetsService.GetTexture(colorTexture.Name);
            
            Matrix4 meshModelMatrix =
                Matrix4.CreateScale(transform.Scale) *
                Matrix4.CreateFromQuaternion(transform.Quaternion) *
                Matrix4.CreateTranslation(transform.Position);
            
            if (world.GetPool<OutlineTag>().Has(entity))
            {
                ShaderAsset outlineShaderAsset = AssetsService.GetShader("outline");
                RenderService.RenderOutline(meshAsset, outlineShaderAsset, meshModelMatrix, perspectiveCamera.Position);
            }

            RenderService.Render(meshAsset, shaderAsset, meshModelMatrix, textureAsset);
            
            if (world.GetPool<BoundingBoxTag>().Has(entity))
            {
                ShaderAsset boundingShaderAsset = AssetsService.GetShader("bounding");
                BoundingBoxAsset boundingBoxAsset = AssetsService.GetBoundingBox(mesh.Name);
                RenderService.Render(boundingBoxAsset, boundingShaderAsset, meshModelMatrix);
            }
            
            if (!RenderRites.Machine.Window!.IsKeyPressed(Keys.W)) return;
        
            _isWireFrame = !_isWireFrame;
            GL.PolygonMode(TriangleFace.FrontAndBack, _isWireFrame ? PolygonMode.Line : PolygonMode.Fill);
        }
    }
}