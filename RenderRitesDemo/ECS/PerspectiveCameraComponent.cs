using Leopotam.EcsLite;
using OpenTK.Mathematics;
using RenderRitesMachine;

namespace RenderRitesDemo.ECS;

public struct PerspectiveCameraComponent : IEcsAutoReset<PerspectiveCameraComponent>
{
    public Vector3 Position;
    public Vector3 Target;
    public Vector3 Up;
    public float FieldOfView;
    public float NearPlane;
    public float FarPlane;
    public float AspectRatio;
    
    public Matrix4 ViewMatrix => Matrix4.LookAt(Position, Target, Up);
    public Matrix4 ProjectionMatrix => Matrix4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);
    
    public void AutoReset(ref PerspectiveCameraComponent component)
    {
        component.Position = Vector3.Zero;
        component.Target = Vector3.Zero;
        component.Up = Vector3.UnitY;
        component.FieldOfView = MathHelper.PiOver4;
        component.NearPlane = 0.01f;
        component.FarPlane = 10000f;
        component.AspectRatio = RenderRites.Machine.Window!.ClientSize.X / (float)RenderRites.Machine.Window.ClientSize.Y;
    }
}