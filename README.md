# RenderRites Machine

Современный игровой движок на C# с использованием OpenGL 4.6 через OpenTK и архитектурой Entity Component System (ECS).

## Особенности

- 🎮 **Архитектура ECS** - Использует Leopotam.EcsLite для эффективной организации игровой логики
- 🎨 **OpenGL 4.6** - Современный рендеринг с поддержкой шейдеров, текстур и мультисэмплинга
- 📦 **Управление ресурсами** - Централизованная система загрузки мешей, шейдеров и текстур
- 🎥 **Система сцен** - Гибкая система управления сценами с поддержкой переключения и автоматической сценой логотипа
- 📷 **Перспективная камера** - Полнофункциональная камера с кэшированием матриц
- 🖼️ **ImGui интеграция** - Встроенная поддержка ImGui для создания пользовательских интерфейсов
- 🎨 **UI обертка** - Удобная обертка над ImGui для упрощения создания окон и виджетов
- 🔧 **Оптимизации** - Кэширование шейдеров, оптимизированное обновление матриц камеры
- 📚 **Документация** - Полная XML-документация для всех публичных API
- ✅ **Валидация** - Проверка входных данных и обработка ошибок
- 🧪 **Тестирование** - Полное покрытие тестами ключевых компонентов (273 теста, включая edge cases)

## Требования

- .NET 9.0 или выше
- OpenGL 4.6 совместимая видеокарта
- Операционная система: Windows, Linux или macOS

## Структура проекта

```
RenderRites/
├── RenderRitesMachine/      # Ядро движка
│   ├── Assets/              # Типы ассетов (Mesh, Shader, Texture, BoundingBox)
│   ├── Configuration/       # Константы и конфигурация
│   ├── Debug/              # Инструменты отладки (FPS, OpenGL debug)
│   ├── ECS/                # ECS компоненты и системы
│   │   ├── Components/     # Базовые компоненты (Transform, Mesh, ColorTexture)
│   │   └── Systems/        # Базовые системы (MainRenderSystem, MainResizeSystem)
│   ├── Output/             # Окно, сцены, камера, менеджер сцен
│   ├── Services/           # Сервисы (Assets, Render, Time, Gui)
│   ├── UI/                 # Обертка над ImGui (UIWindow, UICombo, UIListBox, UIMenu)
│   └── Utilities/          # Утилиты (Ray, и т.д.)
├── RenderRitesDemo/        # Демо-приложение
│   ├── Assets/             # Ресурсы (шейдеры, модели, текстуры, шрифты)
│   │   ├── Shaders/        # Шейдеры (CelShading, Outline, Bounding)
│   │   ├── Objects/        # 3D модели (.obj)
│   │   └── Textures/       # Текстуры
│   ├── ECS/                # Демо-системы и компоненты
│   │   └── Features/       # Демо-фичи
│   │       ├── BoundingBox/  # Визуализация bounding box
│   │       ├── Input/        # Обработка ввода
│   │       ├── Outline/     # Рендеринг контуров
│   │       ├── Rotation/     # Вращение объектов
│   │       └── SceneSwitch/  # Переключение сцен
│   └── Scenes/             # Демо-сцены (DemoScene, GuiTestScene)
└── RenderRitesMachine.Tests/  # Проект тестов
    ├── PerspectiveCameraTests.cs
    ├── SceneManagerTests.cs
    ├── AssetsServiceTests.cs
    ├── RenderConstantsTests.cs
    └── SystemSharedObjectTests.cs
```

## Быстрый старт

### Создание сцены

