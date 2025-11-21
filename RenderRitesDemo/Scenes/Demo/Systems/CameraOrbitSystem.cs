using Leopotam.EcsLite;
using OpenTK.Mathematics;
using RenderRitesMachine.ECS;

namespace RenderRitesDemo.Scenes.Demo.Systems;

internal sealed class CameraOrbitSystem : IEcsRunSystem
{
    private readonly Vector3 _pivot;
    private readonly float _radius;
    private readonly float _height;
    private readonly float _speed;
    private float _angle;

    public CameraOrbitSystem(Vector3 pivot, float radius, float height, float speed)
    {
        if (radius <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(radius), radius, "Orbit radius must be positive.");
        }

        _pivot = pivot;
        _radius = radius;
        _height = height;
        _speed = speed;
    }

    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        float delta = shared.Time.UpdateDeltaTime;

        _angle += delta * _speed;

        Vector3 orbitOffset = new(MathF.Cos(_angle) * _radius, _height, MathF.Sin(_angle) * _radius);
        Vector3 position = _pivot + orbitOffset;

        shared.Camera.Position = position;

        var direction = Vector3.Normalize(_pivot - position);
        float pitch = MathF.Asin(direction.Y);
        float yaw = MathF.Atan2(direction.Z, direction.X);

        shared.Camera.Pitch = MathHelper.RadiansToDegrees(pitch);
        shared.Camera.Yaw = MathHelper.RadiansToDegrees(yaw);

        shared.Audio.SetListenerPosition(position);
        shared.Audio.SetListenerOrientation(shared.Camera.Front, shared.Camera.Up);
    }
}
