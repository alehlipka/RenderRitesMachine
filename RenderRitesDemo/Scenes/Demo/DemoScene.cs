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
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Systems;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.Scenes.Demo;

public class DemoScene(string name, IAssetsService assetsService, ITimeService timeService, IRenderService renderService, IGuiService guiService, IAudioService audioService, ISceneManager sceneManager, ILogger logger)
    : Scene(name, assetsService, timeService, renderService, guiService, audioService, sceneManager, logger)
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
        cowTransform.Position = new Vector3(-15.0f, 0.0f, 0.0f);
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
        EcsPool<RotationTag> rotations,
        Vector3 position,
        float rotationSpeed = 1.0f
    )
    {
        int sphere = World.NewEntity();
        outlines.Add(sphere);
        boundingBoxes.Add(sphere);
        ref Transform sphereTransform = ref transforms.Add(sphere);
        sphereTransform.Position = position;
        sphereTransform.RotationAxis = Vector3.One;
        ref ColorTexture sphereColorTexture = ref colorTextures.Add(sphere);
        sphereColorTexture.Name = "debug";
        ref Mesh sphereMesh = ref meshes.Add(sphere);
        sphereMesh.Name = "sphere";
        ref RotationTag sphereRotation = ref rotations.Add(sphere);
        sphereRotation.Speed = rotationSpeed;
    }

    private void CreateManyObjects(
        EcsPool<OutlineTag> outlines,
        EcsPool<BoundingBoxTag> boundingBoxes,
        EcsPool<Transform> transforms,
        EcsPool<ColorTexture> colorTextures,
        EcsPool<Mesh> meshes,
        EcsPool<RotationTag> rotations
    )
    {
        const int gridSize = 50;
        const float spacing = 8.0f;
        const float startOffset = -(gridSize - 1) * spacing * 0.5f;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 position = new Vector3(
                    startOffset + x * spacing,
                    0.0f,
                    startOffset + z * spacing
                );
                CreateSphere(outlines, boundingBoxes, transforms, colorTextures, meshes, rotations, position, 0.5f);
            }
        }
    }

    protected override void OnLoad()
    {
        LoadAssets();

        EcsPool<OutlineTag>? outlines = World.GetPool<OutlineTag>();
        EcsPool<BoundingBoxTag>? boundingBoxes = World.GetPool<BoundingBoxTag>();
        EcsPool<Transform>? transforms = World.GetPool<Transform>();
        EcsPool<ColorTexture>? colorTextures = World.GetPool<ColorTexture>();
        EcsPool<Mesh>? meshes = World.GetPool<Mesh>();
        EcsPool<RotationTag>? rotations = World.GetPool<RotationTag>();

        CreateCow(outlines, boundingBoxes, transforms, colorTextures, meshes, rotations);
        CreateSphere(outlines, boundingBoxes, transforms, colorTextures, meshes, rotations, new Vector3(15.0f, 0.0f, 0.0f));

        CreateManyObjects(outlines, boundingBoxes, transforms, colorTextures, meshes, rotations);

        Camera.Position = new Vector3(0.0f, 0.0f, 10.0f);

        ResizeSystems.Add(new MainResizeSystem());

        UpdateSystems.Add(new InputUpdateSystem());
        UpdateSystems.Add(new RotationUpdateSystem());
        UpdateSystems.Add(new OutlineUpdateSystem());
        UpdateSystems.Add(new BoundingBoxClickSystem());
        UpdateSystems.Add(new SceneSwitchSystem());

        RenderSystems.Add(new MainRenderSystem());
        RenderSystems.Add(new OutlineRenderSystem());
        RenderSystems.Add(new BoundingBoxRenderSystem());
        RenderSystems.Add(new GuiRenderSystem());
    }
}
