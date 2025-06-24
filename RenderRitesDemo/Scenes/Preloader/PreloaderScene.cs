using Leopotam.EcsLite;
using OpenTK.Mathematics;
using RenderRitesDemo.ECS;
using RenderRitesDemo.ECS.Features.BoundingBox.Components;
using RenderRitesDemo.ECS.Features.BoundingBox.Systems;
using RenderRitesDemo.ECS.Features.Outline.Components;
using RenderRitesDemo.ECS.Features.Outline.Systems;
using RenderRitesMachine;
using RenderRitesMachine.Assets;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.Scenes.Preloader;

public class PreloaderScene(string name) : Scene(name)
{
    private void LoadAssets()
    {
        AssetsService.AddShader("cel", Path.Combine("Assets", "Shaders", "CelShading"));
        AssetsService.AddShader("outline", Path.Combine("Assets", "Shaders", "Outline"));
        AssetsService.AddShader("bounding", Path.Combine("Assets", "Shaders", "Bounding"));

        AssetsService.AddMeshFromFile("cow", Path.Combine("Assets", "Objects", "cow.obj"));
        AssetsService.AddSphere("sphere", 2, 20, 20);
        AssetsService.AddBoundingBox("cow");
        AssetsService.AddBoundingBox("sphere");

        AssetsService.AddTexture("debug", TextureType.ColorMap, Path.Combine("Assets", "Textures", "debug.jpg"));
    }
    
    private void CreateCow(
        EcsPool<OutlineTag> outlines,
        EcsPool<BoundingBoxTag> boundingBoxes,
        EcsPool<Transform> transforms,
        EcsPool<ColorTexture> colorTextures,
        EcsPool<Mesh> meshes
    )
    {
        int cow = World.NewEntity();
        outlines.Add(cow);
        boundingBoxes.Add(cow);
        ref Transform cowTransform = ref transforms.Add(cow);
        cowTransform.Position = new Vector3(0.0f, 0.0f, 0.0f);
        cowTransform.RotationAxis = Vector3.One;
        ref ColorTexture cowColorTexture = ref colorTextures.Add(cow);
        cowColorTexture.Name = "debug";
        ref Mesh cowMesh = ref meshes.Add(cow);
        cowMesh.Name = "cow";
    }

    private void CreateSphere(
        EcsPool<OutlineTag> outlines,
        EcsPool<BoundingBoxTag> boundingBoxes,
        EcsPool<Transform> transforms,
        EcsPool<ColorTexture> colorTextures,
        EcsPool<Mesh> meshes
    )
    {
        int sphere = World.NewEntity();
        outlines.Add(sphere);
        boundingBoxes.Add(sphere);
        ref Transform sphereTransform = ref transforms.Add(sphere);
        sphereTransform.Position = new Vector3(0.0f, 0.0f, 0.0f);
        sphereTransform.RotationAxis = Vector3.One;
        ref ColorTexture sphereColorTexture = ref colorTextures.Add(sphere);
        sphereColorTexture.Name = "debug";
        ref Mesh sphereMesh = ref meshes.Add(sphere);
        sphereMesh.Name = "sphere";
    }
    
    protected override void OnLoad()
    {
        RenderRites.Machine.Scenes.ForEach(item => item.Initialize());
        
        LoadAssets();
        
        var outlines = World.GetPool<OutlineTag>();
        var boundingBoxes = World.GetPool<BoundingBoxTag>();
        var transforms = World.GetPool<Transform>();
        var colorTextures = World.GetPool<ColorTexture>();
        var meshes = World.GetPool<Mesh>();
        
        CreateCow(outlines, boundingBoxes, transforms, colorTextures, meshes);
        CreateSphere(outlines, boundingBoxes, transforms, colorTextures, meshes);

        Camera.Position = new Vector3(0.0f, 0.0f, 25.0f);

        ResizeSystems.Add(new MainResizeSystem());
        ResizeSystems.Add(new OutlineResizeSystem());
        ResizeSystems.Add(new BoundingBoxResizeSystem());
        
        UpdateSystems.Add(new MainUpdateSystem());
        UpdateSystems.Add(new OutlineUpdateSystem());

        RenderSystems.Add(new MainRenderSystem());
        RenderSystems.Add(new OutlineRenderSystem());
        RenderSystems.Add(new BoundingBoxRenderSystem());
    }
}
