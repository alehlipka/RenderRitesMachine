using Leopotam.EcsLite;

namespace RenderRitesDemo.ECS;

public struct Mesh : IEcsAutoReset<Mesh>
{
    public string Name;
    public bool IsVisible;

    public void AutoReset(ref Mesh component)
    {
        component.IsVisible = true;
    }
}
