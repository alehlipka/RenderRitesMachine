using Leopotam.EcsLite;

namespace RenderRitesMachine.ECS.Systems;

/// <summary>
/// Система для автоматического обновления позиции и ориентации слушателя (игрока/камеры) в аудио системе.
/// Обновляет позицию слушателя на основе позиции камеры каждый кадр.
/// </summary>
public class AudioListenerUpdateSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();

        shared.Audio.SetListenerPosition(shared.Camera.Position);

        shared.Audio.SetListenerOrientation(shared.Camera.Front, shared.Camera.Up);
    }
}

