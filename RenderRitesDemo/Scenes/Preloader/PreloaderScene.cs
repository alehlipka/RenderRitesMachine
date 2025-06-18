using OpenTK.Mathematics;
using RenderRitesMachine;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Features.BoundingBox.Components;
using RenderRitesMachine.ECS.Features.BoundingBox.Systems;
using RenderRitesMachine.ECS.Features.PerspectiveCamera.Components;
using RenderRitesMachine.ECS.Systems;
using RenderRitesMachine.Output;
using RenderRitesMachine.Utilities;

namespace RenderRitesDemo.Scenes.Preloader;

public class PreloaderScene(string name) : Scene(name)
{
    protected override void Load()
    {
        RenderRites.Machine.Scenes.ForEach(item => item.Initialize());

        PerspectiveCameraComponent perspectiveCamera = new() { Position = new Vector3(0, 0, 25) };
        
        TextureComponent debugTexture = new(Path.Combine("Assets", "Textures", "debug.jpg"));
        TransformComponent cowTransform = new(new Vector3(-5, 0, 0), new RotationInfo { Axis = new Vector3(1.0f, 1.0f, 1.0f) });
        
        ShaderComponent defaultShader = new(Path.Combine("Assets", "Shaders", "Default"));
        ShaderComponent celShader = new(Path.Combine("Assets", "Shaders", "CelShading"));
        ShaderComponent outlineShader = new(Path.Combine("Assets", "Shaders", "Outline"));

        Entity cow = World.CreateEntity();
        World.AddComponent(cow, debugTexture);
        World.AddComponent(cow, perspectiveCamera);
        World.AddComponent(cow, celShader);
        World.AddComponent(cow, cowTransform);
        var cowMeshes = ModelCreator.Create(Path.Combine("Assets", "Objects", "cow.obj"));
        foreach (MeshComponent cowMesh in cowMeshes)
        {
            World.AddComponent(cow, cowMesh);
        }
        
        Entity sphere = World.CreateEntity();
        MeshComponent sphereMesh = ModelCreator.CreateSphere(2.5f, 20, 20);
        TransformComponent sphereTransform = new(
            position: new Vector3(5, 0, 0),
            rotation: new RotationInfo { Axis = new Vector3(1.0f, 1.0f, 1.0f) }
        );
        World.AddComponent(sphere, sphereMesh);
        World.AddComponent(sphere, debugTexture);
        World.AddComponent(sphere, perspectiveCamera);
        World.AddComponent(sphere, defaultShader);
        World.AddComponent(sphere, sphereTransform);
        
        World.AddSystem(new UpdateSystem());
        World.AddSystem(new ResizeSystem(outlineShader));
        World.AddSystem(new RenderSystem(outlineShader));
        
        #if DEBUG
        BoundingBoxShaderComponent boundingShader = new(Path.Combine("Assets", "Shaders", "Bounding"));
        
        foreach (MeshComponent cowMesh in cowMeshes)
        {
            Entity boundingBox = World.CreateEntity();
            World.AddComponent(boundingBox, BoundingBoxCreator.Create(cowMesh));
            World.AddComponent(boundingBox, perspectiveCamera);
            World.AddComponent(boundingBox, boundingShader);
            World.AddComponent(boundingBox, cowTransform);
        }
        
        Entity sphereBoundingBox = World.CreateEntity();
        World.AddComponent(sphereBoundingBox, BoundingBoxCreator.Create(sphereMesh));
        World.AddComponent(sphereBoundingBox, perspectiveCamera);
        World.AddComponent(sphereBoundingBox, boundingShader);
        World.AddComponent(sphereBoundingBox, sphereTransform);
        
        World.AddSystem(new BoundingBoxResizeSystem());
        World.AddSystem(new BoundingBoxRenderSystem());
        #endif
    }
}