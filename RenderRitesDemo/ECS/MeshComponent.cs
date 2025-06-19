using Leopotam.EcsLite;

namespace RenderRitesDemo.ECS;

public struct MeshComponent : IEcsAutoReset<MeshComponent>
{
    public string Name;
    public bool IsVisible;

    public void AutoReset(ref MeshComponent component)
    {
        component.IsVisible = true;
    }
}
