using RenderRitesMachine.Assets;

namespace RenderRitesMachine.Services;

/// <summary>
/// Интерфейс для сервиса управления ресурсами (меши, шейдеры, текстуры, bounding boxes).
/// </summary>
public interface IAssetsService : IDisposable
{
    /// <summary>
    /// Получает меш по имени.
    /// </summary>
    MeshAsset GetMesh(string name);

    /// <summary>
    /// Получает шейдер по имени.
    /// </summary>
    ShaderAsset GetShader(string name);

    /// <summary>
    /// Получает коллекцию всех загруженных шейдеров.
    /// </summary>
    IReadOnlyCollection<ShaderAsset> GetAllShaders();

    /// <summary>
    /// Получает текстуру по имени.
    /// </summary>
    TextureAsset GetTexture(string name);

    /// <summary>
    /// Получает bounding box по имени меша.
    /// </summary>
    BoundingBoxAsset GetBoundingBox(string name);

    /// <summary>
    /// Создает и добавляет bounding box для указанного меша.
    /// </summary>
    void AddBoundingBox(string meshName);

    /// <summary>
    /// Загружает и добавляет текстуру из файла.
    /// </summary>
    void AddTexture(string name, TextureType type, string path);

    /// <summary>
    /// Загружает и компилирует шейдерную программу из файлов vertex.glsl и fragment.glsl.
    /// </summary>
    void AddShader(string name, string path);

    /// <summary>
    /// Загружает меш из файла 3D-модели.
    /// </summary>
    void AddMeshFromFile(string name, string path);

    /// <summary>
    /// Создает и добавляет сферический меш.
    /// </summary>
    void AddSphere(string name, float radius, int sectors, int stacks);
}
