using OpenTK.Mathematics;

namespace RenderRitesMachine.Output;

public class OrthographicCamera : Camera
{
    public float Left { get; set; } = -10f;
    public float Right { get; set; } = 10f;
    public float Bottom { get; set; } = -10f;
    public float Top { get; set; } = 10f;
    public float NearPlane { get; set; } = -1f;
    public float FarPlane { get; set; } = 1f;

    public override Matrix4 ProjectionMatrix => 
        Matrix4.CreateOrthographicOffCenter(Left, Right, Bottom, Top, NearPlane, FarPlane);
}