```csharp
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Systems;
using OpenTK.Mathematics;

public class MyScene : Scene
{
    public MyScene(string name, IAssetsService assetsService, ITimeService timeService, 
                   IRenderService renderService, IGuiService guiService, ISceneManager sceneManager)
        : base(name, assetsService, timeService, renderService, guiService, sceneManager) { }
    
    protected override void OnLoad()
    {
        // Загрузка ресурсов
        Assets.AddShader("myShader", Path.Combine("Assets", "Shaders", "MyShader"));
        Assets.AddMeshFromFile("model", Path.Combine("Assets", "Objects", "model.obj"));
        Assets.AddTexture("texture", TextureType.ColorMap, Path.Combine("Assets", "Textures", "texture.jpg"));
        
        // Создание сферического меша программно
        Assets.AddSphere("sphere", 1.0f, 20, 20);
        
        // Создание сущностей
        int entity = World.NewEntity();
        var transforms = World.GetPool<Transform>();
        ref Transform transform = ref transforms.Add(entity);
        transform.Position = new Vector3(0, 0, 0);
        
        var meshes = World.GetPool<Mesh>();
        ref Mesh mesh = ref meshes.Add(entity);
        mesh.Name = "model";
        mesh.ShaderName = "myShader"; // По умолчанию используется "cel"
        
        var colorTextures = World.GetPool<ColorTexture>();
        ref ColorTexture texture = ref colorTextures.Add(entity);
        texture.Name = "texture";
        
        // Настройка камеры
        Camera.Position = new Vector3(0.0f, 0.0f, 10.0f);
        
        // Добавление систем
        ResizeSystems.Add(new MainResizeSystem());
        RenderSystems.Add(new MainRenderSystem());
        UpdateSystems.Add(new MyUpdateSystem());
    }
}
```

### Запуск приложения

```csharp
using RenderRitesMachine;
using OpenTK.Windowing.Common;

// Добавление сцен через фабрику (сцена логотипа добавляется автоматически)
RenderRites.Machine.Scenes
    .AddScene<MyScene>("main")
    .AddScene<AnotherScene>("another");

// Запуск окна (сцена логотипа запускается первой)
RenderRites.Machine.RunWindow("My Game", VSyncMode.Adaptive, 4);
```

**Примечание:** Сцена логотипа (`LogoScene`) автоматически добавляется при инициализации и запускается первой. Для переключения между сценами используйте `SceneSwitchSystem` или `SceneManager.SwitchTo()`.

### Полный пример создания игры

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

## Управление (в DemoScene)

- **Стрелки ↑↓** - Движение камеры вперед/назад
- **Стрелки ←→** - Движение камеры влево/вправо
- **A/D** - Движение камеры вверх/вниз
- **Q/E** - Поворот камеры влево/вправо (Yaw)
- **W/S** - Наклон камеры вверх/вниз (Pitch)
- **P** - Переключение режима отображения (wireframe/solid)
- **F** - Переключение полноэкранного режима
- **R** - Включение/выключение вращения объектов
- **ESC** - Выход из приложения

## Архитектура

### Entity Component System (ECS)

Проект использует архитектуру ECS для организации игровой логики:

- **Entities** - Идентификаторы сущностей (int)
- **Components** - Данные (Transform, Mesh, ColorTexture, и т.д.)
- **Systems** - Логика обработки (RenderSystem, UpdateSystem, и т.д.)

#### Создание кастомного компонента

```csharp
using Leopotam.EcsLite;
using OpenTK.Mathematics;

// Компонент для здоровья
public struct Health : IEcsAutoReset<Health>
{
    public float Current;
    public float Maximum;
    
    public void AutoReset(ref Health c)
    {
        c.Current = 100.0f;
        c.Maximum = 100.0f;
    }
}

// Компонент-тег для врагов
public struct EnemyTag { }

// Компонент для скорости движения
public struct Velocity
{
    public Vector3 Value;
}
```

#### Создание кастомной системы

```csharp
using Leopotam.EcsLite;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Components;
using OpenTK.Mathematics;

// Система движения объектов
public class MovementSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        var transforms = world.GetPool<Transform>();
        var velocities = world.GetPool<Velocity>();
        
        // Фильтр: все сущности с Transform и Velocity
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Velocity>()
            .End();
        
        float deltaTime = shared.Time.UpdateDeltaTime;
        
        foreach (int entity in filter)
        {
            ref Transform transform = ref transforms.Get(entity);
            Velocity velocity = velocities.Get(entity);
            
            // Обновляем позицию на основе скорости
            transform.Position += velocity.Value * deltaTime;
        }
    }
}

// Система обработки здоровья
public class HealthSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        var healths = world.GetPool<Health>();
        
        EcsFilter filter = world
            .Filter<Health>()
            .End();
        
        foreach (int entity in filter)
        {
            ref Health health = ref healths.Get(entity);
            
            // Если здоровье <= 0, удаляем сущность
            if (health.Current <= 0)
            {
                world.DelEntity(entity);
            }
        }
    }
}
```

