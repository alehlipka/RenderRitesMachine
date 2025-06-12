using OpenTK.Mathematics;
using RenderRitesMachine.Utilities;

namespace RenderRitesMachine.ECS.Components;

public struct TransformComponent(Vector3 position, RotationInfo? rotation = null, Vector3? scale = null)
    : IComponent
{
    public Vector3 Position = position;
    public readonly RotationInfo Rotation = rotation ?? new RotationInfo();
    public Vector3 Scale = scale ?? Vector3.One;

    public Matrix4 ModelMatrix => Matrix4.CreateScale(Scale) *
                                  Rotation.Matrix *
                                  Matrix4.CreateTranslation(Position);

    public void Dispose()
    {
        
    }
}