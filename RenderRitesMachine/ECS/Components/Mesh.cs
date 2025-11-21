using Leopotam.EcsLite;

namespace RenderRitesMachine.ECS.Components;

public struct Mesh : IEcsAutoReset<Mesh>
{
    public Mesh()
    {
        Name = string.Empty;
        ShaderName = "cel";
        IsVisible = true;
    }

    public string Name { get; set; }
    public string ShaderName { get; set; }
    public bool IsVisible { get; set; }

    public readonly void AutoReset(ref Mesh c)
    {
        c.IsVisible = true;
        c.ShaderName = "cel";
        c.Name = string.Empty;
    }
}

