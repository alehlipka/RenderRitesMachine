using ImGuiNET;
using Leopotam.EcsLite;
using OpenTK.Mathematics;
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
    private const float DisplayDuration = 3.0f; // Длительность отображения в секундах
    private const string LogoSceneName = "logo"; // Имя сцены логотипа
    private float _elapsedTime;
    private string? _nextSceneName;
    private int _logoTextureId;
    private Vector2 _logoSize = new(400, 400); // Размер логотипа
    private bool _logoLoaded;
    private readonly ISceneManager _sceneManager;
    private readonly IGuiService _guiService;

    /// <summary>
    /// Создает новую сцену логотипа. Используется только внутри движка.
    /// </summary>
    internal LogoScene(string name, IAssetsService assetsService, ITimeService timeService, IRenderService renderService, IGuiService guiService, ISceneManager sceneManager) 
        : base(name, assetsService, timeService, renderService, guiService, sceneManager)
    {
        _sceneManager = sceneManager;
        _guiService = guiService;
    }

    protected override void OnLoad()
    {
        // Загружаем логотип
        LoadLogo();

        // Находим следующую сцену (первую после логотипа)
        FindNextScene();

        // Добавляем системы
        ResizeSystems.Add(new MainResizeSystem());
        UpdateSystems.Add(new LogoUpdateSystem(this));
        RenderSystems.Add(new MainRenderSystem());
        RenderSystems.Add(new LogoRenderSystem(this));
    }

    private void LoadLogo()
    {
        try
        {
            // Загружаем логотип из папки движка
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
            // Если не удалось загрузить логотип, просто не будем его отображать
            _logoLoaded = false;
        }
    }

    private void FindNextScene()
    {
        // Находим первую сцену, которая не является логотипом
        var allScenes = _sceneManager.Select(s => s.Name).ToList();
        _nextSceneName = allScenes.FirstOrDefault(s => s != LogoSceneName && s != Name);

        // Если не нашли, используем первую доступную сцену (кроме логотипа)
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

        // Переключаемся на следующую сцену после истечения времени
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
        // Обновляем следующую сцену на случай, если сцены были добавлены после загрузки
        UpdateNextScene();

        if (!string.IsNullOrEmpty(_nextSceneName))
        {
            try
            {
                // Пытаемся переключиться на следующую сцену
                _sceneManager.SwitchTo(_nextSceneName);
            }
            catch (ArgumentException)
            {
                // Если сцена не найдена, пробуем найти любую другую сцену (кроме логотипа)
                var allScenes = _sceneManager.Select(s => s.Name).ToList();
                var fallbackScene = allScenes.FirstOrDefault(s => s != LogoSceneName && s != Name);
                if (!string.IsNullOrEmpty(fallbackScene))
                {
                    try
                    {
                        _sceneManager.SwitchTo(fallbackScene);
                    }
                    catch
                    {
                        // Если не удалось переключиться, просто остаемся на логотипе
                    }
                }
            }
        }
    }

    public bool IsLogoLoaded => _logoLoaded;
    public int LogoTextureId => _logoTextureId;
    public Vector2 LogoSize => _logoSize;
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

        // Обновляем время отображения
        _logoScene.UpdateLogo(shared.Time.UpdateDeltaTime);

        // Проверяем нажатие клавиш для пропуска логотипа
        if (shared.Window != null)
        {
            var keyboard = shared.Window.KeyboardState;
            // Переключаемся при нажатии Space или Enter для пропуска логотипа
            if (keyboard.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space) ||
                keyboard.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Enter))
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

        // Устанавливаем контекст ImGui
        IntPtr context = shared.Gui.GetContext();
        if (context != IntPtr.Zero)
        {
            ImGui.SetCurrentContext(context);
        }

        // Создаем полноэкранное окно без рамки для отображения логотипа
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
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, 0));
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(windowSize.X, windowSize.Y));
        }

        if (ImGui.Begin("LogoWindow", flags))
        {
            // Центрируем логотип
            if (shared.Window != null && _logoScene.IsLogoLoaded)
            {
                Vector2i windowSize = shared.Window.ClientSize;
                Vector2 logoSize = _logoScene.LogoSize;

                // Адаптивный размер логотипа (максимум 80% от размера окна)
                float maxSize = Math.Min(windowSize.X, windowSize.Y) * 0.8f;
                if (logoSize.X > maxSize || logoSize.Y > maxSize)
                {
                    float scale = maxSize / Math.Max(logoSize.X, logoSize.Y);
                    logoSize = new Vector2(logoSize.X * scale, logoSize.Y * scale);
                }

                float x = (windowSize.X - logoSize.X) * 0.5f;
                float y = (windowSize.Y - logoSize.Y) * 0.5f;

                ImGui.SetCursorPos(new System.Numerics.Vector2(x, y));

                // Отображаем логотип с инвертированными UV координатами
                // чтобы компенсировать flip при загрузке текстуры для OpenGL
                Vector2 uv0 = new Vector2(0, 1); // Верхний левый угол (инвертирован по Y)
                Vector2 uv1 = new Vector2(1, 0); // Нижний правый угол (инвертирован по Y)
                RenderRitesMachine.UI.UI.Image(_logoScene.LogoTextureId, logoSize, uv0, uv1);
            }
        }
        ImGui.End();
    }
}

