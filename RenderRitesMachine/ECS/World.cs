using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Systems;

namespace RenderRitesMachine.ECS;

public static class World
{
    private static readonly List<Entity> _entities = [];
    private static Dictionary<Type, Dictionary<Entity, IComponent>> _components = new();
    private static readonly List<ISystem> _systems = [];
    
    public static Entity CreateEntity()
    {
        Entity entity = new(_entities.Count + 1);
        _entities.Add(entity);
        
        return entity;
    }
    
    public static void AddComponent<T>(Entity entity, T component) where T : IComponent
    {
        if (!_components.ContainsKey(typeof(T)))
        {
            _components[typeof(T)] = new Dictionary<Entity, IComponent>();
        }

        _components[typeof(T)][entity] = component;
    }
    
    public static T GetComponent<T>(Entity entity) where T : IComponent
    {
        return (T)_components[typeof(T)][entity];
    }

    // public static T GetComponents<T>() where T : IComponent
    // {
    //     
    // }
    
    public static IEnumerable<(T1, T2)> GetComponents<T1, T2>() where T1 : IComponent where T2 : IComponent
    {
        foreach (Entity entity in _entities)
        {
            if (_components.TryGetValue(typeof(T1), out var comp1) &&
                comp1.TryGetValue(entity, out IComponent? c1) &&
                _components.TryGetValue(typeof(T2), out var comp2) &&
                comp2.TryGetValue(entity, out IComponent? c2))
            {
                yield return ((T1)c1, (T2)c2);
            }
        }
    }

    public static void AddSystem(ISystem system)
    {
        _systems.Add(system);
    }

    public static void Update(float deltaTime)
    {
        _systems.ForEach(system => system.Update(deltaTime));
    }
}