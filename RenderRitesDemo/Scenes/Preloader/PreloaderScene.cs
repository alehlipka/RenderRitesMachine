using OpenTK.Mathematics;
using RenderRitesDemo.ECS;
using RenderRitesMachine;
using RenderRitesMachine.Output;

namespace RenderRitesDemo.Scenes.Preloader;

public class PreloaderScene(string name) : Scene(name)
{
    protected override void OnLoad()
    {
        RenderRites.Machine.Scenes.ForEach(item => item.Initialize());

        int sphere = World.NewEntity();

        ref TransformComponent transform = ref World.GetPool<TransformComponent>().Add(sphere);
        transform.Position = new Vector3(0.0f, 0.0f, -5.0f);

        ref MeshComponent mesh = ref World.GetPool<MeshComponent>().Add(sphere);
        mesh.Name = "sphere";

        
    }
}
