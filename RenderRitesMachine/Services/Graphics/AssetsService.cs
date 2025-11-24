using Assimp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RenderRitesMachine.Assets;
using RenderRitesMachine.Configuration;
using RenderRitesMachine.Debug;
using RenderRitesMachine.Exceptions;
using RenderRitesMachine.Output;
using StbImageSharp;
using BufferTarget = OpenTK.Graphics.OpenGL4.BufferTarget;
using BufferUsageHint = OpenTK.Graphics.OpenGL4.BufferUsageHint;
using GenerateMipmapTarget = OpenTK.Graphics.OpenGL4.GenerateMipmapTarget;
using GetProgramParameterName = OpenTK.Graphics.OpenGL4.GetProgramParameterName;
using GL = OpenTK.Graphics.OpenGL4.GL;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using PixelInternalFormat = OpenTK.Graphics.OpenGL4.PixelInternalFormat;
using PixelType = OpenTK.Graphics.OpenGL4.PixelType;
using Scene = Assimp.Scene;
using ShaderType = OpenTK.Graphics.OpenGL4.ShaderType;
using TextureMagFilter = OpenTK.Graphics.OpenGL4.TextureMagFilter;
using TextureMinFilter = OpenTK.Graphics.OpenGL4.TextureMinFilter;
using TextureParameterName = OpenTK.Graphics.OpenGL4.TextureParameterName;
using TextureTarget = OpenTK.Graphics.OpenGL4.TextureTarget;
using TextureType = RenderRitesMachine.Assets.TextureType;
using TextureWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode;
using VertexAttribPointerType = OpenTK.Graphics.OpenGL4.VertexAttribPointerType;

namespace RenderRitesMachine.Services;

/// <summary>
/// Сервис для управления ресурсами (меши, шейдеры, текстуры, bounding boxes).
/// Предоставляет методы для загрузки и получения ресурсов OpenGL.
/// </summary>
public class AssetsService : IAssetsService
{
    private readonly Dictionary<string, MeshAsset> _meshes = [];
    private readonly Dictionary<string, ShaderAsset> _shaders = [];
    private readonly Dictionary<string, TextureAsset> _textures = [];
    private readonly Dictionary<string, BoundingBoxAsset> _boundingBoxes = [];
    private readonly ILogger? _logger;
    private bool _disposed;

