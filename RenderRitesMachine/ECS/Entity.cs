namespace RenderRitesMachine.ECS;

public readonly record struct Entity(int Id)
{
    public int Id { get; init; } = Id;
}