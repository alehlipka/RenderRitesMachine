# Лучшие практики

## 1. Организация кода

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

## 2. Управление ресурсами

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

## 3. Работа с ECS

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

## 4. Управление камерой

```csharp
// ✅ Хорошо: Использование свойств камеры
Camera.Position = new Vector3(0, 5, 10);
Camera.Pitch = -20.0f;

// ❌ Плохо: Прямое изменение внутренних полей
// Не делайте так, используйте публичные свойства
```

## 5. Обработка времени

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

## 6. Логирование

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