    /// <summary>
    /// Создает новый экземпляр сервиса управления ресурсами.
    /// </summary>
    /// <param name="logger">Логгер для записи сообщений или null, чтобы отключить журналирование.</param>
    public AssetsService(ILogger? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Получает меш по имени.
    /// </summary>
    /// <param name="name">Имя меша.</param>
    /// <returns>Меш с указанным именем.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается, если name равен null.</exception>
    /// <exception cref="KeyNotFoundException">Выбрасывается, если меш с указанным именем не найден.</exception>
    public MeshAsset GetMesh(string name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentNullException(nameof(name), "Mesh name cannot be null or empty.")
            : _meshes.TryGetValue(name, out MeshAsset? value) && value != null
            ? value
            : throw new KeyNotFoundException($"No mesh found with the name: {name}");
    }

    /// <summary>
    /// Получает шейдер по имени.
    /// </summary>
    /// <param name="name">Имя шейдера.</param>
    /// <returns>Шейдер с указанным именем.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается, если name равен null.</exception>
    /// <exception cref="KeyNotFoundException">Выбрасывается, если шейдер с указанным именем не найден.</exception>
    public ShaderAsset GetShader(string name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentNullException(nameof(name), "Shader name cannot be null or empty.")
            : _shaders.TryGetValue(name, out ShaderAsset? value) && value != null
            ? value
            : throw new KeyNotFoundException($"No shader found with the name: {name}");
    }

    /// <summary>
    /// Получает коллекцию всех загруженных шейдеров.
    /// </summary>
    /// <returns>Неизменяемая коллекция всех шейдеров.</returns>
    public IReadOnlyCollection<ShaderAsset> GetAllShaders()
    {
        return _shaders.Values;
    }

    /// <summary>
    /// Получает текстуру по имени.
    /// </summary>
    /// <param name="name">Имя текстуры.</param>
    /// <returns>Текстура с указанным именем.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается, если name равен null.</exception>
    /// <exception cref="KeyNotFoundException">Выбрасывается, если текстура с указанным именем не найдена.</exception>
    public TextureAsset GetTexture(string name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentNullException(nameof(name), "Texture name cannot be null or empty.")
            : _textures.TryGetValue(name, out TextureAsset? value) && value != null
            ? value
            : throw new KeyNotFoundException($"No texture found with the name: {name}");
    }

    /// <summary>
    /// Получает bounding box по имени меша.
    /// </summary>
    /// <param name="name">Имя меша, для которого создан bounding box.</param>
    /// <returns>Bounding box для указанного меша.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается, если name равен null.</exception>
    /// <exception cref="KeyNotFoundException">Выбрасывается, если bounding box с указанным именем не найден.</exception>
    public BoundingBoxAsset GetBoundingBox(string name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentNullException(nameof(name), "Bounding box name cannot be null or empty.")
            : _boundingBoxes.TryGetValue(name, out BoundingBoxAsset? value) && value != null
            ? value
            : throw new KeyNotFoundException($"No bounding box found with the name: {name}");
    }

    /// <summary>
    /// Создает и добавляет bounding box для указанного меша.
    /// </summary>
    /// <param name="meshName">Имя меша, для которого создается bounding box.</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если meshName равен null.</exception>
    /// <exception cref="KeyNotFoundException">Выбрасывается, если меш с указанным именем не найден.</exception>
    /// <exception cref="DuplicateResourceException">Выбрасывается при попытке добавить bounding box с уже существующим именем.</exception>
    public void AddBoundingBox(string meshName)
    {
        if (string.IsNullOrWhiteSpace(meshName))
        {
            throw new ArgumentNullException(nameof(meshName), "Mesh name cannot be null or empty.");
        }

        if (_boundingBoxes.ContainsKey(meshName))
        {
            throw new DuplicateResourceException("bounding box", meshName);
        }

        MeshAsset meshAsset = GetMesh(meshName);

        float[] vertices =
        [
            meshAsset.Minimum.X, meshAsset.Minimum.Y, meshAsset.Minimum.Z,
            meshAsset.Maximum.X, meshAsset.Minimum.Y, meshAsset.Minimum.Z,
            meshAsset.Maximum.X, meshAsset.Maximum.Y, meshAsset.Minimum.Z,
            meshAsset.Minimum.X, meshAsset.Maximum.Y, meshAsset.Minimum.Z,
            meshAsset.Minimum.X, meshAsset.Minimum.Y, meshAsset.Maximum.Z,
            meshAsset.Maximum.X, meshAsset.Minimum.Y, meshAsset.Maximum.Z,
            meshAsset.Maximum.X, meshAsset.Maximum.Y, meshAsset.Maximum.Z,
            meshAsset.Minimum.X, meshAsset.Maximum.Y, meshAsset.Maximum.Z
        ];

        uint[] indices =
        [
            0, 1, 1, 2, 2, 3, 3, 0,
            4, 5, 5, 6, 6, 7, 7, 4,
            0, 4, 1, 5, 2, 6, 3, 7
        ];

        (int vao, int vbo, int ebo) = GetPositionVao([.. vertices], [.. indices]);
        BoundingBoxAsset asset = new()
        {
            Vao = vao,
            Vbo = vbo,
            Ebo = ebo,
            IndicesCount = indices.Length
        };

        _boundingBoxes.Add(meshName, asset);
        _logger?.LogDebug($"Bounding box created for mesh '{meshName}'");
    }

    /// <summary>
    /// Загружает и добавляет текстуру из файла.
    /// </summary>
    /// <param name="name">Имя текстуры для последующего доступа.</param>
    /// <param name="type">Тип текстуры.</param>
    /// <param name="path">Путь к файлу текстуры.</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если name или path равны null.</exception>
    /// <exception cref="FileNotFoundException">Выбрасывается, если файл не найден.</exception>
    /// <exception cref="InvalidDataException">Выбрасывается, если не удалось загрузить изображение.</exception>
    /// <exception cref="IOException">Выбрасывается при ошибке чтения файла.</exception>
    /// <exception cref="DuplicateResourceException">Выбрасывается при попытке добавить текстуру с уже существующим именем.</exception>
    public void AddTexture(string name, TextureType type, string path)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "Texture name cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException(nameof(path), "Texture path cannot be null or empty.");
        }

        if (_textures.ContainsKey(name))
        {
            _logger?.LogWarning($"Attempted to add duplicate texture '{name}'");
            throw new DuplicateResourceException("texture", name);
        }

        if (!File.Exists(path))
        {
            _logger?.LogError($"Texture file not found: {path}");
            throw new FileNotFoundException("Texture file not found.", path);
        }

