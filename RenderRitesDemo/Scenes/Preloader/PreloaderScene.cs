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
        ShaderComponent shader = new(Path.Combine("Assets", "Shaders", "Default"));
        ShaderComponent boundingShader = new(Path.Combine("Assets", "Shaders", "Bounding"));
        TransformComponent transform = new(
            position: Vector3.Zero,
            rotation: new RotationInfo { Axis = new Vector3(0.0f, 1.0f, 0.0f) }
        );
        
        // GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
        // GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);

        Entity cow = World.CreateEntity();
        World.AddComponent(cow, texture);
        World.AddComponent(cow, camera);
        World.AddComponent(cow, shader);
        World.AddComponent(cow, transform);
        var cubeMeshes = ModelLoader.Load(Path.Combine("Assets", "Objects", "cow.obj"));
        foreach (MeshComponent cubeMesh in cubeMeshes)
        {
            World.AddComponent(cow, cubeMesh);
            
            Entity boundingBox = World.CreateEntity();
            World.AddComponent(boundingBox, BoundingBoxCreator.CreateForEntity(World, cow));
            World.AddComponent(boundingBox, texture);
            World.AddComponent(boundingBox, camera);
            World.AddComponent(boundingBox, boundingShader);
            World.AddComponent(boundingBox, new TransformComponent(Vector3.Zero));
        }
        
        World.AddSystem(new RenderSystem());
        World.AddSystem(new BoundingBoxRenderSystem());
    }

    protected override void Unload()
    {
        World.Dispose();
    }
}