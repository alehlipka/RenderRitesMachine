using OpenTK.Mathematics;
using RenderRitesDemo.ECS;
using RenderRitesMachine;
using RenderRitesMachine.Assets;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.Scenes.Preloader;

public class PreloaderScene(string name) : Scene(name)
{
    protected override void OnLoad()
    {
        RenderRites.Machine.Scenes.ForEach(item => item.Initialize());

        AssetsService.AddMeshFromFile("cow", Path.Combine("Assets", "Objects", "cow.obj"));
        AssetsService.AddSphere("sphere", 2, 20, 20);
        AssetsService.AddBoundingBox("cow");
        AssetsService.AddBoundingBox("sphere");

        AssetsService.AddShader("cel", Path.Combine("Assets", "Shaders", "CelShading"));
        AssetsService.AddShader("outline", Path.Combine("Assets", "Shaders", "Outline"));
        AssetsService.AddShader("bounding", Path.Combine("Assets", "Shaders", "Bounding"));

        AssetsService.AddTexture("debug", TextureType.ColorMap, Path.Combine("Assets", "Textures", "debug.jpg"));

        int cow = World.NewEntity();
        int sphere = World.NewEntity();

        var outlines = World.GetPool<OutlineTag>();
        ref OutlineTag cowOutline = ref outlines.Add(cow);
        ref OutlineTag sphereOutline = ref outlines.Add(sphere);

        var boundingBoxes = World.GetPool<BoundingBoxTag>();
        ref BoundingBoxTag cowBoundingBox = ref boundingBoxes.Add(cow);
        ref BoundingBoxTag sphereBoundingBox = ref boundingBoxes.Add(sphere);
        #if DEBUG
        cowBoundingBox.IsVisible = true;
        sphereBoundingBox.IsVisible = true;
        #endif

        var transforms = World.GetPool<Transform>();
        ref Transform cowTransform = ref transforms.Add(cow);
        ref Transform sphereTransform = ref transforms.Add(sphere);
        cowTransform.Position = new Vector3(-5.0f, 0.0f, 0.0f);
        cowTransform.RotationAxis = Vector3.One;
        sphereTransform.Position = new Vector3(5.0f, 0.0f, 0.0f);
        sphereTransform.RotationAxis = Vector3.One;

        var colorTextures = World.GetPool<ColorTexture>();
        ref ColorTexture cowColorTexture = ref colorTextures.Add(cow);
        ref ColorTexture sphereColorTexture = ref colorTextures.Add(sphere);
        cowColorTexture.Name = "debug";
        sphereColorTexture.Name = "debug";

        var meshes = World.GetPool<Mesh>();
        ref Mesh cowMesh = ref meshes.Add(cow);
        ref Mesh sphereMesh = ref meshes.Add(sphere);
        cowMesh.Name = "cow";
        sphereMesh.Name = "sphere";

        var shaders = World.GetPool<Shader>();
        ref Shader cowShader = ref shaders.Add(cow);
        ref Shader sphereShader = ref shaders.Add(sphere);
        cowShader.Name = "cel";
        sphereShader.Name = "cel";

        Camera.Position = new(0.0f, 0.0f, 25.0f);

        ResizeSystems.Add(new ResizeSystem());
        UpdateSystems.Add(new UpdateSystem());
        RenderSystems.Add(new RenderSystem());
    }
}
