using OpenTK.Mathematics;
using RenderRitesDemo.ECS;
using RenderRitesMachine;
using RenderRitesMachine.Assets;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.Scenes.Preloader;

public class PreloaderScene(string name) : Scene(name)
{
    protected override void OnLoad()
    {
        RenderRites.Machine.Scenes.ForEach(item => item.Initialize());
        AssetsService.AddSphere("sphere", 1.0f, 50, 50);
        AssetsService.AddShader("cel", Path.Combine("Assets", "Shaders", "CelShading"));
        AssetsService.AddShader("outline", Path.Combine("Assets", "Shaders", "Outline"));
        AssetsService.AddShader("bounding", Path.Combine("Assets", "Shaders", "Bounding"));
        AssetsService.AddTexture("debug", TextureType.ColorMap, Path.Combine("Assets", "Textures", "debug.jpg"));
        AssetsService.AddBoundingBox("sphere");

        int sphere = World.NewEntity();

        World.GetPool<OutlineTag>().Add(sphere);
        World.GetPool<BoundingBoxTag>().Add(sphere);
        
        ref Transform transform = ref World.GetPool<Transform>().Add(sphere);
        transform.Position = new Vector3(0.0f, 0.0f, -5.0f);
        
        ref ColorTexture colorTexture = ref World.GetPool<ColorTexture>().Add(sphere);
        colorTexture.Name = "debug";
        
        ref Mesh mesh = ref World.GetPool<Mesh>().Add(sphere);
        mesh.Name = "sphere";
        
        ref Shader shader = ref World.GetPool<Shader>().Add(sphere);
        shader.Name = "cel";
        
        ref PerspectiveCamera camera = ref World.GetPool<PerspectiveCamera>().Add(sphere);
        camera.Position = new Vector3(0.0f, 0.0f, 0.0f);
        camera.Target = transform.Position;
        
        ResizeSystems.Add(new ResizeSystem());
        UpdateSystems.Add(new UpdateSystem());
        RenderSystems.Add(new RenderSystem());
    }
}
