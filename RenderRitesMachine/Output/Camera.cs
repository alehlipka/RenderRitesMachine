using OpenTK.Mathematics;

namespace RenderRitesMachine.Output;

public abstract class Camera
{
    public Vector3 Position { get; set; }
    public Vector3 Target { get; set; }
    public Vector3 Up { get; set; } = Vector3.UnitY;

    public Matrix4 ViewMatrix => Matrix4.LookAt(Position, Target, Up);
    public abstract Matrix4 ProjectionMatrix { get; }
}