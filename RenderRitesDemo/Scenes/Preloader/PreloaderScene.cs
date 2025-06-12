using OpenTK.Mathematics;
using RenderRitesDemo.Scenes.Preloader.ECS.Components;
using RenderRitesMachine;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Systems;
using RenderRitesMachine.Output;
using RenderRitesMachine.Utilities;

namespace RenderRitesDemo.Scenes.Preloader;

public class PreloaderScene(string name) : Scene(name)
{
    protected override void Load()
    {
        RenderRites.Machine.Scenes.ForEach(item => item.Initialize());
        Random r = new();
        
        TextureComponent texture = new(Path.Combine("Assets", "Textures", "debug.jpg"));
        PerspectiveCameraComponent cam = new()
        {
            Position = new Vector3(0, 0, 20)
        };
        ShaderComponent shader = new(Path.Combine("Assets", "Shaders", "Default"));
        MeshComponent cubeMesh = CubeMeshComponentFactory.CreateCube();
        
        World.AddSystem(new InstancedRenderSystem());

        const int objectsCount = 100_000;
        const float sphereRadius = 5.0f;
        const float minDistance = 0.5f;
        List<Vector3> occupiedPositions = [];
        
        for (int i = 0; i < objectsCount; i++)
        {
            Vector3 position;
            int attempts = 0;
            do 
            {
                position = Vector3.Normalize(new Vector3(
                    (float)(r.NextDouble() * 2 - 1), 
                    (float)(r.NextDouble() * 2 - 1), 
                    (float)(r.NextDouble() * 2 - 1)
                )) * sphereRadius;

                attempts++;
                if (attempts > 100) break;
            } 
            while (occupiedPositions.Any(p => Vector3.Distance(p, position) < minDistance));

            occupiedPositions.Add(position);

            Entity test = World.CreateEntity();
            World.AddComponent(test, texture);
            World.AddComponent(test, cam);
            World.AddComponent(test, shader);
            World.AddComponent(test, new TransformComponent(
                position: position,
                rotation: new RotationInfo { Axis = new Vector3(r.NextSingle(), r.NextSingle(), r.NextSingle()), Angle = r.NextSingle() },
                scale: new Vector3(r.NextSingle(), r.NextSingle(), r.NextSingle())
            ));
            World.AddComponent(test, cubeMesh);
        }
    }

    protected override void Unload()
    {
        World.Dispose();
    }
}