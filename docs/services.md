# Сервисы

Движок предоставляет набор сервисов для работы с различными аспектами игры:

- **IAssetsService** - Управление ресурсами (меши, шейдеры, текстуры, bounding boxes)
- **ITimeService** - Управление временем (delta time для update и render)
- **IRenderService** - Сервис рендеринга
- **IGuiService** - Доступ к контексту ImGui для создания интерфейсов
- **IAudioService** - Управление аудио (загрузка и воспроизведение звуков)
- **ILogger** - Система логирования

## Работа с камерой

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

## Работа с аудио

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

## Работа с логированием

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

