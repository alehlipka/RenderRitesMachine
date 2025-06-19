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
        var positions = World.GetPool<TransformComponent>();

        positions.Add(sphere) = new TransformComponent { Position = Vector3.Zero };
    }
}