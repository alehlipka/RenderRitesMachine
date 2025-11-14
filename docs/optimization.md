# Оптимизации

Движок включает несколько оптимизаций для повышения производительности:

- **Кэширование матриц камеры** - Матрицы view и projection пересчитываются только при изменении параметров камеры
- **Отслеживание активных шейдеров** - Обновляются только шейдеры, использованные в текущем кадре
- **Lazy evaluation** - Ресурсы загружаются по требованию
- **Система общих сервисов** - Ресурсы и сервисы разделяются между всеми сценами для экономии памяти
- **Кэширование мешей** - Меши кэшируются в системах рендеринга для избежания повторных запросов

## Получение статистики рендеринга

```csharp
// Получение статистики рендеринга
SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
var stats = shared.RenderStats;
Console.WriteLine($"Total objects: {stats.TotalObjects}");
Console.WriteLine($"Rendered: {stats.RenderedObjects}");
```

## Оптимизация загрузки ресурсов

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

