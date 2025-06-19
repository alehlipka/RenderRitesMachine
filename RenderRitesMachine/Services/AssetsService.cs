using RenderRitesMachine.Assets;

namespace RenderRitesMachine.Services;

public class AssetsService : IDisposable
{
    public MeshAsset GetMesh(string name)
    {
        return new MeshAsset();
    }

    public void Dispose()
    {
        
    }
}
