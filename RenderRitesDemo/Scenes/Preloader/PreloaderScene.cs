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
        PerspectiveCameraComponent camera = new() { Position = new Vector3(0, 0, 25) };
        TransformComponent transform = new(
            position: Vector3.Zero,
            rotation: new RotationInfo { Axis = new Vector3(1.0f, 1.0f, 1.0f) }
        );

        Entity cow = World.CreateEntity();
        World.AddComponent(cow, texture);
        World.AddComponent(cow, camera);
        World.AddComponent(cow, new ShaderComponent(Path.Combine("Assets", "Shaders", "Default")));
        World.AddComponent(cow, transform);
        var cubeMeshes = ModelLoader.Load(Path.Combine("Assets", "Objects", "cow.obj"));
        foreach (MeshComponent cubeMesh in cubeMeshes)
        {
            World.AddComponent(cow, cubeMesh);
            #if DEBUG
            Entity boundingBox = World.CreateEntity();
            World.AddComponent(boundingBox, BoundingBoxCreator.CreateForEntity(World, cow));
            World.AddComponent(boundingBox, texture);
            World.AddComponent(boundingBox, camera);
            World.AddComponent(boundingBox, new ShaderComponent(Path.Combine("Assets", "Shaders", "Bounding")));
            World.AddComponent(boundingBox, new TransformComponent(Vector3.Zero));
            #endif
        }
        
        World.AddSystem(new UpdateSystem());
        World.AddSystem(new ResizeSystem());
        World.AddSystem(new RenderSystem());
        
        #if DEBUG
        World.AddSystem(new BoundingUpdateSystem());
        World.AddSystem(new BoundingRenderSystem());
        #endif
    }

    protected override void Unload() { }
}