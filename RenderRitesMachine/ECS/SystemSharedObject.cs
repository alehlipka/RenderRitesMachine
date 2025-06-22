using RenderRitesMachine.Output;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.ECS;

public class SystemSharedObject(PerspectiveCamera camera, TimeService time)
{
    public PerspectiveCamera Camera = camera;
    public TimeService Time = time;
}
