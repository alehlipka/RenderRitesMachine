using OpenTK.Mathematics;
using RenderRitesMachine;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Systems;
using RenderRitesMachine.Output;
using RenderRitesMachine.Utilities;

namespace RenderRitesDemo.Scenes.Preloader;

public class PreloaderScene(string name) : Scene(name)
{
    protected override void Load()
    {
        RenderRites.Machine.Scenes.ForEach(item => item.Initialize());
        
        TextureComponent texture = new(Path.Combine("Assets", "Textures", "debug.jpg"));
        PerspectiveCameraComponent camera = new() { Position = new Vector3(0, 0, 100) };
        ShaderComponent shader = new(Path.Combine("Assets", "Shaders", "Default"));
        
        RenderSystem render = new();

        Entity test = World.CreateEntity();
        World.AddComponent(test, texture);
        World.AddComponent(test, camera);
        World.AddComponent(test, shader);
        World.AddComponent(test, new TransformComponent(
            position: Vector3.Zero,
            rotation: new RotationInfo { Axis = new Vector3(0, 1, 0), Angle = 0 }
        ));
        var cubeMeshes = ModelLoader.Load(Path.Combine("Assets", "Objects", "teddy.obj"));
        foreach (MeshComponent cubeMesh in cubeMeshes)
        {
            World.AddComponent(test, cubeMesh);
        }
        
        World.AddSystem(render);
    }

    protected override void Unload()
    {
        World.Dispose();
    }
}