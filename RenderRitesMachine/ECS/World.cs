using System.Runtime.CompilerServices;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Systems.Contracts;

namespace RenderRitesMachine.ECS;

public class World
{
    private readonly List<Entity> _entities = [];
    private readonly List<ISystem> _systems = [];
    private readonly Dictionary<Type, Dictionary<Entity, IComponent>> _components = new();
    private static readonly Dictionary<Type[], Func<IComponent[], ITuple>> TupleFactories = new();
    private readonly Dictionary<Type, List<ISystem>> _systemsByInterface = new();

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

    public T GetEntityComponent<T>(Entity entity) where T : IComponent
    {
        return (T)_components[typeof(T)][entity];
    }

    public IEnumerable<ITuple> GetComponents(params Type[] componentTypes)
    {
        var dictionaries = new Dictionary<Entity, IComponent>[componentTypes.Length];
        for (int i = 0; i < componentTypes.Length; i++)
        {
            if (!_components.TryGetValue(componentTypes[i], out var dict))
            {
                yield break;
            }

            dictionaries[i] = dict;
        }

        var tupleFactory = GetTupleFactory(componentTypes);
        var components = new IComponent[componentTypes.Length];

        foreach (bool unused
             in _entities
                 .Select(entity => !dictionaries
                     .Where((t, i) => !t.TryGetValue(entity, out components[i]!))
                     .Any())
                 .Where(hasAllComponents => hasAllComponents)
        )
        {
            yield return tupleFactory(components);
        }
    }

    public void AddSystem(ISystem system)
    {
        _systems.Add(system);
        UpdateInterfaceGroups(system);
    }
    
    public void RemoveSystem(ISystem system)
    {
        _systems.Remove(system);
        RebuildInterfaceGroups();
    }

    public void Resize(int width, int height)
    {
        GetSystemsByInterface<IResizeSystem>().ForEach(system => system.Resize(width, height, this));
    }

    public void Update(float deltaTime)
    {
        GetSystemsByInterface<IUpdateSystem>().ForEach(system => system.Update(deltaTime, this));
    }

    public void Render(float deltaTime)
    {
        GetSystemsByInterface<IRenderSystem>().ForEach(system => system.Render(deltaTime, this));
    }
    
    private static Func<IComponent[], ITuple> GetTupleFactory(Type[] componentTypes)
    {
        if (TupleFactories.TryGetValue(componentTypes, out var factory))
        {
            return factory;
        }
        
        factory = CreateTupleFactory(componentTypes);
        TupleFactories[componentTypes] = factory;

        return factory;
    }

    private static Func<IComponent[], ITuple> CreateTupleFactory(Type[] componentTypes)
    {
        return components =>
        {
            return componentTypes.Length switch
            {
                1 => ValueTuple.Create((dynamic)components[0]),
                2 => ValueTuple.Create((dynamic)components[0], (dynamic)components[1]),
                3 => ValueTuple.Create((dynamic)components[0], (dynamic)components[1], (dynamic)components[2]),
                4 => ValueTuple.Create((dynamic)components[0], (dynamic)components[1], (dynamic)components[2],
                    (dynamic)components[3]),
                5 => ValueTuple.Create((dynamic)components[0], (dynamic)components[1], (dynamic)components[2],
                    (dynamic)components[3], (dynamic)components[4]),
                6 => ValueTuple.Create((dynamic)components[0], (dynamic)components[1], (dynamic)components[2],
                    (dynamic)components[3], (dynamic)components[4], (dynamic)components[5]),
                7 => ValueTuple.Create((dynamic)components[0], (dynamic)components[1], (dynamic)components[2],
                    (dynamic)components[3], (dynamic)components[4], (dynamic)components[5], (dynamic)components[6]),
                8 => ValueTuple.Create((dynamic)components[0], (dynamic)components[1], (dynamic)components[2],
                    (dynamic)components[3], (dynamic)components[4], (dynamic)components[5], (dynamic)components[6],
                    (dynamic)components[7]),
                _ => throw new NotSupportedException("Too many components")
            };
        };
    }

    private List<T> GetSystemsByInterface<T>() where T : class, ISystem
    {
        return _systemsByInterface.TryGetValue(typeof(T), out var systems)
            ? [..systems.Cast<T>()]
            : [];
    }
    
    private void UpdateInterfaceGroups(ISystem system)
    {
        foreach (Type interfaceType in GetSupportedInterfaces(system))
        {
            if (!_systemsByInterface.ContainsKey(interfaceType))
            {
                _systemsByInterface[interfaceType] = [];
            }
            
            _systemsByInterface[interfaceType].Add(system);
        }
    }
    
    private static IEnumerable<Type> GetSupportedInterfaces(ISystem system)
    {
        return system
            .GetType()
            .GetInterfaces()
            .Where(type => type != typeof(ISystem) && typeof(ISystem).IsAssignableFrom(type));
    }
    
    private void RebuildInterfaceGroups()
    {
        _systemsByInterface.Clear();
        _systems.ForEach(UpdateInterfaceGroups);
    }
}