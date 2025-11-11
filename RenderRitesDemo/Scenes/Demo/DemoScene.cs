using Leopotam.EcsLite;
using OpenTK.Mathematics;
using RenderRitesDemo.ECS;
using RenderRitesDemo.ECS.Features.BoundingBox.Components;
using RenderRitesDemo.ECS.Features.BoundingBox.Systems;
using RenderRitesDemo.ECS.Features.Input.Systems;
using RenderRitesDemo.ECS.Features.Outline.Components;
using RenderRitesDemo.ECS.Features.Outline.Systems;
using RenderRitesDemo.ECS.Features.Rotation.Components;
using RenderRitesDemo.ECS.Features.Rotation.Systems;
using RenderRitesDemo.ECS.Features.SceneSwitch;
using RenderRitesMachine;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Systems;
using RenderRitesMachine.Output;

namespace RenderRitesDemo.Scenes.Demo;

public class DemoScene(string name) : Scene(name)
{
    private void LoadAssets()
    {
        Assets.AddShader("cel", Path.Combine("Assets", "Shaders", "CelShading"));
        Assets.AddShader("outline", Path.Combine("Assets", "Shaders", "Outline"));
        Assets.AddShader("bounding", Path.Combine("Assets", "Shaders", "Bounding"));

        Assets.AddMeshFromFile("cow", Path.Combine("Assets", "Objects", "cow.obj"));
        Assets.AddSphere("sphere", 2, 20, 20);
        Assets.AddBoundingBox("cow");
        Assets.AddBoundingBox("sphere");

        Assets.AddTexture("debug", TextureType.ColorMap, Path.Combine("Assets", "Textures", "debug.jpg"));
    }

    private void CreateCow(
        EcsPool<OutlineTag> outlines,
        EcsPool<BoundingBoxTag> boundingBoxes,
        EcsPool<Transform> transforms,
        EcsPool<ColorTexture> colorTextures,
        EcsPool<Mesh> meshes,
        EcsPool<RotationTag> rotations
    )
    {
        int cow = World.NewEntity();
        outlines.Add(cow);
        boundingBoxes.Add(cow);
        ref Transform cowTransform = ref transforms.Add(cow);
        cowTransform.Position = new Vector3(-5.0f, 0.0f, 0.0f);
        cowTransform.RotationAxis = Vector3.One;
        ref ColorTexture cowColorTexture = ref colorTextures.Add(cow);
        cowColorTexture.Name = "debug";
        ref Mesh cowMesh = ref meshes.Add(cow);
        cowMesh.Name = "cow";
        ref RotationTag cowRotation = ref rotations.Add(cow);
        cowRotation.Speed = 1.0f;
    }

    private void CreateSphere(
        EcsPool<OutlineTag> outlines,
        EcsPool<BoundingBoxTag> boundingBoxes,
        EcsPool<Transform> transforms,
        EcsPool<ColorTexture> colorTextures,
        EcsPool<Mesh> meshes,
        EcsPool<RotationTag> rotations
    )
    {
        int sphere = World.NewEntity();
        outlines.Add(sphere);
        boundingBoxes.Add(sphere);
        ref Transform sphereTransform = ref transforms.Add(sphere);
        sphereTransform.Position = new Vector3(5.0f, 0.0f, 0.0f);
        sphereTransform.RotationAxis = Vector3.One;
        ref ColorTexture sphereColorTexture = ref colorTextures.Add(sphere);
        sphereColorTexture.Name = "debug";
        ref Mesh sphereMesh = ref meshes.Add(sphere);
        sphereMesh.Name = "sphere";
        ref RotationTag sphereRotation = ref rotations.Add(sphere);
        sphereRotation.Speed = 1.0f;
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
        var rotations = World.GetPool<RotationTag>();

        CreateCow(outlines, boundingBoxes, transforms, colorTextures, meshes, rotations);
        CreateSphere(outlines, boundingBoxes, transforms, colorTextures, meshes, rotations);

        Camera.Position = new Vector3(0.0f, 0.0f, 10.0f);

        ResizeSystems.Add(new MainResizeSystem());

        UpdateSystems.Add(new InputUpdateSystem());
        UpdateSystems.Add(new RotationUpdateSystem());
        UpdateSystems.Add(new OutlineUpdateSystem());
        UpdateSystems.Add(new SceneSwitchSystem());

        RenderSystems.Add(new MainRenderSystem());
        RenderSystems.Add(new OutlineRenderSystem());
        RenderSystems.Add(new BoundingBoxRenderSystem());
        RenderSystems.Add(new GuiRenderSystem());
    }
}
