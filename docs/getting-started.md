# Быстрый старт

## Требования

- .NET 9.0 или выше
- OpenGL 4.6 совместимая видеокарта
- Операционная система: Windows, Linux или macOS
- OpenAI библиотека для работы аудио системы

**Примечание:** Для работы аудио используется OpenAI. Убедитесь, что библиотека OpenAI установлена и настроена перед использованием аудио функций.

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

## Создание сцены

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

## Запуск приложения

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

## Структура проекта

```
RenderRites/
├── RenderRitesMachine/             # Ядро движка
│   ├── Assets/                     # Типы ассетов (Mesh, Shader, Texture, BoundingBox)
│   ├── Configuration/              # Константы и конфигурация
│   ├── Debug/                      # Инструменты отладки (FPS, OpenGL debug)
│   ├── ECS/                        # ECS компоненты и системы
│   │   ├── Components/             # Базовые компоненты (Transform, Mesh, ColorTexture)
│   │   └── Systems/                # Базовые системы (MainRenderSystem, MainResizeSystem)
│   ├── Output/                     # Окно, сцены, камера, менеджер сцен
│   ├── Services/                   # Сервисы (Assets, Render, Time, Gui)
│   ├── UI/                         # Обертка над ImGui (UIWindow, UICombo, UIListBox, UIMenu)
│   └── Utilities/                  # Утилиты (Ray, и т.д.)
├── RenderRitesDemo/                # Демо-приложение
│   ├── Assets/                     # Ресурсы (шейдеры, модели, текстуры, шрифты)
│   │   ├── Shaders/                # Шейдеры (CelShading, Outline, Bounding)
│   │   ├── Objects/                # 3D модели (.obj)
│   │   └── Textures/               # Текстуры
│   ├── ECS/                        # Демо-системы и компоненты
│   │   └── Features/               # Демо-фичи
│   │       ├── BoundingBox/        # Визуализация bounding box
│   │       ├── Input/              # Обработка ввода
│   │       ├── Outline/            # Рендеринг контуров
│   │       ├── Rotation/           # Вращение объектов
│   │       └── SceneSwitch/        # Переключение сцен
│   └── Scenes/                     # Демо-сцены (DemoScene, GuiTestScene)
└── RenderRitesMachine.Tests/       # Проект тестов
    ├── PerspectiveCameraTests.cs
    ├── SceneManagerTests.cs
    ├── AssetsServiceTests.cs
    ├── RenderConstantsTests.cs
    └── SystemSharedObjectTests.cs
```