        _logger?.LogDebug($"Loading texture '{name}' from '{path}'");
        try
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
            using FileStream stream = File.OpenRead(path);
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            if (image == null)
            {
                _logger?.LogError($"Failed to load texture image from {path}");
                throw new InvalidDataException($"Failed to load texture image from {path}.");
            }

            int handle = GL.GenTexture();
            GlDebugWatchdog.CheckGLError("texture generation");

            GL.BindTexture(TextureTarget.Texture2D, handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                image.Width, image.Height, 0, PixelFormat.Rgba,
                PixelType.UnsignedByte, image.Data);
            GlDebugWatchdog.CheckGLError("texture data upload");

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, RenderConstants.AnisotropicFilteringLevel);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GlDebugWatchdog.CheckGLError("texture mipmap generation");

            GL.BindTexture(TextureTarget.Texture2D, 0);

            TextureAsset asset = new() { Id = handle, Type = type };

            _textures.Add(name, asset);
            _logger?.LogInfo($"Texture '{name}' loaded successfully ({image.Width}x{image.Height})");
        }
        catch (Exception ex) when (ex is not FileNotFoundException and not InvalidDataException)
        {
            _logger?.LogException(LogLevel.Error, ex, $"Error reading texture file: {path}");
            throw new IOException($"Error reading texture file: {path}", ex);
        }
    }

    /// <summary>
    /// Загружает и компилирует шейдерную программу из файлов vertex.glsl и fragment.glsl.
    /// </summary>
    /// <param name="name">Имя шейдера для последующего доступа.</param>
    /// <param name="path">Путь к директории, содержащей vertex.glsl и fragment.glsl.</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если name или path равны null.</exception>
    /// <exception cref="FileNotFoundException">Выбрасывается, если файлы шейдеров не найдены.</exception>
    /// <exception cref="ShaderCompilationException">Выбрасывается при ошибке компиляции шейдера.</exception>
    /// <exception cref="ShaderLinkingException">Выбрасывается при ошибке линковки шейдерной программы.</exception>
    /// <exception cref="DuplicateResourceException">Выбрасывается при попытке добавить шейдер с уже существующим именем.</exception>
    public void AddShader(string name, string path)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "Shader name cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException(nameof(path), "Shader path cannot be null or empty.");
        }

        if (_shaders.ContainsKey(name))
        {
            _logger?.LogWarning($"Attempted to add duplicate shader '{name}'");
            throw new DuplicateResourceException("shader", name);
        }

        string vertexShaderPath = Path.Combine(path, "vertex.glsl");
        string fragmentShaderPath = Path.Combine(path, "fragment.glsl");

        if (!File.Exists(vertexShaderPath))
        {
            _logger?.LogError($"Vertex shader file not found: {vertexShaderPath}");
            throw new FileNotFoundException("Vertex shader file not found.", vertexShaderPath);
        }

        if (!File.Exists(fragmentShaderPath))
        {
            _logger?.LogError($"Fragment shader file not found: {fragmentShaderPath}");
            throw new FileNotFoundException("Fragment shader file not found.", fragmentShaderPath);
        }

        _logger?.LogDebug($"Loading shader '{name}' from '{path}'");
        Shader vertexShader = new(vertexShaderPath, ShaderType.VertexShader);
        Shader fragmentShader = new(fragmentShaderPath, ShaderType.FragmentShader);

        try
        {
            vertexShader.Create();
            fragmentShader.Create();
            _logger?.LogDebug($"Shader '{name}': vertex and fragment shaders compiled successfully");

            int handle = GL.CreateProgram();

            GL.AttachShader(handle, vertexShader.Handle);
            GL.AttachShader(handle, fragmentShader.Handle);

            GL.LinkProgram(handle);

            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int linked);
            if (linked != 1)
            {
                string infoLog = GL.GetProgramInfoLog(handle);
                GL.DeleteProgram(handle);
                _logger?.LogError($"Shader '{name}' linking failed: {infoLog}");
                throw new ShaderLinkingException(name, infoLog);
            }

            GL.DetachShader(handle, vertexShader.Handle);
            GL.DetachShader(handle, fragmentShader.Handle);

            vertexShader.Delete();
            fragmentShader.Delete();

            ShaderAsset shader = new() { Id = handle };

            _shaders.Add(name, shader);
            _logger?.LogInfo($"Shader '{name}' loaded and linked successfully");
        }
        catch (ShaderCompilationException ex)
        {
            _logger?.LogException(LogLevel.Error, ex, $"Shader '{name}' compilation failed");
            throw;
        }
        catch (ShaderLinkingException ex)
        {
            _logger?.LogException(LogLevel.Error, ex, $"Shader '{name}' linking failed");
            throw;
        }
    }

    /// <summary>
    /// Загружает меш из файла 3D-модели (поддерживаются форматы, поддерживаемые Assimp).
    /// </summary>
    /// <param name="name">Имя меша для последующего доступа.</param>
    /// <param name="path">Путь к файлу 3D-модели.</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если name или path равны null.</exception>
    /// <exception cref="FileNotFoundException">Выбрасывается, если файл не найден.</exception>
    /// <exception cref="InvalidOperationException">Выбрасывается при ошибке загрузки модели или отсутствии мешей в файле.</exception>
    /// <exception cref="DuplicateResourceException">Выбрасывается при попытке добавить меш с уже существующим именем.</exception>
    public void AddMeshFromFile(string name, string path)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "Mesh name cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException(nameof(path), "Mesh path cannot be null or empty.");
        }

        if (_meshes.ContainsKey(name))
        {
            _logger?.LogWarning($"Attempted to add duplicate mesh '{name}'");
            throw new DuplicateResourceException("mesh", name);
        }

        if (!File.Exists(path))
        {
            _logger?.LogError($"Mesh file not found: {path}");
            throw new FileNotFoundException("Mesh file not found.", path);
        }

        _logger?.LogDebug($"Loading mesh '{name}' from '{path}'");
        AssimpContext importer = new();
        Scene? scene = importer.ImportFile(path,
            PostProcessSteps.Triangulate |
            PostProcessSteps.GenerateSmoothNormals |
            PostProcessSteps.GenerateBoundingBoxes
        );

        if (scene == null || scene.MeshCount == 0)
        {
            _logger?.LogError($"Failed to load model from {path} or model contains no meshes");
            throw new InvalidOperationException($"Failed to load model from {path} or model contains no meshes.");
        }

        Mesh mesh = scene.Meshes[0];

        List<float> floatVertices = [];
        List<System.Numerics.Vector3>? textures = mesh.TextureCoordinateChannels[0];

        for (int i = 0; i < mesh.VertexCount; i++)
        {
            Vector3 position = new(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z);
            Vector3 normal = new(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);
            Vector2 texture = Vector2.Zero;
            if (mesh.HasTextureCoords(0))
            {
                texture = new Vector2(textures[i].X, textures[i].Y);
            }

            float[] vertexInfoIndexedArray =
            [
                position.X, position.Y, position.Z,
                normal.X, normal.Y, normal.Z,
                texture.X, texture.Y
            ];

            floatVertices.AddRange(vertexInfoIndexedArray);
        }

        uint[] indices = mesh.GetUnsignedIndices().ToArray();
        float[] vertices = [.. floatVertices];

        BoundingBox aabb = mesh.BoundingBox;
        Vector3 min = new(aabb.Min.X, aabb.Min.Y, aabb.Min.Z);
        Vector3 max = new(aabb.Max.X, aabb.Max.Y, aabb.Max.Z);

        (int vao, int vbo, int ebo) = GetPositionNormalTextureVao(vertices, indices);
        MeshAsset asset = new()
        {
            Vao = vao,
            Vbo = vbo,
            Ebo = ebo,
            IndicesCount = indices.Length,
            Minimum = min,
            Maximum = max
        };

        _meshes.Add(name, asset);
        _logger?.LogInfo($"Mesh '{name}' loaded successfully ({indices.Length} indices, {mesh.VertexCount} vertices)");
    }

    /// <summary>
    /// Создает и добавляет сферический меш.
    /// </summary>
    /// <param name="name">Имя меша для последующего доступа.</param>
    /// <param name="radius">Радиус сферы.</param>
    /// <param name="sectors">Количество секторов (сегментов по горизонтали).</param>
    /// <param name="stacks">Количество стеков (сегментов по вертикали).</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если name равен null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если radius, sectors или stacks имеют недопустимые значения.</exception>
    /// <exception cref="DuplicateResourceException">Выбрасывается при попытке добавить меш с уже существующим именем.</exception>
    public void AddSphere(string name, float radius, int sectors, int stacks)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "Mesh name cannot be null or empty.");
        }

        if (_meshes.ContainsKey(name))
        {
            _logger?.LogWarning($"Attempted to add duplicate mesh '{name}'");
            throw new DuplicateResourceException("mesh", name);
        }

        if (radius <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(radius), radius, "Radius must be greater than 0.");
        }

        if (sectors < 3)
        {
            throw new ArgumentOutOfRangeException(nameof(sectors), sectors, "Sectors must be at least 3.");
        }

        if (stacks < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(stacks), stacks, "Stacks must be at least 2.");
        }

        _logger?.LogDebug($"Creating sphere mesh '{name}' (radius: {radius}, sectors: {sectors}, stacks: {stacks})");

        List<float> vertices = [];
        List<uint> indices = [];
        Vector3 min = new(-radius);
        Vector3 max = new(radius);

        for (int i = 0; i <= stacks; ++i)
        {
            double stackAngle = (Math.PI / 2) - (i * Math.PI / stacks);
            double xy = radius * Math.Cos(stackAngle);
            double z = radius * Math.Sin(stackAngle);

            for (int j = 0; j <= sectors; ++j)
            {
                double sectorAngle = j * 2 * Math.PI / sectors;

                double x = xy * Math.Cos(sectorAngle);
                double y = xy * Math.Sin(sectorAngle);

                float nx = (float)(x / radius);
                float ny = (float)(y / radius);
                float nz = (float)(z / radius);

                float s = (float)j / sectors;
                float t = 1.0f - ((float)i / stacks);

                vertices.Add((float)x);
                vertices.Add((float)y);
                vertices.Add((float)z);
                vertices.Add(nx);
                vertices.Add(ny);
                vertices.Add(nz);
                vertices.Add(s);
                vertices.Add(t);
            }
        }

        for (uint i = 0; i < stacks; ++i)
        {
            uint k1 = i * ((uint)sectors + 1);
            uint k2 = k1 + (uint)sectors + 1;

            for (int j = 0; j < sectors; ++j, ++k1, ++k2)
            {
                if (i != 0)
                {
                    indices.Add(k1);
                    indices.Add(k2);
                    indices.Add(k1 + 1);
                }

                if (i == stacks - 1)
                {
                    continue;
                }

                indices.Add(k1 + 1);
                indices.Add(k2);
                indices.Add(k2 + 1);
            }
        }

        (int vao, int vbo, int ebo) = GetPositionNormalTextureVao([.. vertices], [.. indices]);
        MeshAsset asset = new()
        {
            Vao = vao,
            Vbo = vbo,
            Ebo = ebo,
            IndicesCount = indices.Count,
            Minimum = min,
            Maximum = max
        };

        _meshes.Add(name, asset);
        _logger?.LogInfo($"Sphere mesh '{name}' created successfully ({indices.Count} indices)");
    }

    private static (int vao, int vbo, int ebo) GetPositionNormalTextureVao(float[] vertices, uint[] indices)
    {
        int vao = GL.GenVertexArray();
        int vbo = GL.GenBuffer();
        int ebo = GL.GenBuffer();
        GlDebugWatchdog.CheckGLError("VAO/VBO/EBO generation");

        GL.BindVertexArray(vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        GlDebugWatchdog.CheckGLError("VBO data upload");

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        GlDebugWatchdog.CheckGLError("EBO data upload");

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, RenderConstants.VertexAttributeSize * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, RenderConstants.VertexAttributeSize * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, RenderConstants.VertexAttributeSize * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        return (vao, vbo, ebo);
    }

    private static (int vao, int vbo, int ebo) GetPositionVao(float[] vertices, uint[] indices)
    {
        int vbo = GL.GenBuffer();
        int ebo = GL.GenBuffer();
        int vao = GL.GenVertexArray();
        GlDebugWatchdog.CheckGLError("VAO/VBO/EBO generation");

        GL.BindVertexArray(vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);
        GlDebugWatchdog.CheckGLError("VBO data upload");

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        GlDebugWatchdog.CheckGLError("EBO data upload");

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, RenderConstants.PositionAttributeSize * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        return (vao, vbo, ebo);
    }

    /// <summary>
    /// Освобождает все загруженные ресурсы OpenGL.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            foreach (MeshAsset mesh in _meshes.Values)
            {
                mesh?.Dispose();
            }

            foreach (ShaderAsset shader in _shaders.Values)
            {
                shader?.Dispose();
            }

            foreach (TextureAsset texture in _textures.Values)
            {
                texture?.Dispose();
            }

            foreach (BoundingBoxAsset boundingBox in _boundingBoxes.Values)
            {
                boundingBox?.Dispose();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogException(LogLevel.Error, ex, "Error disposing assets");
        }
        finally
        {
            _meshes.Clear();
            _shaders.Clear();
            _textures.Clear();
            _boundingBoxes.Clear();
            _disposed = true;
        }
    }
}
