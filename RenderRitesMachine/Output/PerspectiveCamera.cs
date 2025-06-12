using OpenTK.Mathematics;

namespace RenderRitesMachine.Output;

public class PerspectiveCamera : Camera
{
    public float FieldOfView { get; set; } = MathHelper.PiOver4; // 45°
    public float AspectRatio { get; set; } = RenderRites.Machine.Window!.Size.X / (float)RenderRites.Machine.Window.Size.Y;
    public float NearPlane { get; set; } = 0.01f;
    public float FarPlane { get; set; } = 10000f;

    public override Matrix4 ProjectionMatrix =>
        Matrix4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);
}