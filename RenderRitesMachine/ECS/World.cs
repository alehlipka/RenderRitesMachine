using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Systems;

namespace RenderRitesMachine.ECS;

public class World : IDisposable
{
    private readonly List<Entity> _entities = [];
    private readonly Dictionary<Type, Dictionary<Entity, IComponent>> _components = new();
    private readonly List<ISystem> _systems = [];
    
    public Entity CreateEntity()
    {
        Entity entity = new(_entities.Count + 1);
        _entities.Add(entity);
        
        return entity;
    }
    
    public void AddComponent<T>(Entity entity, T component) where T : IComponent
    {
        if (!_components.ContainsKey(typeof(T)))
        {
            _components[typeof(T)] = new Dictionary<Entity, IComponent>();
        }

        _components[typeof(T)][entity] = component;
    }
    
    public T GetComponent<T>(Entity entity) where T : IComponent
    {
        return (T)_components[typeof(T)][entity];
    }
    
    public IEnumerable<T1> GetComponents<T1>() where T1 : IComponent
    {
        foreach (Entity entity in _entities)
        {
            if (
                _components.TryGetValue(typeof(T1), out var dict) && dict.TryGetValue(entity, out IComponent? c1)
            )
            {
                yield return (T1)c1;
            }
        }
    }
    
    public IEnumerable<(T1, T2)> GetComponents<T1, T2>() where T1 : IComponent where T2 : IComponent
    {
        foreach (Entity entity in _entities)
        {
            if (
                _components.TryGetValue(typeof(T1), out var dict1) && dict1.TryGetValue(entity, out IComponent? c1) &&
                _components.TryGetValue(typeof(T2), out var dict2) && dict2.TryGetValue(entity, out IComponent? c2)
            )
            {
                yield return ((T1)c1, (T2)c2);
            }
        }
    }
    
    public IEnumerable<(T1, T2, T3)> GetComponents<T1, T2, T3>() where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        foreach (Entity entity in _entities)
        {
            if (
                _components.TryGetValue(typeof(T1), out var dict1) && dict1.TryGetValue(entity, out IComponent? c1) &&
                _components.TryGetValue(typeof(T2), out var dict2) && dict2.TryGetValue(entity, out IComponent? c2) &&
                _components.TryGetValue(typeof(T3), out var dict3) && dict3.TryGetValue(entity, out IComponent? c3)
            )
            {
                yield return ((T1)c1, (T2)c2, (T3)c3);
            }
        }
    }
    
    public IEnumerable<(T1, T2, T3, T4)> GetComponents<T1, T2, T3, T4>() where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
    {
        foreach (Entity entity in _entities)
        {
            if (
                _components.TryGetValue(typeof(T1), out var dict1) && dict1.TryGetValue(entity, out IComponent? c1) &&
                _components.TryGetValue(typeof(T2), out var dict2) && dict2.TryGetValue(entity, out IComponent? c2) &&
                _components.TryGetValue(typeof(T3), out var dict3) && dict3.TryGetValue(entity, out IComponent? c3) &&
                _components.TryGetValue(typeof(T4), out var dict4) && dict4.TryGetValue(entity, out IComponent? c4)
            )
            {
                yield return ((T1)c1, (T2)c2, (T3)c3, (T4)c4);
            }
        }
    }

    public void AddSystem(ISystem system)
    {
        _systems.Add(system);
    }

    public void Resize(int width, int height)
    {
        _systems.ForEach(system => system.Resize(width, height, this));
    }

    public void Update(float deltaTime)
    {
        _systems.ForEach(system => system.Update(deltaTime, this));
    }
    
    public void Render(float deltaTime)
    {
        _systems.ForEach(system => system.Render(deltaTime, this));
    }

    public void Dispose()
    {
        _systems.ForEach(system => system.Dispose());
        var entityComponentDict = _components.Values.SelectMany(
            entityComponentDict => entityComponentDict.Values
        );
        
        foreach (IComponent component in entityComponentDict)
        {
            component.Dispose();
        }
        
        _entities.Clear();
        _systems.Clear();
        _components.Clear();
    }
}