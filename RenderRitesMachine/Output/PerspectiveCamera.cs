using OpenTK.Mathematics;

namespace RenderRitesMachine.Output;

public class PerspectiveCamera
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
    
    public PerspectiveCamera()
    {
        Position = Vector3.Zero;
        Target = Vector3.Zero;
        Up = Vector3.UnitY;
        FieldOfView = MathHelper.PiOver4;
        NearPlane = 0.01f;
        FarPlane = 10000f;
        AspectRatio = 1.0f;
    }
}