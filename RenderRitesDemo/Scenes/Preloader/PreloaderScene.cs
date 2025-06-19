using OpenTK.Mathematics;
using RenderRitesDemo.ECS;
using RenderRitesMachine;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.Scenes.Preloader;

public class PreloaderScene(string name) : Scene(name)
{
    protected override void OnLoad()
    {
        RenderRites.Machine.Scenes.ForEach(item => item.Initialize());
        AssetsService.AddSphere("demo_sphere", 1.0f, 20, 20);
        AssetsService.AddShader("cel_shader", Path.Combine("Assets", "Shaders", "CelShading"));
        AssetsService.AddTexture("demo_texture", Path.Combine("Assets", "Textures", "debug.jpg"));

        int sphere = World.NewEntity();

        ref TransformComponent transform = ref World.GetPool<TransformComponent>().Add(sphere);
        transform.Position = new Vector3(0.0f, 0.0f, -5.0f);
        
        ref MeshComponent mesh = ref World.GetPool<MeshComponent>().Add(sphere);
        mesh.Name = "demo_sphere";
        
        ref ShaderComponent shader = ref World.GetPool<ShaderComponent>().Add(sphere);
        shader.Name = "cel_shader";
        
        ref TextureComponent texture = ref World.GetPool<TextureComponent>().Add(sphere);
        texture.Name = "demo_texture";
        
        ref PerspectiveCameraComponent camera = ref World.GetPool<PerspectiveCameraComponent>().Add(sphere);
        camera.Position = new Vector3(0.0f, 0.0f, 5.0f);

        RenderSystems.Add(new RenderSystem());
        ResizeSystems.Add(new ResizeSystem());
    }
}
