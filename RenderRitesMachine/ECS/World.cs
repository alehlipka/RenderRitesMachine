using System.Runtime.CompilerServices;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Systems;

namespace RenderRitesMachine.ECS;

public class World : IDisposable
{
    private readonly List<Entity> _entities = [];
    private readonly Dictionary<Type, Dictionary<Entity, IComponent>> _components = new();
    private static readonly Dictionary<Type[], Func<IComponent[], ITuple>> TupleFactories = new();
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

        foreach (bool hasAllComponents in _entities
                     .Select(entity => !dictionaries
                         .Where((t, i) => !t.TryGetValue(entity, out components[i]!))
                         .Any())
                     .Where(hasAllComponents => hasAllComponents))
        {
            yield return tupleFactory(components);
        }
    }

    private static Func<IComponent[], ITuple> GetTupleFactory(Type[] componentTypes)
    {
        if (!TupleFactories.TryGetValue(componentTypes, out var factory))
        {
            factory = CreateTupleFactory(componentTypes);
            TupleFactories[componentTypes] = factory;
        }

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
        var entityComponentDict = _components.Values.SelectMany(entityComponentDict => entityComponentDict.Values
        );

        foreach (IComponent component in entityComponentDict)
        {
            component.Dispose();
        }

        _systems.Clear();
        _components.Clear();
        _entities.Clear();
    }
}