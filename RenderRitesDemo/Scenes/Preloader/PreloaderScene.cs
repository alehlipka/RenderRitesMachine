using OpenTK.Mathematics;
using RenderRitesDemo.Scenes.Preloader.ECS.Components;
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
        
        Entity cube = World.CreateEntity();
        World.AddComponent(cube, new TextureComponent(Path.Combine("Assets", "Textures", "debug.jpg")));
        World.AddComponent(cube, new PerspectiveCameraComponent
        {
            Position = new Vector3(0, 0, 3)
        });
        World.AddComponent(cube, new ShaderComponent(Path.Combine("Assets", "Shaders", "Default")));
        World.AddComponent(cube, new TransformComponent(
            position: new Vector3(0, 0, 0),
            rotation: new RotationInfo { Axis = new Vector3(0.5f, 1, 0.25f), Angle = 0 }
        ));
        World.AddComponent(cube, CubeMeshComponentFactory.CreateCube());
        World.AddSystem(new RenderSystem());
    }

    protected override void Unload()
    {
        World.Dispose();
    }
}