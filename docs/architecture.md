# Архитектура

## Entity Component System (ECS)

Проект использует архитектуру ECS для организации игровой логики:

- **Entities** - Идентификаторы сущностей (int)
- **Components** - Данные (Transform, Mesh, ColorTexture, и т.д.)
- **Systems** - Логика обработки (RenderSystem, UpdateSystem, и т.д.)

## Создание кастомного компонента

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

## Создание кастомной системы

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

## Использование кастомных систем в сцене

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

## Системы рендеринга

- `MainRenderSystem` - Основной рендеринг объектов
- `OutlineRenderSystem` - Рендеринг контуров объектов
- `BoundingBoxRenderSystem` - Визуализация bounding box

## Системы обновления

- `InputUpdateSystem` - Обработка ввода пользователя (клавиатура, мышь)
- `RotationUpdateSystem` - Вращение объектов с компонентом `RotationTag`
- `OutlineUpdateSystem` - Обновление видимости контуров объектов
- `SceneSwitchSystem` - Переключение между сценами во время выполнения

## Демо-сцены

- **LogoScene** - Автоматически добавляемая сцена с логотипом движка
- **DemoScene** - Демонстрация основных возможностей: рендеринг моделей, контуры, bounding box, вращение
- **GuiTestScene** - Демонстрация элементов интерфейса через ImGui

