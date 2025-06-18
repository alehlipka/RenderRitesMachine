using OpenTK.Mathematics;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesMachine.ECS.Features.PerspectiveCamera.Components;

public struct PerspectiveCameraComponent() : IComponent
{
    public Vector3 Position { get; set; } = Vector3.Zero;
    public Vector3 Target { get; set; } = Vector3.Zero;
    public Vector3 Up { get; set; } = Vector3.UnitY;
    public float FieldOfView { get; set; } = MathHelper.PiOver4;
    public float NearPlane { get; set; } = 0.01f;
    public float FarPlane { get; set; } = 10000f;

    private static float AspectRatio => RenderRites.Machine.Window!.ClientSize.X / (float)RenderRites.Machine.Window.ClientSize.Y;
    
    public Matrix4 ViewMatrix => Matrix4.LookAt(Position, Target, Up);
    public Matrix4 ProjectionMatrix => Matrix4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);

    public void Dispose()
    {
        
    }
}