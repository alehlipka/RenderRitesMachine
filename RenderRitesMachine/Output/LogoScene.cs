using ImGuiNET;
using Leopotam.EcsLite;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Systems;
using RenderRitesMachine.Services;
using Vector2 = System.Numerics.Vector2;

namespace RenderRitesMachine.Output;

/// <summary>
/// Сцена с логотипом движка, которая отображается при запуске приложения.
/// Внутренняя сцена, автоматически добавляемая движком. Пользователь не должен создавать её вручную.
/// </summary>
internal class LogoScene : Scene
{
    private const float DisplayDuration = 3.0f;
    private const string LogoSceneName = "logo";
    private float _elapsedTime;
    private string? _nextSceneName;
    private int _logoTextureId;
    private Vector2 _logoSize = new(400, 400);
    private bool _logoLoaded;
    private readonly ISceneManager _sceneManager;
    private readonly IGuiService _guiService;
    private readonly ILogger _logger;
    private int? _audioSourceId;

    /// <summary>
    /// Создает новую сцену логотипа. Используется только внутри движка.
    /// </summary>
    internal LogoScene(string name, IAssetsService assetsService, ITimeService timeService, IRenderService renderService, IGuiService guiService, IAudioService audioService, ISceneManager sceneManager, ILogger logger)
        : base(name, assetsService, timeService, renderService, guiService, audioService, sceneManager, logger)
    {
        _sceneManager = sceneManager;
        _guiService = guiService;
        _logger = logger;
    }

    protected override void OnLoad()
    {
        LoadAndPlayAudio();

        LoadLogo();

        FindNextScene();

        ResizeSystems.Add(new MainResizeSystem());
        UpdateSystems.Add(new LogoUpdateSystem(this));
        RenderSystems.Add(new MainRenderSystem());
        RenderSystems.Add(new LogoRenderSystem(this));
    }

    private void LoadLogo()
    {
        try
        {
            string[] logoPaths =
            {
                "render-rites-logo.png",
                Path.Combine(AppContext.BaseDirectory, "render-rites-logo.png"),
                Path.Combine(Directory.GetCurrentDirectory(), "render-rites-logo.png")
            };

            string? logoPath = null;
            foreach (string path in logoPaths)
            {
                if (File.Exists(path))
                {
                    logoPath = path;
                    break;
                }
            }

            if (logoPath != null)
            {
                Assets.AddTexture("logo", TextureType.ColorMap, logoPath);
                TextureAsset logoTexture = Assets.GetTexture("logo");
                _logoTextureId = logoTexture.Id;
                _logoLoaded = true;
            }
        }
        catch
        {
            _logoLoaded = false;
        }
    }