#### Использование кастомных систем в сцене

```csharp
protected override void OnLoad()
{
    // ... загрузка ресурсов ...
    
    // Добавляем кастомные системы
    UpdateSystems.Add(new MovementSystem());
    UpdateSystems.Add(new HealthSystem());
    UpdateSystems.Add(new MainRenderSystem());
}
```

### Системы рендеринга

- `MainRenderSystem` - Основной рендеринг объектов
- `OutlineRenderSystem` - Рендеринг контуров объектов
- `BoundingBoxRenderSystem` - Визуализация bounding box

### Системы обновления

- `InputUpdateSystem` - Обработка ввода пользователя (клавиатура, мышь)
- `RotationUpdateSystem` - Вращение объектов с компонентом `RotationTag`
- `OutlineUpdateSystem` - Обновление видимости контуров объектов
- `SceneSwitchSystem` - Переключение между сценами во время выполнения

### Демо-сцены

- **LogoScene** - Автоматически добавляемая сцена с логотипом движка
- **DemoScene** - Демонстрация основных возможностей: рендеринг моделей, контуры, bounding box, вращение
- **GuiTestScene** - Демонстрация элементов интерфейса через ImGui

### Сервисы

- **IAssetsService** - Управление ресурсами (меши, шейдеры, текстуры, bounding boxes)
- **ITimeService** - Управление временем (delta time для update и render)
- **IRenderService** - Сервис рендеринга
- **IGuiService** - Доступ к контексту ImGui для создания интерфейсов
- **IAudioService** - Управление аудио (загрузка и воспроизведение звуков)
- **ILogger** - Система логирования

#### Примеры работы с сервисами

**Работа с камерой:**

```csharp
// В методе OnLoad() или в системе обновления
protected override void OnLoad()
{
    // Настройка камеры
    Camera.Position = new Vector3(0, 5, 10);
    Camera.Pitch = -20.0f; // Угол наклона в градусах (-89 до 89)
    Camera.Yaw = 0.0f;      // Поворот влево/вправо
    Camera.Fov = 60.0f;    // Поле зрения (1 до 90 градусов)
    Camera.AspectRatio = 16.0f / 9.0f; // Соотношение сторон
    Camera.Speed = 10.0f;  // Скорость движения
    Camera.AngularSpeed = 90.0f; // Скорость поворота (градусы/сек)
}

// Динамическое управление камерой в системе
public class CameraFollowSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        var camera = shared.Camera;
        
        // Плавное следование за объектом
        Vector3 targetPosition = GetTargetPosition(); // Ваша логика
        camera.Position += (targetPosition - camera.Position) * 0.1f;
        
        // Получение матриц камеры (автоматически кэшируются)
        Matrix4 view = camera.ViewMatrix;
        Matrix4 projection = camera.ProjectionMatrix;
    }
}
```

**Работа с аудио:**

```csharp
protected override void OnLoad()
{
    // Загрузка аудио файлов
    int backgroundMusic = Audio.LoadAudio("bgm", 
        Path.Combine("Assets", "Sounds", "background.mp3"));
    int jumpSound = Audio.LoadAudio("jump", 
        Path.Combine("Assets", "Sounds", "jump.mp3"));
    
    // Создание источника звука
    int musicSource = Audio.CreateSource("bgm", loop: true, volume: 0.5f);
    int jumpSource = Audio.CreateSource("jump", loop: false, volume: 1.0f);
    
    // Воспроизведение фоновой музыки
    Audio.Play(musicSource);
}

// В системе обновления
public class AudioSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        var audio = shared.Audio;
        
        // Воспроизведение звука прыжка при нажатии пробела
        if (IsJumpPressed())
        {
            int jumpSource = audio.CreateSource("jump");
            audio.Play(jumpSource);
        }
        
        // Обновление позиции 3D звука
        Vector3 playerPosition = GetPlayerPosition();
        int soundSource = GetSoundSource();
        audio.SetSourcePosition(soundSource, playerPosition);
    }
}
```

