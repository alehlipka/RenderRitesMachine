using OpenTK.Mathematics;

namespace RenderRitesDemo.Scenes.Demo.Components;

internal struct FloatingAnimation
{
    public Vector3 BasePosition;
    public float Amplitude;
    public float Frequency;
    public float Phase;
    public float ElapsedTime;
}
