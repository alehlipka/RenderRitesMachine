# Примеры использования RenderRites Machine

Этот документ содержит расширенные примеры использования различных возможностей движка.

## Содержание

1. [Базовые примеры](#базовые-примеры)
2. [Полный пример создания игры](#полный-пример-создания-игры)
3. [ECS примеры](#ecs-примеры)
4. [Работа с ресурсами](#работа-с-ресурсами)
5. [Работа с камерой](#работа-с-камерой)
6. [Работа с аудио](#работа-с-аудио)
7. [Создание UI](#создание-ui)
8. [Обработка ввода](#обработка-ввода)
9. [Оптимизация](#оптимизация)
10. [Дополнительные примеры](#дополнительные-примеры)

## Базовые примеры

### Минимальный пример приложения

```csharp
using RenderRitesMachine;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace MinimalExample;

public class MinimalScene : Scene
{
    public MinimalScene(string name, IAssetsService assetsService, ITimeService timeService,
        IRenderService renderService, IGuiService guiService, IAudioService audioService,
        ISceneManager sceneManager, ILogger logger)
        : base(name, assetsService, timeService, renderService, guiService, audioService, sceneManager, logger) { }
    
    protected override void OnLoad()
    {
        // Минимальная настройка
        Camera.Position = new Vector3(0, 0, 5);
    }
}

class Program
{
    static void Main()
    {
        RenderRites.Machine.Scenes
            .AddScene<MinimalScene>("main");
        
        RenderRites.Machine.RunWindow("Minimal Example", VSyncMode.Adaptive);
    }
}
```

## Полный пример создания игры

```csharp
using RenderRitesMachine;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Systems;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace MyGame;

// 1. Создаем главную сцену игры
public class GameScene : Scene
{
    public GameScene(string name, IAssetsService assetsService, ITimeService timeService, 
                     IRenderService renderService, IGuiService guiService, 
                     IAudioService audioService, ISceneManager sceneManager, ILogger logger)
        : base(name, assetsService, timeService, renderService, guiService, audioService, sceneManager, logger) { }
    
    protected override void OnLoad()
    {
        // Загрузка всех ресурсов
        LoadAssets();
        
        // Создание игровых объектов
        CreatePlayer();
        CreateEnemies();
        CreateEnvironment();
        
        // Настройка камеры
        SetupCamera();
        
        // Добавление систем
        SetupSystems();
    }
    
    private void LoadAssets()
    {
        // Шейдеры
        Assets.AddShader("cel", Path.Combine("Assets", "Shaders", "CelShading"));
        Assets.AddShader("outline", Path.Combine("Assets", "Shaders", "Outline"));
        
        // Модели
        Assets.AddMeshFromFile("player", Path.Combine("Assets", "Objects", "player.obj"));
        Assets.AddMeshFromFile("enemy", Path.Combine("Assets", "Objects", "enemy.obj"));
        Assets.AddSphere("bullet", 0.1f, 10, 10);
        
        // Текстуры
        Assets.AddTexture("playerTex", TextureType.ColorMap, 
            Path.Combine("Assets", "Textures", "player.jpg"));
        Assets.AddTexture("enemyTex", TextureType.ColorMap, 
            Path.Combine("Assets", "Textures", "enemy.jpg"));
        
        // Bounding boxes для коллизий
        Assets.AddBoundingBox("player");
        Assets.AddBoundingBox("enemy");
    }
    
    private void CreatePlayer()
    {
        int player = World.NewEntity();
        var transforms = World.GetPool<Transform>();
        ref Transform transform = ref transforms.Add(player);
        transform.Position = new Vector3(0, 0, 0);
        transform.Scale = Vector3.One;
        
        var meshes = World.GetPool<Mesh>();
        ref Mesh mesh = ref meshes.Add(player);
        mesh.Name = "player";
        mesh.ShaderName = "cel";
        
        var textures = World.GetPool<ColorTexture>();
        ref ColorTexture texture = ref textures.Add(player);
        texture.Name = "playerTex";
    }
    
    private void CreateEnemies()
    {
        var transforms = World.GetPool<Transform>();
        var meshes = World.GetPool<Mesh>();
        var textures = World.GetPool<ColorTexture>();
        
        // Создаем 10 врагов в случайных позициях
        Random random = new();
        for (int i = 0; i < 10; i++)
        {
            int enemy = World.NewEntity();
            ref Transform transform = ref transforms.Add(enemy);
            transform.Position = new Vector3(
                random.NextSingle() * 20 - 10,
                0,
                random.NextSingle() * 20 - 10
            );
            
            ref Mesh mesh = ref meshes.Add(enemy);
            mesh.Name = "enemy";
            mesh.ShaderName = "cel";
            
            ref ColorTexture texture = ref textures.Add(enemy);
            texture.Name = "enemyTex";
        }
    }
    
    private void CreateEnvironment()
    {
        // Создание окружения (например, платформы)
        // ...
    }
    
    private void SetupCamera()
    {
        Camera.Position = new Vector3(0, 5, 10);
        Camera.Pitch = -20.0f; // Смотрим немного вниз
        Camera.Yaw = 0.0f;
        Camera.Fov = 60.0f;
        Camera.Speed = 10.0f;
    }
    
    private void SetupSystems()
    {
        // Системы обновления (выполняются каждый кадр)
        UpdateSystems.Add(new InputUpdateSystem());
        UpdateSystems.Add(new PlayerMovementSystem());
        UpdateSystems.Add(new EnemyAISystem());
        UpdateSystems.Add(new CollisionSystem());
        
        // Системы рендеринга
        ResizeSystems.Add(new MainResizeSystem());
        RenderSystems.Add(new MainRenderSystem());
        RenderSystems.Add(new OutlineRenderSystem());
        RenderSystems.Add(new GuiRenderSystem());
    }
}

// 2. Точка входа приложения
internal static class Program
{
    private static void Main()
    {
        // Регистрация сцен
        RenderRites.Machine.Scenes
            .AddScene<GameScene>("game")
            .AddScene<MenuScene>("menu");
        
        // Запуск игры
        RenderRites.Machine.RunWindow(
            "My Awesome Game", 
            VSyncMode.Adaptive, 
            samples: 4
        );
    }
}
```

## ECS примеры

### Создание простого объекта

```csharp
protected override void OnLoad()
{
    // Создание сущности
    int entity = World.NewEntity();
    
    // Добавление компонентов
    var transforms = World.GetPool<Transform>();
    ref Transform transform = ref transforms.Add(entity);
    transform.Position = new Vector3(0, 0, 0);
    transform.Scale = Vector3.One;
    
    var meshes = World.GetPool<Mesh>();
    ref Mesh mesh = ref meshes.Add(entity);
    mesh.Name = "sphere";
    mesh.ShaderName = "cel";
    
    var textures = World.GetPool<ColorTexture>();
    ref ColorTexture texture = ref textures.Add(entity);
    texture.Name = "debug";
}
```

### Создание множества объектов

```csharp
protected override void OnLoad()
{
    Assets.AddSphere("cube", 1.0f, 10, 10);
    Assets.AddTexture("tex", TextureType.ColorMap, "path/to/texture.jpg");
    
    var transforms = World.GetPool<Transform>();
    var meshes = World.GetPool<Mesh>();
    var textures = World.GetPool<ColorTexture>();
    
    // Создаем сетку объектов
    for (int x = 0; x < 10; x++)
    {
        for (int z = 0; z < 10; z++)
        {
            int entity = World.NewEntity();
            
            ref Transform transform = ref transforms.Add(entity);
            transform.Position = new Vector3(x * 2, 0, z * 2);
            
            ref Mesh mesh = ref meshes.Add(entity);
            mesh.Name = "cube";
            
            ref ColorTexture texture = ref textures.Add(entity);
            texture.Name = "tex";
        }
    }
}
```

### Система физики (простая)

```csharp
public struct Velocity
{
    public Vector3 Value;
}

public class PhysicsSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        var transforms = world.GetPool<Transform>();
        var velocities = world.GetPool<Velocity>();
        
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Velocity>()
            .End();
        
        float deltaTime = shared.Time.UpdateDeltaTime;
        Vector3 gravity = new Vector3(0, -9.81f, 0);
        
        foreach (int entity in filter)
        {
            ref Transform transform = ref transforms.Get(entity);
            ref Velocity velocity = ref velocities.Get(entity);
            
            // Применяем гравитацию
            velocity.Value += gravity * deltaTime;
            
            // Обновляем позицию
            transform.Position += velocity.Value * deltaTime;
            
            // Простая проверка земли
            if (transform.Position.Y < 0)
            {
                transform.Position = new Vector3(transform.Position.X, 0, transform.Position.Z);
                velocity.Value = new Vector3(velocity.Value.X, 0, velocity.Value.Z);
            }
        }
    }
}
```

### Создание системы переключения сцен

```csharp
public class MenuScene : Scene
{
    protected override void OnLoad()
    {
        // ... настройка меню ...
        
        UpdateSystems.Add(new MenuInputSystem());
    }
}

// Система обработки ввода в меню
public class MenuInputSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        var window = shared.Window;
        
        if (window != null && window.KeyboardState.IsKeyPressed(Keys.Enter))
        {
            // Переключение на игровую сцену
            shared.SceneManager.SwitchTo("game");
        }
    }
}
```

### Создание системы коллизий

```csharp
public class CollisionSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        var transforms = world.GetPool<Transform>();
        var players = world.GetPool<PlayerTag>();
        var enemies = world.GetPool<EnemyTag>();
        
        // Получаем позиции всех игроков и врагов
        var playerPositions = GetEntityPositions(world, players);
        var enemyPositions = GetEntityPositions(world, enemies);
        
        // Проверяем коллизии
        foreach (var playerPos in playerPositions)
        {
            foreach (var enemyPos in enemyPositions)
            {
                float distance = (playerPos - enemyPos).Length;
                if (distance < 1.0f) // Радиус коллизии
                {
                    // Обработка коллизии
                    HandleCollision(playerPos, enemyPos);
                }
            }
        }
    }
}
```

## Работа с ресурсами

### Загрузка различных типов ресурсов

```csharp
protected override void OnLoad()
{
    // Шейдеры (требуют папку с vertex.glsl и fragment.glsl)
    Assets.AddShader("basic", Path.Combine("Assets", "Shaders", "Basic"));
    Assets.AddShader("cel", Path.Combine("Assets", "Shaders", "CelShading"));
    
    // 3D модели (поддерживаются форматы Assimp: .obj, .fbx, .dae и др.)
    Assets.AddMeshFromFile("character", Path.Combine("Assets", "Models", "character.obj"));
    Assets.AddMeshFromFile("weapon", Path.Combine("Assets", "Models", "weapon.fbx"));
    
    // Текстуры (поддерживаются форматы StbImage: .jpg, .png, .bmp и др.)
    Assets.AddTexture("diffuse", TextureType.ColorMap, 
        Path.Combine("Assets", "Textures", "diffuse.jpg"));
    Assets.AddTexture("normal", TextureType.NormalMap, 
        Path.Combine("Assets", "Textures", "normal.png"));
    
    // Программное создание мешей
    Assets.AddSphere("ball", 1.0f, 20, 20);
    
    // Bounding boxes для коллизий
    Assets.AddBoundingBox("character");
    Assets.AddBoundingBox("weapon");
}
```

### Обработка ошибок при загрузке

```csharp
protected override void OnLoad()
{
    // Безопасная загрузка с fallback
    if (!TryLoadShader("custom", "path/to/custom"))
    {
        Logger.LogWarning("Custom shader not found, using default");
        Assets.AddShader("cel", Path.Combine("Assets", "Shaders", "CelShading"));
    }
}

private bool TryLoadShader(string name, string path)
{
    try
    {
        Assets.AddShader(name, path);
        return true;
    }
    catch (FileNotFoundException)
    {
        return false;
    }
    catch (ShaderCompilationException ex)
    {
        Logger.LogException(LogLevel.Error, ex, $"Shader compilation failed: {name}");
        return false;
    }
}
```

## Работа с камерой

### Камера от первого лица

```csharp
public class FirstPersonCameraSystem : IEcsRunSystem
{
    private float _mouseSensitivity = 0.1f;
    
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        var camera = shared.Camera;
        var window = shared.Window;
        
        if (window == null) return;
        
        // Обработка мыши для поворота камеры
        var mouseState = window.MouseState;
        if (mouseState.IsButtonDown(MouseButton.Left))
        {
            float deltaX = mouseState.Delta.X * _mouseSensitivity;
            float deltaY = mouseState.Delta.Y * _mouseSensitivity;
            
            camera.Yaw += deltaX;
            camera.Pitch -= deltaY; // Инвертируем для естественного управления
        }
        
        // Обработка клавиатуры для движения
        var keyboard = window.KeyboardState;
        Vector3 moveDirection = Vector3.Zero;
        
        if (keyboard.IsKeyDown(Keys.W))
            moveDirection += camera.Front;
        if (keyboard.IsKeyDown(Keys.S))
            moveDirection -= camera.Front;
        if (keyboard.IsKeyDown(Keys.A))
            moveDirection -= camera.Right;
        if (keyboard.IsKeyDown(Keys.D))
            moveDirection += camera.Right;
        
        if (moveDirection.LengthSquared > 0)
        {
            moveDirection.Normalize();
            camera.Position += moveDirection * camera.Speed * shared.Time.UpdateDeltaTime;
        }
    }
}
```

### Камера следования за объектом

```csharp
public class FollowCameraSystem : IEcsRunSystem
{
    private Vector3 _offset = new Vector3(0, 5, 10);
    private float _smoothness = 5.0f;
    
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        var camera = shared.Camera;
        
        // Находим объект для следования (например, игрок)
        var transforms = world.GetPool<Transform>();
        var players = world.GetPool<PlayerTag>();
        
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<PlayerTag>()
            .End();
        
        if (filter.GetEntitiesCount() > 0)
        {
            int playerEntity = filter.GetRawEntities()[0];
            Transform playerTransform = transforms.Get(playerEntity);
            
            // Целевая позиция камеры
            Vector3 targetPosition = playerTransform.Position + _offset;
            
            // Плавное перемещение
            float deltaTime = shared.Time.UpdateDeltaTime;
            float lerpFactor = _smoothness * deltaTime;
            camera.Position += (targetPosition - camera.Position) * lerpFactor;
            
            // Камера смотрит на игрока
            Vector3 direction = (playerTransform.Position - camera.Position).Normalized();
            // Можно вычислить Pitch и Yaw из direction
        }
    }
}
```

### Орбитальная камера

```csharp
public class OrbitCameraSystem : IEcsRunSystem
{
    private float _distance = 10.0f;
    private float _angleX = 0.0f;
    private float _angleY = 45.0f;
    private Vector3 _target = Vector3.Zero;
    
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        var camera = shared.Camera;
        var window = shared.Window;
        
        if (window == null) return;
        
        // Управление углами через мышь
        var mouseState = window.MouseState;
        if (mouseState.IsButtonDown(MouseButton.Right))
        {
            _angleY += mouseState.Delta.X * 0.1f;
            _angleX -= mouseState.Delta.Y * 0.1f;
            _angleX = MathHelper.Clamp(_angleX, -89.0f, 89.0f);
        }
        
        // Управление расстоянием через колесо мыши
        _distance -= mouseState.ScrollDelta.Y * 0.5f;
        _distance = MathHelper.Clamp(_distance, 2.0f, 50.0f);
        
        // Вычисление позиции камеры
        float radX = MathHelper.DegreesToRadians(_angleX);
        float radY = MathHelper.DegreesToRadians(_angleY);
        
        float x = _distance * MathF.Cos(radX) * MathF.Sin(radY);
        float y = _distance * MathF.Sin(radX);
        float z = _distance * MathF.Cos(radX) * MathF.Cos(radY);
        
        camera.Position = _target + new Vector3(x, y, z);
        
        // Камера смотрит на цель
        Vector3 direction = (_target - camera.Position).Normalized();
        // Устанавливаем Pitch и Yaw из direction
    }
}
```

## Работа с аудио

### Фоновая музыка

```csharp
protected override void OnLoad()
{
    // Загрузка фоновой музыки
    int musicBuffer = Audio.LoadAudio("bgm", 
        Path.Combine("Assets", "Sounds", "background.mp3"));
    
    // Создание источника с зацикливанием
    int musicSource = Audio.CreateSource("bgm", loop: true, volume: 0.3f);
    
    // Воспроизведение
    Audio.Play(musicSource);
}

// Управление громкостью
public class AudioControlSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        var audio = shared.Audio;
        var window = shared.Window;
        
        if (window?.KeyboardState.IsKeyPressed(Keys.M) == true)
        {
            // Переключение музыки
            float currentVolume = GetMasterVolume();
            audio.SetMasterVolume(currentVolume > 0 ? 0.0f : 0.5f);
        }
    }
}
```

### 3D позиционный звук

```csharp
public class SoundEffectSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        var audio = shared.Audio;
        
        // Обновление позиции слушателя (камеры)
        var camera = shared.Camera;
        audio.SetListenerPosition(camera.Position);
        
        // Обновление позиций источников звука
        EcsWorld world = systems.GetWorld();
        var transforms = world.GetPool<Transform>();
        var soundSources = world.GetPool<SoundSourceComponent>();
        
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<SoundSourceComponent>()
            .End();
        
        foreach (int entity in filter)
        {
            Transform transform = transforms.Get(entity);
            SoundSourceComponent sound = soundSources.Get(entity);
            
            audio.SetSourcePosition(sound.SourceId, transform.Position);
        }
    }
}

// Компонент для источников звука
public struct SoundSourceComponent
{
    public int SourceId;
    public string SoundName;
}
```

## Создание UI

### HUD игрока

```csharp
public class PlayerHUDSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        // Получаем данные игрока
        float health = GetPlayerHealth();
        float maxHealth = GetPlayerMaxHealth();
        int score = GetScore();
        
        // Создаем HUD
        UI.Window("Player HUD").With(() =>
        {
            // Полоса здоровья
            float healthPercent = health / maxHealth;
            UI.ProgressBar(healthPercent, new Vector2(200, 20), $"Health: {health:F0}/{maxHealth:F0}");
            
            UI.Spacing();
            
            // Счет
            UI.Text($"Score: {score}");
            
            UI.Spacing();
            
            // Мини-карта или другая информация
            if (UI.BeginChild("Minimap", new Vector2(200, 200)))
            {
                DrawMinimap();
                UI.EndChild();
            }
        });
    }
}
```

### Меню настроек

```csharp
public class SettingsMenuSystem : IEcsRunSystem
{
    private bool _showSettings = false;
    private float _masterVolume = 0.5f;
    private float _sfxVolume = 1.0f;
    private bool _fullscreen = false;
    
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        var window = shared.Window;
        
        // Открытие меню настроек
        if (window?.KeyboardState.IsKeyPressed(Keys.Escape) == true)
        {
            _showSettings = !_showSettings;
        }
        
        if (_showSettings)
        {
            UI.Window("Settings").With(() =>
            {
                // Громкость
                UI.Text("Audio Settings");
                UI.SliderFloat("Master Volume", ref _masterVolume, 0.0f, 1.0f);
                UI.SliderFloat("SFX Volume", ref _sfxVolume, 0.0f, 1.0f);
                
                shared.Audio.SetMasterVolume(_masterVolume);
                
                UI.Spacing();
                
                // Графика
                UI.Text("Graphics Settings");
                UI.Checkbox("Fullscreen", ref _fullscreen);
                
                if (_fullscreen != window.WindowState == WindowState.Fullscreen)
                {
                    window.WindowState = _fullscreen ? WindowState.Fullscreen : WindowState.Normal;
                }
                
                UI.Spacing();
                
                if (UI.Button("Apply"))
                {
                    ApplySettings();
                }
                
                if (UI.Button("Cancel"))
                {
                    _showSettings = false;
                }
            });
        }
    }
}
```

## Обработка ввода

### Система управления игроком

```csharp
public class PlayerControlSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        var window = shared.Window;
        
        if (window == null) return;
        
        var transforms = world.GetPool<Transform>();
        var players = world.GetPool<PlayerTag>();
        var velocities = world.GetPool<Velocity>();
        
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<PlayerTag>()
            .Inc<Velocity>()
            .End();
        
        var keyboard = window.KeyboardState;
        float moveSpeed = 5.0f;
        
        foreach (int entity in filter)
        {
            ref Transform transform = ref transforms.Get(entity);
            ref Velocity velocity = ref velocities.Get(entity);
            
            Vector3 moveDirection = Vector3.Zero;
            
            // WASD управление
            if (keyboard.IsKeyDown(Keys.W))
                moveDirection += Vector3.UnitZ;
            if (keyboard.IsKeyDown(Keys.S))
                moveDirection -= Vector3.UnitZ;
            if (keyboard.IsKeyDown(Keys.A))
                moveDirection -= Vector3.UnitX;
            if (keyboard.IsKeyDown(Keys.D))
                moveDirection += Vector3.UnitX;
            
            // Прыжок
            if (keyboard.IsKeyPressed(Keys.Space))
            {
                velocity.Value += Vector3.UnitY * 5.0f;
            }
            
            if (moveDirection.LengthSquared > 0)
            {
                moveDirection.Normalize();
                velocity.Value = moveDirection * moveSpeed;
            }
            else
            {
                velocity.Value = Vector3.Zero;
            }
        }
    }
}
```

### Обработка мыши

```csharp
public class MouseInteractionSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        var window = shared.Window;
        
        if (window == null) return;
        
        var mouseState = window.MouseState;
        var camera = shared.Camera;
        
        // Ray casting для выбора объектов
        if (mouseState.IsButtonPressed(MouseButton.Left))
        {
            Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
            Vector2i windowSize = window.ClientSize;
            
            Ray ray = Ray.GetFromScreen(
                mousePos.X, 
                mousePos.Y, 
                windowSize,
                camera.Position,
                camera.ProjectionMatrix,
                camera.ViewMatrix
            );
            
            // Проверяем пересечения с объектами
            CheckRayIntersections(ray, systems);
        }
        
        // Перетаскивание объектов
        if (mouseState.IsButtonDown(MouseButton.Left))
        {
            HandleDrag(mouseState, systems);
        }
    }
}
```

## Оптимизация

### Использование видимости объектов

```csharp
public class VisibilitySystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        var meshes = world.GetPool<Mesh>();
        var transforms = world.GetPool<Transform>();
        
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .End();
        
        // Отключаем рендеринг объектов далеко от камеры
        float maxDistance = 100.0f;
        Vector3 cameraPos = shared.Camera.Position;
        
        foreach (int entity in filter)
        {
            Transform transform = transforms.Get(entity);
            float distance = (transform.Position - cameraPos).Length;
            
            ref Mesh mesh = ref meshes.Get(entity);
            mesh.IsVisible = distance < maxDistance;
        }
    }
}
```

### Оптимизация загрузки ресурсов

```csharp
protected override void OnLoad()
{
    // Загружаем только необходимые ресурсы
    LoadEssentialAssets();
    
    // Остальные ресурсы загружаем асинхронно или по требованию
    Task.Run(() => LoadOptionalAssets());
}

private void LoadEssentialAssets()
{
    // Критически важные ресурсы
    Assets.AddShader("cel", Path.Combine("Assets", "Shaders", "CelShading"));
    Assets.AddTexture("default", TextureType.ColorMap, 
        Path.Combine("Assets", "Textures", "default.jpg"));
}

private void LoadOptionalAssets()
{
    // Ресурсы, которые можно загрузить позже
    try
    {
        Assets.AddMeshFromFile("optionalModel", "path/to/model.obj");
    }
    catch (Exception ex)
    {
        Logger.LogWarning($"Failed to load optional asset: {ex.Message}");
    }
}
```

### Мониторинг производительности

```csharp
public class PerformanceMonitorSystem : IEcsRunSystem
{
    private float _fpsUpdateInterval = 1.0f;
    private float _fpsTimer = 0.0f;
    private int _frameCount = 0;
    private float _currentFPS = 0.0f;
    
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        float deltaTime = shared.Time.UpdateDeltaTime;
        
        _frameCount++;
        _fpsTimer += deltaTime;
        
        if (_fpsTimer >= _fpsUpdateInterval)
        {
            _currentFPS = _frameCount / _fpsTimer;
            _frameCount = 0;
            _fpsTimer = 0.0f;
        }
        
        // Отображение FPS в UI
        UI.Window("Performance").With(() =>
        {
            UI.Text($"FPS: {_currentFPS:F1}");
            UI.Text($"Delta Time: {deltaTime * 1000:F2} ms");
            
            var stats = shared.RenderStats;
            UI.Text($"Objects: {stats.TotalObjects}");
            UI.Text($"Rendered: {stats.RenderedObjects}");
            UI.Text($"Culled: {stats.CulledObjects}");
        });
    }
}
```

## Дополнительные примеры

### Система частиц (базовая)

```csharp
public struct Particle
{
    public Vector3 Position;
    public Vector3 Velocity;
    public float Lifetime;
    public float MaxLifetime;
    public Vector4 Color;
}

public class ParticleSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        var particles = world.GetPool<Particle>();
        
        EcsFilter filter = world
            .Filter<Particle>()
            .End();
        
        float deltaTime = shared.Time.UpdateDeltaTime;
        
        foreach (int entity in filter)
        {
            ref Particle particle = ref particles.Get(entity);
            
            // Обновление позиции
            particle.Position += particle.Velocity * deltaTime;
            
            // Обновление времени жизни
            particle.Lifetime -= deltaTime;
            
            // Удаление мертвых частиц
            if (particle.Lifetime <= 0)
            {
                world.DelEntity(entity);
            }
            else
            {
                // Обновление цвета на основе времени жизни
                float alpha = particle.Lifetime / particle.MaxLifetime;
                particle.Color = new Vector4(particle.Color.Xyz, alpha);
            }
        }
    }
}
```

### Система анимации (простая)

```csharp
public struct Animation
{
    public float CurrentTime;
    public float Duration;
    public Vector3 StartPosition;
    public Vector3 EndPosition;
}

public class AnimationSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        var transforms = world.GetPool<Transform>();
        var animations = world.GetPool<Animation>();
        
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Animation>()
            .End();
        
        float deltaTime = shared.Time.UpdateDeltaTime;
        
        foreach (int entity in filter)
        {
            ref Transform transform = ref transforms.Get(entity);
            ref Animation animation = ref animations.Get(entity);
            
            animation.CurrentTime += deltaTime;
            
            if (animation.CurrentTime >= animation.Duration)
            {
                // Анимация завершена
                transform.Position = animation.EndPosition;
                animations.Del(entity);
            }
            else
            {
                // Интерполяция
                float t = animation.CurrentTime / animation.Duration;
                transform.Position = animation.StartPosition + 
                    (animation.EndPosition - animation.StartPosition) * t;
            }
        }
    }
}
```