**Работа с логированием:**

```csharp
protected override void OnLoad()
{
    // Логирование через SystemSharedObject в системах
    // Или напрямую через RenderRites.Machine.Logger
    
    RenderRites.Machine.Logger.LogInfo("Scene loaded successfully");
    RenderRites.Machine.Logger.LogWarning("Low memory detected");
    RenderRites.Machine.Logger.LogError("Failed to load resource");
    
    try
    {
        // Код, который может выбросить исключение
    }
    catch (Exception ex)
    {
        RenderRites.Machine.Logger.LogException(LogLevel.Error, ex, 
            "Error occurred during initialization");
    }
}
```

## Оптимизации

- **Кэширование матриц камеры** - Матрицы view и projection пересчитываются только при изменении параметров камеры
- **Отслеживание активных шейдеров** - Обновляются только шейдеры, использованные в текущем кадре
- **Lazy evaluation** - Ресурсы загружаются по требованию
- **Система общих сервисов** - Ресурсы и сервисы разделяются между всеми сценами для экономии памяти
- **Frustum culling** - Автоматическое отсечение объектов вне видимости камеры
- **Batch rendering** - Группировка объектов с одинаковыми параметрами для эффективного рендеринга
- **Кэширование мешей** - Меши кэшируются в системах рендеринга для избежания повторных запросов

### Примеры оптимизации

**Использование frustum culling:**

```csharp
// Frustum culling включен по умолчанию в MainRenderSystem
// Для отключения (например, для отладки):
SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
shared.EnableFrustumCulling = false; // Отключить culling

// Получение статистики рендеринга
var stats = shared.RenderStats;
Console.WriteLine($"Total objects: {stats.TotalObjects}");
Console.WriteLine($"Rendered: {stats.RenderedObjects}");
Console.WriteLine($"Culled: {stats.CulledObjects}");
```

**Оптимизация загрузки ресурсов:**

```csharp
protected override void OnLoad()
{
    // Загружайте ресурсы только один раз
    // Они будут доступны во всех сценах через общий AssetsService
    
    // Хорошо: загрузка в OnLoad
    Assets.AddMeshFromFile("player", "path/to/player.obj");
    
    // Плохо: загрузка в каждом кадре
    // НЕ ДЕЛАЙТЕ ТАК:
    // void Update() { Assets.AddMeshFromFile(...); }
}
```

## Интерфейс пользователя

### ImGui интеграция

Движок включает встроенную поддержку ImGui для создания пользовательских интерфейсов:

```csharp
// Получение контекста ImGui
IntPtr context = RenderRites.Machine.Gui.GetContext();
if (context != IntPtr.Zero)
{
    ImGui.SetCurrentContext(context);
    // Использование ImGui API
}
```

### UI обертка

Для упрощения работы с ImGui предоставлена обертка `UI`:

```csharp
using RenderRitesMachine.UI;

// Создание окна
UI.Window("Мое окно").With(() =>
{
    UI.Text("Привет, мир!");
    if (UI.Button("Нажми меня"))
    {
        // Действие
    }
});

// Чекбоксы, слайдеры и другие виджеты
bool value = false;
UI.Checkbox("Опция", ref value);

float slider = 0.5f;
UI.SliderFloat("Значение", ref slider, 0.0f, 1.0f);
```

Подробнее см. [RenderRitesMachine/UI/README.md](RenderRitesMachine/UI/README.md)

## Тестирование

Проект включает полное покрытие тестами ключевых компонентов:

