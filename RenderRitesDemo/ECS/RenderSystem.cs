using Leopotam.EcsLite;
using OpenTK.Mathematics;
using RenderRitesMachine;
using RenderRitesMachine.Assets;

namespace RenderRitesDemo.ECS;

public class RenderSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        EcsFilter filter = world.Filter<TransformComponent>().Inc<MeshComponent>().End();

        var transforms = world.GetPool<TransformComponent>();
        var meshes = world.GetPool<MeshComponent>();

        foreach (int entity in filter)
        {
            ref TransformComponent transform = ref transforms.Get(entity);
            ref MeshComponent mesh = ref meshes.Get(entity);

            if (mesh.IsVisible)
            {
                Matrix4 meshModelMatrix =
                    Matrix4.CreateScale(transform.Scale) *
                    Matrix4.CreateFromQuaternion(transform.Quaternion) *
                    Matrix4.CreateTranslation(transform.Position);

                MeshAsset meshAsset = RenderRites.Machine.Assets.GetMesh(mesh.Name);
                RenderRites.Machine.Renderer.Render(meshAsset, meshModelMatrix);
            }
        }
    }
}
