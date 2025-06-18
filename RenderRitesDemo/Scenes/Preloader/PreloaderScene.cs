using OpenTK.Mathematics;
using RenderRitesMachine;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Features.BoundingBox.Components;
using RenderRitesMachine.ECS.Features.BoundingBox.Systems;
using RenderRitesMachine.ECS.Features.CelShader.Components;
using RenderRitesMachine.ECS.Features.CelShader.Systems;
using RenderRitesMachine.ECS.Features.Mesh.Components;
using RenderRitesMachine.ECS.Features.Outline.Components;
using RenderRitesMachine.ECS.Features.Outline.Systems;
using RenderRitesMachine.ECS.Features.PerspectiveCamera.Components;
using RenderRitesMachine.ECS.Features.Texture.Components;
using RenderRitesMachine.ECS.Features.Transform.Components;
using RenderRitesMachine.ECS.Features.Transform.Systems;
using RenderRitesMachine.ECS.Features.Window.Systems;
using RenderRitesMachine.ECS.Features.Wireframe.System;
using RenderRitesMachine.Output;
using RenderRitesMachine.Utilities;

namespace RenderRitesDemo.Scenes.Preloader;

public class PreloaderScene(string name) : Scene(name)
{
    protected override void Load()
    {
        RenderRites.Machine.Scenes.ForEach(item => item.Initialize());

        PerspectiveCameraComponent perspectiveCamera = new() { Position = new Vector3(0, 0, 5) };
        TextureComponent debugTexture = new(Path.Combine("Assets", "Textures", "debug.jpg"));
        OutlineShaderComponent outlineShader = new(Path.Combine("Assets", "Shaders", "Outline"));
        CelShaderComponent celCelShader = new(Path.Combine("Assets", "Shaders", "CelShading"));
        MeshComponent sphereMesh = ModelCreator.CreateSphere(1, 40, 40);
        TransformComponent sphereTransform = new(new Vector3(0, 0, 0), new RotationInfo { Axis = new Vector3(1.0f, 1.0f, 1.0f) });

        Entity sphere = World.CreateEntity();

        World.AddComponent(sphere, sphereMesh);
        World.AddComponent(sphere, debugTexture);
        World.AddComponent(sphere, perspectiveCamera);
        World.AddComponent(sphere, celCelShader);
        World.AddComponent(sphere, outlineShader);
        World.AddComponent(sphere, sphereTransform);
        
        World.AddSystem(new WindowResizeSystem());
        World.AddSystem(new WireFrameUpdateSystem());
        World.AddSystem(new TransformUpdateSystem());
        World.AddSystem(new OutlineResizeSystem());
        World.AddSystem(new OutlineRenderSystem());
        World.AddSystem(new CelShaderResizeSystem());
        World.AddSystem(new CelShaderRenderSystem());
        
        #if DEBUG
        BoundingBoxShaderComponent boundingShader = new(Path.Combine("Assets", "Shaders", "Bounding"));
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