- **PerspectiveCameraTests** (40 тестов) - Тестирование камеры, кэширования матриц, валидации параметров, edge cases
- **SceneManagerTests** (20 тестов) - Тестирование управления сценами, добавления и переключения, edge cases
- **AssetsServiceTests** (47 тестов) - Тестирование валидации входных данных для загрузки ресурсов, edge cases
- **RenderConstantsTests** (12 тестов) - Тестирование корректности констант рендеринга
- **SystemSharedObjectTests** (18 тестов) - Тестирование управления шейдерами и матрицами, edge cases
- **FrustumTests** (9 тестов) - Тестирование frustum culling и работы с AABB
- **RayTests** (10 тестов) - Тестирование работы с лучами и пересечениями
- **LoggerTests** (20 тестов) - Тестирование системы логирования, edge cases
- **SceneFactoryTests** (5 тестов) - Тестирование фабрики сцен
- **AudioServiceTests** (32 теста) - Тестирование аудио сервиса

**Статистика:** 273 теста, все проходят успешно ✅

Тесты включают:
- Базовое функциональное тестирование
- Edge cases (экстремальные значения, граничные случаи)
- Валидацию входных данных
- Тестирование обработки ошибок

### Запуск тестов

```bash
# Запустить все тесты
dotnet test

# Запустить тесты с подробным выводом
dotnet test --verbosity normal

# Запустить конкретный класс тестов
dotnet test --filter "FullyQualifiedName~PerspectiveCameraTests"
```

Подробнее о тестах см. [RenderRitesMachine.Tests/README.md](RenderRitesMachine.Tests/README.md)

### Подход к тестированию OpenGL

Для тестирования компонентов, требующих OpenGL контекста, используется подход **Dependency Injection**:
- Создан интерфейс `IOpenGLWrapper` для абстракции OpenGL вызовов
- В тестах используются моки вместо реального OpenGL контекста
- Это позволяет тестировать логику без необходимости инициализации OpenGL

## Зависимости

### Основные библиотеки

- **OpenTK 4.9.4** - Обертка над OpenGL, OpenAL и GLFW
- **Leopotam.EcsLite 1.0.1** - Легковесная ECS библиотека
- **ImGui.NET 1.91.6.1** - .NET обертка над Dear ImGui
- **AssimpNetter 6.0.2.1** - Загрузка 3D моделей
- **StbImageSharp 2.30.15** - Загрузка изображений
- **StbTrueTypeSharp 1.26.12** - Загрузка шрифтов

## Сборка и запуск

```bash
# Восстановление зависимостей
dotnet restore

# Сборка решения
dotnet build

# Запуск демо-приложения
cd RenderRitesDemo
dotnet run

# Запуск тестов
cd RenderRitesMachine.Tests
dotnet test
```

## Обработка ошибок

Движок предоставляет подробные исключения для различных ситуаций:

### Примеры обработки ошибок

```csharp
protected override void OnLoad()
{
    try
    {
        // Загрузка ресурсов с обработкой ошибок
        Assets.AddShader("myShader", Path.Combine("Assets", "Shaders", "MyShader"));
    }
    catch (FileNotFoundException ex)
    {
        Logger.LogError($"Shader file not found: {ex.FileName}");
        // Fallback: используем дефолтный шейдер
        Assets.AddShader("cel", Path.Combine("Assets", "Shaders", "CelShading"));
    }
    catch (ShaderCompilationException ex)
    {
        Logger.LogException(LogLevel.Error, ex, "Shader compilation failed");
        // Обработка ошибки компиляции
    }
    catch (DuplicateResourceException ex)
    {
        Logger.LogWarning($"Resource already exists: {ex.ResourceName}");
        // Ресурс уже загружен, продолжаем
    }
    
    try
    {
        Assets.AddMeshFromFile("model", "path/to/model.obj");
    }
    catch (InvalidOperationException ex)
    {
        Logger.LogError($"Failed to load model: {ex.Message}");
        // Создаем простой меш программно как fallback
        Assets.AddSphere("fallback", 1.0f, 10, 10);
    }
}

// Валидация параметров камеры
try
{
    Camera.AspectRatio = -1.0f; // Недопустимое значение
}
catch (ArgumentOutOfRangeException ex)
{
    Logger.LogError($"Invalid aspect ratio: {ex.Message}");
    Camera.AspectRatio = 16.0f / 9.0f; // Значение по умолчанию
}

try
{
    Camera.Fov = 120.0f; // Вне допустимого диапазона
}
catch (ArgumentOutOfRangeException ex)
{
    Logger.LogError($"Invalid FOV: {ex.Message}");
    Camera.Fov = 60.0f; // Значение по умолчанию
}
```

