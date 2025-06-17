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
        
        TextureComponent debugTexture = new(Path.Combine("Assets", "Textures", "debug.jpg"));
        PerspectiveCameraComponent perspectiveCamera = new() { Position = new Vector3(0, 0, 25) };
        TransformComponent cowTransform = new(
            position: new Vector3(-5, 0, 0),
            rotation: new RotationInfo { Axis = new Vector3(0.0f, 1.0f, 0.0f) }
        );
        ShaderComponent defaultShader = new(Path.Combine("Assets", "Shaders", "Default"));
        ShaderComponent boundingShader = new(Path.Combine("Assets", "Shaders", "Bounding"));

        Entity cow = World.CreateEntity();
        World.AddComponent(cow, debugTexture);
        World.AddComponent(cow, perspectiveCamera);
        World.AddComponent(cow, defaultShader);
        World.AddComponent(cow, cowTransform);
        var cowMeshes = ModelLoader.Load(Path.Combine("Assets", "Objects", "cow.obj"));
        foreach (MeshComponent cowMesh in cowMeshes)
        {
            World.AddComponent(cow, cowMesh);
            #if DEBUG
            Entity boundingBox = World.CreateEntity();
            World.AddComponent(boundingBox, BoundingBoxCreator.Create(cowMesh));
            World.AddComponent(boundingBox, debugTexture);
            World.AddComponent(boundingBox, perspectiveCamera);
            World.AddComponent(boundingBox, boundingShader);
            World.AddComponent(boundingBox, cowTransform);
            #endif
        }
        
        Entity sphere = World.CreateEntity();
        MeshComponent sphereMesh = ModelLoader.CreateSphere(2.5f, 20, 20);
        TransformComponent sphereTransform = new(
            position: new Vector3(5, 0, 0),
            rotation: new RotationInfo { Axis = new Vector3(1.0f, 1.0f, 1.0f) }
        );
        World.AddComponent(sphere, sphereMesh);
        World.AddComponent(sphere, debugTexture);
        World.AddComponent(sphere, perspectiveCamera);
        World.AddComponent(sphere, defaultShader);
        World.AddComponent(sphere, sphereTransform);
        
        #if DEBUG
        Entity sphereBoundingBox = World.CreateEntity();
        World.AddComponent(sphereBoundingBox, BoundingBoxCreator.Create(sphereMesh));
        World.AddComponent(sphereBoundingBox, debugTexture);
        World.AddComponent(sphereBoundingBox, perspectiveCamera);
        World.AddComponent(sphereBoundingBox, boundingShader);
        World.AddComponent(sphereBoundingBox, sphereTransform);
        #endif
        
        World.AddSystem(new UpdateSystem());
        World.AddSystem(new ResizeSystem());
        World.AddSystem(new RenderSystem());
        
        #if DEBUG
        World.AddSystem(new BoundingRenderSystem());
        #endif
    }
}