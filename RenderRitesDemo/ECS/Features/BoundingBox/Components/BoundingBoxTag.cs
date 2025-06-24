using Leopotam.EcsLite;

namespace RenderRitesDemo.ECS.Features.BoundingBox.Components;

public struct BoundingBoxTag : IEcsAutoReset<BoundingBoxTag>
{
    public bool IsVisible;
    
    public void AutoReset(ref BoundingBoxTag c)
    {
        c.IsVisible = true;
    }
}