## Лучшие практики

### 1. Организация кода

```csharp
// ✅ Хорошо: Разделение на методы
protected override void OnLoad()
{
    LoadAssets();
    CreateEntities();
    SetupCamera();
    SetupSystems();
}

// ❌ Плохо: Все в одном методе
protected override void OnLoad()
{
    // 200+ строк кода...
}
```

### 2. Управление ресурсами

```csharp
// ✅ Хорошо: Загрузка ресурсов в OnLoad
protected override void OnLoad()
{
    Assets.AddShader("myShader", "path");
    Assets.AddMeshFromFile("model", "path");
}

// ❌ Плохо: Загрузка в каждом кадре
public void Update()
{
    Assets.AddMeshFromFile("model", "path"); // НЕ ДЕЛАЙТЕ ТАК!
}
```

### 3. Работа с ECS

```csharp
// ✅ Хорошо: Использование фильтров
EcsFilter filter = world
    .Filter<Transform>()
    .Inc<Mesh>()
    .End();

foreach (int entity in filter)
{
    // Обработка только нужных сущностей
}

// ❌ Плохо: Перебор всех сущностей
var allEntities = world.GetAllEntities();
foreach (int entity in allEntities)
{
    // Медленно и неэффективно
}
```

### 4. Управление камерой

```csharp
// ✅ Хорошо: Использование свойств камеры
Camera.Position = new Vector3(0, 5, 10);
Camera.Pitch = -20.0f;

// ❌ Плохо: Прямое изменение внутренних полей
// Не делайте так, используйте публичные свойства
```

### 5. Обработка времени

```csharp
// ✅ Хорошо: Использование delta time
public void Run(IEcsSystems systems)
{
    SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
    float deltaTime = shared.Time.UpdateDeltaTime;
    
    transform.Position += velocity * deltaTime;
}

// ❌ Плохо: Использование фиксированных значений
transform.Position += velocity * 0.016f; // Не зависит от FPS
```

### 6. Логирование

```csharp
// ✅ Хорошо: Использование правильных уровней логирования
Logger.LogDebug("Detailed debug information");
Logger.LogInfo("General information");
Logger.LogWarning("Warning message");
Logger.LogError("Error occurred");
Logger.LogCritical("Critical error!");

// ❌ Плохо: Все через LogError
Logger.LogError("Debug info"); // Неправильный уровень
Logger.LogError("Info message"); // Неправильный уровень
```

## Расширенные примеры

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

### Работа с UI в системах

```csharp
public class GameUISystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        // Использование UI обертки
        UI.Window("Game HUD").With(() =>
        {
            UI.Text($"Health: {GetPlayerHealth()}");
            UI.Text($"Score: {GetScore()}");
            
            if (UI.Button("Pause"))
            {
                shared.SceneManager.SwitchTo("pause");
            }
        });
        
        // Или прямой доступ к ImGui
        IntPtr context = shared.Gui.GetContext();
        if (context != IntPtr.Zero)
        {
            ImGui.SetCurrentContext(context);
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Game"))
                {
                    if (ImGui.MenuItem("Restart"))
                    {
                        RestartGame();
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
        }
    }
}
```

## Дополнительные ресурсы

- [EXAMPLES.md](EXAMPLES.md) - Расширенные примеры использования движка
- [RenderRitesMachine/UI/README.md](RenderRitesMachine/UI/README.md) - Документация по UI обертке
- [RenderRitesMachine.Tests/README.md](RenderRitesMachine.Tests/README.md) - Документация по тестированию

## Лицензия

Этот проект распространяется под лицензией GNU General Public License v3.0 (GPL-3.0).

См. файл [LICENSE](LICENSE) для полного текста лицензии.
