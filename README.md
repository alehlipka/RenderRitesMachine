# RenderRites Machine

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![OpenGL](https://img.shields.io/badge/OpenGL-4.6-green.svg)](https://www.opengl.org/)
[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Linux-lightgrey.svg)](https://github.com/yourusername/RenderRites)
[![License](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](LICENSE)
[![CI](https://github.com/alehlipka/RenderRitesMachine/actions/workflows/ci.yml/badge.svg)](https://github.com/alehlipka/RenderRitesMachine/actions)

**RenderRites Machine** — это современный мультиплатформенный 3D игровой движок, построенный на C# и .NET 9.0. Движок использует архитектуру Entity Component System (ECS) и предоставляет полный набор инструментов для создания 3D приложений и игр. Протестирован и работает на Windows и Linux.

## 🚀 Основные возможности

- **3D рендеринг**: Поддержка мешей, шейдеров, текстур и камер (перспективная и ортогональная)
- **ECS архитектура**: Использование Leopotam.EcsLite для эффективного управления сущностями и компонентами
- **Управление сценами**: Гибкая система сцен с возможностью переключения между ними
- **Загрузка ресурсов**: Автоматическая загрузка мешей (OBJ через Assimp), шейдеров (GLSL) и текстур
- **Аудио система**: Воспроизведение звуков и музыки через OpenAL (декодирование через NLayer)
- **GUI система**: Программный рендеринг интерфейса с поддержкой шрифтов (TrueType)
- **Сервисы**: Встроенные сервисы для времени, логирования, рендеринга и управления ресурсами
- **Отладка**: Инструменты для отслеживания FPS, времени кадра и статистики рендеринга
- **OpenGL 4.6**: Современный OpenGL с поддержкой отладочного контекста
- **Мультиплатформенность**: Кроссплатформенная поддержка Windows и Linux

## 📋 Требования

- **.NET 9.0 SDK** или новее
- **OpenGL 4.6** совместимая видеокарта
- **OpenAL** (для аудио системы)
- **Поддерживаемые платформы**:
  - **Windows** (протестировано на Windows 10/11)
  - **Linux** (протестировано на современных дистрибутивах)
- **IDE** (опционально): Visual Studio 2022, JetBrains Rider, или Visual Studio Code

## 🛠️ Установка

1. Клонируйте репозиторий:
```bash
git clone https://github.com/yourusername/RenderRites.git
cd RenderRites
```

2. Восстановите зависимости NuGet:
```bash
dotnet restore
```

3. Соберите решение:
```bash
dotnet build
```

4. Запустите демо:
```bash
cd RenderRitesDemo
dotnet run
```

## 🎮 Быстрый старт

Создание простого приложения с RenderRites Machine:

```csharp
using RenderRitesMachine;
using RenderRitesDemo.Configuration;
using RenderRitesDemo.Scenes.Demo;

// Настройка движка
RenderRites.Machine
    .ConfigureRenderSettings(DemoRenderConfiguration.Create())
    .Scenes
    .AddScene<DemoScene>("demo");

// Запуск окна
RenderRites.Machine.RunWindow("My Game");
```

### Создание собственной сцены

```csharp
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;
using RenderRitesMachine.Services.Gui;
using RenderRitesMachine.ECS.Components;
using OpenTK.Mathematics;

public class MyScene : Scene
{
    public MyScene(string name, IAssetsService assetsService, 
                   ITimeService timeService, IRenderService renderService,
                   IAudioService audioService, IGuiService guiService,
                   ISceneManager sceneManager, ILogger logger)
        : base(name, assetsService, timeService, renderService, 
               audioService, guiService, sceneManager, logger)
    {
    }

    protected override void OnLoad()
    {
        // Настройка камеры
        Camera.Position = new Vector3(0, 2, 5);
        Camera.Yaw = -90f;
        Camera.Pitch = -20f;

        // Загрузка ресурсов
        Assets.AddShader("my-shader", "path/to/shaders");
        Assets.AddMeshFromFile("my-mesh", "path/to/mesh.obj");
        Assets.AddTexture("my-texture", TextureType.ColorMap, "path/to/texture.jpg");

        // Создание сущности
        int entity = World.NewEntity();
        
        ref Transform transform = ref World.GetPool<Transform>().Add(entity);
        transform.Position = Vector3.Zero;
        transform.Scale = Vector3.One;
        
        ref Mesh mesh = ref World.GetPool<Mesh>().Add(entity);
        mesh.Name = "my-mesh";
        mesh.ShaderName = "my-shader";
        
        ref ColorTexture texture = ref World.GetPool<ColorTexture>().Add(entity);
        texture.Name = "my-texture";

        // Регистрация систем
        _ = UpdateSystems.Add(new MyUpdateSystem());
        _ = RenderSystems.Add(new MainRenderSystem());
    }
}
```

## 📁 Структура проекта

```
RenderRites/
├── RenderRitesMachine/          # Основная библиотека движка
│   ├── Assets/                 # Классы для управления ресурсами
│   ├── Configuration/           # Настройки рендеринга
│   ├── Debug/                   # Инструменты отладки
│   ├── ECS/                     # Entity Component System
│   │   ├── Components/          # Компоненты (Transform, Mesh, ColorTexture)
│   │   └── Systems/             # Системы рендеринга и обновления
│   ├── Exceptions/              # Исключения движка
│   ├── Output/                  # Камеры, сцены, окна, шейдеры
│   └── Services/                # Сервисы (Audio, Graphics, Gui, Timing)
│
├── RenderRitesDemo/             # Демо-приложение
│   ├── Assets/                  # Ресурсы демо (модели, текстуры, шейдеры)
│   ├── Configuration/           # Конфигурация демо
│   └── Scenes/                  # Сцены демо
│
└── RenderRitesMachine.Tests/    # Юнит-тесты
```

## 🏗️ Архитектура

### Entity Component System (ECS)

RenderRites использует ECS архитектуру для эффективного управления игровыми объектами:

- **Entities (Сущности)**: Идентификаторы объектов
- **Components (Компоненты)**: Данные (Transform, Mesh, ColorTexture)
- **Systems (Системы)**: Логика обработки (UpdateSystems, RenderSystems, ResizeSystems)

### Система сцен

Каждая сцена имеет:
- Собственный ECS мир
- Камеру (перспективную или ортогональную)
- Системы обновления, рендеринга и обработки изменения размера
- Доступ к общим сервисам (Assets, Audio, Gui, Time)

### Сервисы

- **IAssetsService**: Загрузка и управление мешами, шейдерами, текстурами
- **IAudioService**: Воспроизведение звуков и музыки через OpenAL
- **IGuiService**: Рендеринг GUI элементов
- **ITimeService**: Управление временем (delta time для update и render)
- **IRenderService**: Низкоуровневый рендеринг примитивов
- **ILogger**: Логирование сообщений и ошибок

## 📚 Примеры использования

### Загрузка ресурсов

```csharp
// Загрузка шейдера (vertex.glsl и fragment.glsl в папке)
Assets.AddShader("shader-name", "path/to/shader/folder");

// Загрузка меша из файла
Assets.AddMeshFromFile("mesh-name", "path/to/mesh.obj");

// Загрузка текстуры
Assets.AddTexture("texture-name", TextureType.ColorMap, "path/to/texture.jpg");
```

### Работа с камерой

```csharp
// Перспективная камера (по умолчанию)
Camera.Position = new Vector3(0, 5, 10);
Camera.Yaw = -90f;      // Поворот по горизонтали
Camera.Pitch = -20f;    // Поворот по вертикали
Camera.AspectRatio = 16f / 9f;

// Ортогональная камера
var orthoCamera = new OrthographicCamera();
```

### Создание систем

```csharp
public class MyUpdateSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        var world = systems.GetWorld();
        var shared = systems.GetShared<SystemSharedObject>();
        
        var filter = world.Filter<Transform>().End();
        var transforms = world.GetPool<Transform>();
        
        foreach (int entity in filter)
        {
            ref Transform transform = ref transforms.Get(entity);
            // Обновление логики
        }
    }
}
```

### Настройка рендеринга

```csharp
var settings = new RenderSettings
{
    DefaultWindowWidth = 1920,
    DefaultWindowHeight = 1080,
    DefaultVSyncMode = VSyncMode.On,
    DefaultSamples = 8,  // MSAA
    UpdateFrequency = 60.0
};

RenderRites.Machine.ConfigureRenderSettings(settings);
```

## 🧪 Тестирование

Запуск тестов:

```bash
dotnet test
```

## 🔄 CI/CD

Проект использует **GitHub Actions** для автоматической сборки и тестирования на Windows и Linux. Каждый Pull Request в **main** автоматически проверяется на обеих платформах.

Статус последних сборок можно посмотреть в разделе [Actions](https://github.com/alehlipka/RenderRitesMachine/actions) репозитория.

## 📦 Зависимости

- **OpenTK 4.9.4**: Оконная система и OpenGL биндинги
- **Leopotam.EcsLite 1.0.1**: ECS фреймворк
- **AssimpNetter 6.0.2.1**: Загрузка 3D моделей
- **StbImageSharp 2.30.15**: Загрузка изображений
- **StbTrueTypeSharp 1.26.12**: Рендеринг шрифтов
- **OpenAL**: Кроссплатформенная аудио библиотека для воспроизведения звука
- **NLayer 1.16.0**: Декодирование аудио файлов

## 🤝 Вклад в проект

Мы приветствуем вклад в развитие проекта! Пожалуйста:

1. Создайте форк репозитория
2. Создайте ветку для новой функции (`git checkout -b feature/AmazingFeature`)
3. Зафиксируйте изменения (`git commit -m 'Add some AmazingFeature'`)
4. Отправьте в ветку (`git push origin feature/AmazingFeature`)
5. Откройте Pull Request

## 📝 Лицензия

Этот проект распространяется под лицензией **GNU General Public License v3.0**. См. файл [LICENSE](LICENSE) для подробностей.

## 🙏 Благодарности

- [OpenTK](https://github.com/opentk/opentk) за отличные OpenGL биндинги
- [OpenAL](https://www.openal.org/) за кроссплатформенную аудио библиотеку
- [Leopotam](https://github.com/Leopotam/ecslite) за легковесный ECS фреймворк
- [Assimp](https://github.com/assimp/assimp) за поддержку множества форматов 3D моделей

## 📧 Контакты

Если у вас есть вопросы или предложения, создайте [Issue](https://github.com/yourusername/RenderRites/issues) в репозитории.

---

**Сделано с ❤️ для сообщества разработчиков игр**