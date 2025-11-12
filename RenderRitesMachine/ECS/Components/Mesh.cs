using Leopotam.EcsLite;

namespace RenderRitesMachine.ECS.Components;

public struct Mesh : IEcsAutoReset<Mesh>
{
    public string Name;
    public string ShaderName;
    public bool IsVisible;

    public void AutoReset(ref Mesh c)
    {
        c.IsVisible = true;
        c.ShaderName = "cel";
    }
}

