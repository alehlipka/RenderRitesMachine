using Leopotam.EcsLite;

namespace RenderRitesMachine.ECS.Components;

public struct Mesh : IEcsAutoReset<Mesh>
{
    public string Name;
    public bool IsVisible;

    public void AutoReset(ref Mesh c)
    {
        c.IsVisible = true;
    }
}