    private void LoadAndPlayAudio()
    {
        try
        {
            string[] audioPaths =
            {
                "logo.mp3",
                Path.Combine(AppContext.BaseDirectory, "logo.mp3"),
                Path.Combine(Directory.GetCurrentDirectory(), "logo.mp3")
            };

            string? audioPath = null;
            foreach (string path in audioPaths)
            {
                if (File.Exists(path))
                {
                    audioPath = path;
                    break;
                }
            }

            if (audioPath != null)
            {
                Audio.LoadAudio("logo", audioPath);

                _audioSourceId = Audio.CreateSource("logo", position: null, volume: 1.0f, loop: false);

                Audio.Play(_audioSourceId.Value);
            }
            else
            {
                _logger?.LogDebug("Logo audio file (logo.mp3) not found, skipping audio playback");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning($"Failed to load or play logo audio: {ex.Message}");
        }
    }

    private void FindNextScene()
    {
        var allScenes = _sceneManager.Select(s => s.Name).ToList();
        _nextSceneName = allScenes.FirstOrDefault(s => s != LogoSceneName && s != Name);

        if (string.IsNullOrEmpty(_nextSceneName))
        {
            _nextSceneName = allScenes.FirstOrDefault(s => s != LogoSceneName);
        }
    }

    /// <summary>
    /// Обновляет следующую сцену (вызывается при необходимости).
    /// </summary>
    private void UpdateNextScene()
    {
        FindNextScene();
    }

    /// <summary>
    /// Обновляет время отображения и переключает сцену при необходимости.
    /// </summary>
    public void UpdateLogo(float deltaTime)
    {
        _elapsedTime += deltaTime;

        if (_elapsedTime >= DisplayDuration)
        {
            SwitchToNextScene();
        }
    }

    /// <summary>
    /// Переключается на следующую сцену.
    /// </summary>
    public void SwitchToNextScene()
    {
        UpdateNextScene();

        if (!string.IsNullOrEmpty(_nextSceneName))
        {
            try
            {
                _sceneManager.SwitchTo(_nextSceneName);
            }
            catch (ArgumentException)
            {
                var allScenes = _sceneManager.Select(s => s.Name).ToList();
                string? fallbackScene = allScenes.FirstOrDefault(s => s != LogoSceneName && s != Name);
                if (!string.IsNullOrEmpty(fallbackScene))
                {
                    try
                    {
                        _sceneManager.SwitchTo(fallbackScene);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

    public bool IsLogoLoaded => _logoLoaded;
    public int LogoTextureId => _logoTextureId;
    public Vector2 LogoSize => _logoSize;

    /// <summary>
    /// Освобождает ресурсы сцены, включая аудио источник.
    /// </summary>
    public new void Dispose()
    {
        if (_audioSourceId.HasValue)
        {
            Audio.DeleteSource(_audioSourceId.Value);
            _audioSourceId = null;
        }
        base.Dispose();
    }
}

/// <summary>
/// Система обновления логотипа.
/// </summary>
internal class LogoUpdateSystem : IEcsRunSystem
{
    private readonly LogoScene _logoScene;

    public LogoUpdateSystem(LogoScene logoScene)
    {
        _logoScene = logoScene;
    }

    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();

        _logoScene.UpdateLogo(shared.Time.UpdateDeltaTime);

        if (shared.Window != null)
        {
            KeyboardState? keyboard = shared.Window.KeyboardState;
            if (keyboard.IsKeyPressed(Keys.Space) ||
                keyboard.IsKeyPressed(Keys.Enter))
            {
                _logoScene.SwitchToNextScene();
            }
        }
    }
}

/// <summary>
/// Система рендеринга логотипа.
/// </summary>
internal class LogoRenderSystem : IEcsRunSystem
{
    private readonly LogoScene _logoScene;

    public LogoRenderSystem(LogoScene logoScene)
    {
        _logoScene = logoScene;
    }

    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();

        IntPtr context = shared.Gui.GetContext();
        if (context != IntPtr.Zero)
        {
            ImGui.SetCurrentContext(context);
        }

        ImGuiWindowFlags flags = ImGuiWindowFlags.NoTitleBar
                                 | ImGuiWindowFlags.NoResize
                                 | ImGuiWindowFlags.NoMove
                                 | ImGuiWindowFlags.NoScrollbar
                                 | ImGuiWindowFlags.NoCollapse
                                 | ImGuiWindowFlags.NoBackground
                                 | ImGuiWindowFlags.NoBringToFrontOnFocus;

        if (shared.Window != null)
        {
            Vector2i windowSize = shared.Window.ClientSize;
            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.SetNextWindowSize(new Vector2(windowSize.X, windowSize.Y));
        }

        if (ImGui.Begin("LogoWindow", flags))
        {
            if (shared.Window != null && _logoScene.IsLogoLoaded)
            {
                Vector2i windowSize = shared.Window.ClientSize;
                Vector2 logoSize = _logoScene.LogoSize;

                float maxSize = Math.Min(windowSize.X, windowSize.Y) * 0.8f;
                if (logoSize.X > maxSize || logoSize.Y > maxSize)
                {
                    float scale = maxSize / Math.Max(logoSize.X, logoSize.Y);
                    logoSize = new Vector2(logoSize.X * scale, logoSize.Y * scale);
                }

                float x = (windowSize.X - logoSize.X) * 0.5f;
                float y = (windowSize.Y - logoSize.Y) * 0.5f;

                ImGui.SetCursorPos(new Vector2(x, y));

                Vector2 uv0 = new Vector2(0, 1);
                Vector2 uv1 = new Vector2(1, 0);
                UI.UI.Image(_logoScene.LogoTextureId, logoSize, uv0, uv1);
            }
        }
        ImGui.End();
    }
}

