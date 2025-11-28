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
/// Manages meshes, shaders, textures, and bounding boxes, exposing helpers to load and retrieve OpenGL resources.
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
    /// Creates a new instance of the assets service.
    /// </summary>
    /// <param name="logger">Logger for diagnostics, or null to disable logging.</param>
    public AssetsService(ILogger? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a mesh by name.
    /// </summary>
    /// <param name="name">Mesh identifier.</param>
    /// <returns>Mesh asset with the specified name.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the mesh cannot be found.</exception>
    public MeshAsset GetMesh(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "Mesh name cannot be null or empty.");
        }

        if (!_meshes.TryGetValue(name, out MeshAsset? value) || value == null)
        {
            _logger?.LogError($"Mesh '{name}' not found");
            throw new KeyNotFoundException($"No mesh found with the name: {name}");
        }

        return value;
    }

    /// <summary>
    /// Retrieves a shader by name.
    /// </summary>
    /// <param name="name">Shader identifier.</param>
    /// <returns>Shader asset with the specified name.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the shader cannot be found.</exception>
    public ShaderAsset GetShader(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "Shader name cannot be null or empty.");
        }

        if (!_shaders.TryGetValue(name, out ShaderAsset? value) || value == null)
        {
            _logger?.LogError($"Shader '{name}' not found");
            throw new KeyNotFoundException($"No shader found with the name: {name}");
        }

        return value;
    }

    /// <summary>
    /// Returns all loaded shaders.
    /// </summary>
    /// <returns>An immutable collection of shaders.</returns>
    public IReadOnlyCollection<ShaderAsset> GetAllShaders()
    {
        return _shaders.Values;
    }

    /// <summary>
    /// Retrieves a texture by name.
    /// </summary>
    /// <param name="name">Texture identifier.</param>
    /// <returns>Texture asset with the specified name.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the texture cannot be found.</exception>
    public TextureAsset GetTexture(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "Texture name cannot be null or empty.");
        }

        if (!_textures.TryGetValue(name, out TextureAsset? value) || value == null)
        {
            _logger?.LogError($"Texture '{name}' not found");
            throw new KeyNotFoundException($"No texture found with the name: {name}");
        }

        return value;
    }

    /// <summary>
    /// Retrieves a bounding box for the given mesh.
    /// </summary>
    /// <param name="name">Mesh name associated with the bounding box.</param>
    /// <returns>Bounding box asset for the mesh.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the bounding box cannot be found.</exception>
    public BoundingBoxAsset GetBoundingBox(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "Bounding box name cannot be null or empty.");
        }

        if (!_boundingBoxes.TryGetValue(name, out BoundingBoxAsset? value) || value == null)
        {
            _logger?.LogError($"Bounding box '{name}' not found");
            throw new KeyNotFoundException($"No bounding box found with the name: {name}");
        }

        return value;
    }

    /// <summary>
    /// Generates and stores a bounding box for the specified mesh.
    /// </summary>
    /// <param name="meshName">Mesh name used for the bounding box.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="meshName"/> is null or empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the mesh cannot be found.</exception>
    /// <exception cref="DuplicateResourceException">Thrown when the bounding box already exists.</exception>
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
    /// Loads a texture from disk and registers it.
    /// </summary>
    /// <param name="name">Texture identifier used for future lookups.</param>
    /// <param name="type">Texture type.</param>
    /// <param name="path">Path to the texture file.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or <paramref name="path"/> is null or empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file cannot be found.</exception>
    /// <exception cref="InvalidDataException">Thrown when the image data cannot be loaded.</exception>
    /// <exception cref="IOException">Thrown when the file cannot be read.</exception>
    /// <exception cref="DuplicateResourceException">Thrown when a texture with the same name already exists.</exception>
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
    /// Loads and compiles a shader program from <c>vertex.glsl</c> and <c>fragment.glsl</c>.
    /// </summary>
    /// <param name="name">Shader identifier.</param>
    /// <param name="path">Directory containing the shader files.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or <paramref name="path"/> is null or empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown when shader files cannot be found.</exception>
    /// <exception cref="ShaderCompilationException">Thrown when compilation fails.</exception>
    /// <exception cref="ShaderLinkingException">Thrown when linking fails.</exception>
    /// <exception cref="DuplicateResourceException">Thrown when a shader with the same name already exists.</exception>
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
        ShaderProgram shaderProgram = new();

        try
        {
            vertexShader.Create();
            fragmentShader.Create();
            _logger?.LogDebug($"Shader '{name}': vertex and fragment shaders compiled successfully");

            shaderProgram.AttachShader(vertexShader);
            shaderProgram.AttachShader(fragmentShader);
            shaderProgram.Link();
            shaderProgram.DetachShader(vertexShader);
            shaderProgram.DetachShader(fragmentShader);
            vertexShader.Delete();
            fragmentShader.Delete();

            ShaderAsset shader = new() { Id = shaderProgram.Handle };

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
    /// Loads a mesh from a 3D model file (any format supported by Assimp).
    /// </summary>
    /// <param name="name">Mesh identifier.</param>
    /// <param name="path">Path to the model file.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or <paramref name="path"/> is null or empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file cannot be found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when loading fails or the file contains no meshes.</exception>
    /// <exception cref="DuplicateResourceException">Thrown when a mesh with the same name already exists.</exception>
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
    /// Creates and registers a procedural sphere mesh.
    /// </summary>
    /// <param name="name">Mesh identifier.</param>
    /// <param name="radius">Sphere radius.</param>
    /// <param name="sectors">Number of horizontal segments.</param>
    /// <param name="stacks">Number of vertical segments.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when radius, sectors, or stacks are invalid.</exception>
    /// <exception cref="DuplicateResourceException">Thrown when a mesh with the same name already exists.</exception>
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
    /// Releases all loaded OpenGL resources.
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
