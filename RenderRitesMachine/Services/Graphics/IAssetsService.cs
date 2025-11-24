using RenderRitesMachine.Assets;

namespace RenderRitesMachine.Services;

/// <summary>
/// Service interface for managing meshes, shaders, textures, and bounding boxes.
/// </summary>
public interface IAssetsService : IDisposable
{
    /// <summary>
    /// Returns a mesh by name.
    /// </summary>
    MeshAsset GetMesh(string name);

    /// <summary>
    /// Returns a shader by name.
    /// </summary>
    ShaderAsset GetShader(string name);

    /// <summary>
    /// Returns all loaded shaders.
    /// </summary>
    IReadOnlyCollection<ShaderAsset> GetAllShaders();

    /// <summary>
    /// Returns a texture by name.
    /// </summary>
    TextureAsset GetTexture(string name);

    /// <summary>
    /// Returns a bounding box by mesh name.
    /// </summary>
    BoundingBoxAsset GetBoundingBox(string name);

    /// <summary>
    /// Creates and adds a bounding box for the specified mesh.
    /// </summary>
    void AddBoundingBox(string meshName);

    /// <summary>
    /// Loads and registers a texture from disk.
    /// </summary>
    void AddTexture(string name, TextureType type, string path);

    /// <summary>
    /// Loads and compiles a shader program from vertex/fragment files.
    /// </summary>
    void AddShader(string name, string path);

    /// <summary>
    /// Loads a mesh from a 3D model file.
    /// </summary>
    void AddMeshFromFile(string name, string path);

    /// <summary>
    /// Creates and adds a procedural sphere mesh.
    /// </summary>
    void AddSphere(string name, float radius, int sectors, int stacks);
}
