using OpenTK.Mathematics;
using RenderRitesDemo.Scenes.Demo.Components;
using RenderRitesDemo.Scenes.Demo.Systems;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Systems;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;
using RenderRitesMachine.Services.Gui;

namespace RenderRitesDemo.Scenes.Demo;

internal sealed class DemoScene(string name, IAssetsService assetsService, ITimeService timeService, IRenderService renderService, IAudioService audioService, IGuiService guiService, ISceneManager sceneManager, ILogger logger)
    : Scene(name, assetsService, timeService, renderService, audioService, guiService, sceneManager, logger)
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly string _assetsRoot = Path.Combine(AppContext.BaseDirectory, "Assets");

    private const string DemoShaderName = "demo/demo-shader";
    private const string TestMeshName = "demo/test";
    private const string TestTextureName = "demo/test-texture";
    private const string AmbientAudioName = "demo/ambient-loop";

    private int? _ambientSourceId;

    protected override void OnLoad()
    {
        ConfigureCamera();
        RegisterSystems();
        LoadDemoAssets();
        SpawnDemoEntities();
        InitializeAudio();
    }

    private void ConfigureCamera()
    {
        Camera.Position = new Vector3(0f, 2.5f, 6f);
        Camera.Yaw = -90f;
        Camera.Pitch = -12f;
        Camera.AspectRatio = 800f / 600f;
    }

    private void RegisterSystems()
    {
        _ = UpdateSystems
            .Add(new CameraOrbitSystem(Vector3.Zero, radius: 6.0f, height: 1.8f, speed: 0.35f))
            .Add(new RotationAnimationSystem())
            .Add(new FloatingAnimationSystem());

        _ = RenderSystems
            .Add(new MainRenderSystem());

        _ = ResizeSystems
            .Add(new MainResizeSystem());
    }

    private void LoadDemoAssets()
    {
        string shadersRoot = Path.Combine(_assetsRoot, "Shaders");
        Assets.AddShader(DemoShaderName, Path.Combine(shadersRoot, "DemoShading"));

        string texturesRoot = Path.Combine(_assetsRoot, "Textures");
        Assets.AddTexture(TestTextureName, TextureType.ColorMap, Path.Combine(texturesRoot, "debug.jpg"));

        string objectsRoot = Path.Combine(_assetsRoot, "Objects");
        Assets.AddMeshFromFile(TestMeshName, Path.Combine(objectsRoot, "test.obj"));
    }

    private void SpawnDemoEntities()
    {
        CreateTestEntity(
            position: new Vector3(0f, -0.5f, 0f),
            scale: new Vector3(0.9f),
            rotationSpeed: 0.65f,
            floatAmplitude: 0.25f,
            floatFrequency: 0.4f,
            floatPhase: 0f
        );

        CreateTestEntity(
            position: new Vector3(2.0f, -0.4f, -1.8f),
            scale: new Vector3(0.75f),
            rotationSpeed: -0.45f,
            floatAmplitude: 0.15f,
            floatFrequency: 0.55f,
            floatPhase: MathF.PI / 2f
        );

        CreateTestEntity(
            position: new Vector3(-2.4f, -0.45f, 1.6f),
            scale: new Vector3(0.8f),
            rotationSpeed: 0.9f,
            floatAmplitude: 0.2f,
            floatFrequency: 0.7f,
            floatPhase: MathF.PI
        );
    }

    private void CreateTestEntity(
        Vector3 position,
        Vector3 scale,
        float rotationSpeed,
        float floatAmplitude,
        float floatFrequency,
        float floatPhase)
    {
        int entity = World.NewEntity();

        ref Transform transform = ref World.GetPool<Transform>().Add(entity);
        transform.Position = position;
        transform.Scale = scale;
        transform.RotationAxis = Vector3.UnitY;

        ref Mesh mesh = ref World.GetPool<Mesh>().Add(entity);
        mesh.Name = TestMeshName;
        mesh.ShaderName = DemoShaderName;

        ref ColorTexture texture = ref World.GetPool<ColorTexture>().Add(entity);
        texture.Name = TestTextureName;

        if (Math.Abs(rotationSpeed) > 0.0001f)
        {
            ref RotationAnimation rotation = ref World.GetPool<RotationAnimation>().Add(entity);
            rotation.Axis = Vector3.UnitY;
            rotation.Speed = rotationSpeed;
        }

        if (floatAmplitude > 0f)
        {
            ref FloatingAnimation floating = ref World.GetPool<FloatingAnimation>().Add(entity);
            floating.BasePosition = position;
            floating.Amplitude = floatAmplitude;
            floating.Frequency = MathF.Max(0.1f, floatFrequency);
            floating.Phase = floatPhase;
        }
    }

    private void InitializeAudio()
    {
        string audioPath = Path.Combine(_assetsRoot, "Sounds", "click.mp3");
        if (!File.Exists(audioPath))
        {
            return;
        }

        try
        {
            _ = Audio.LoadAudio(AmbientAudioName, audioPath);
            _ambientSourceId = Audio.CreateSource(AmbientAudioName, volume: 0.35f, loop: true);
            Audio.Play(_ambientSourceId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Failed to initialize demo audio: {ex.Message}");
        }
    }
}
