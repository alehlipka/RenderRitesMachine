using Assimp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RenderRitesMachine.Assets;
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

public static class AssetsService
{
    private static readonly Dictionary<string, MeshAsset> Meshes = [];
    private static readonly Dictionary<string, ShaderAsset> Shaders = [];
    private static readonly Dictionary<string, TextureAsset> Textures = [];
    private static readonly Dictionary<string, BoundingBoxAsset> BoundingBoxes = [];

    public static MeshAsset GetMesh(string name)
    {
        if (Meshes.TryGetValue(name, out MeshAsset value))
        {
            return value;
        }
        
        throw new KeyNotFoundException($"No mesh found with the name: {name}");
    }

    public static ShaderAsset GetShader(string name)
    {
        if (Shaders.TryGetValue(name, out ShaderAsset value))
        {
            return value;
        }
        
        throw new KeyNotFoundException($"No shader found with the name: {name}");
    }

    public static ShaderAsset[] GetAllShaders()
    {
        return Shaders.Values.ToArray();
    }
    
    public static TextureAsset GetTexture(string name)
    {
        if (Textures.TryGetValue(name, out TextureAsset value))
        {
            return value;
        }
        
        throw new KeyNotFoundException($"No texture found with the name: {name}");
    }
    
    public static BoundingBoxAsset GetBoundingBox(string name)
    {
        if (BoundingBoxes.TryGetValue(name, out BoundingBoxAsset value))
        {
            return value;
        }
        
        throw new KeyNotFoundException($"No bounding box found with the name: {name}");
    }

    public static void AddBoundingBox(string meshName)
    {
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

        BoundingBoxAsset asset = new()
        {
            Vao = GetPositionVao(vertices.ToArray(), indices.ToArray()),
            IndicesCount = indices.Length
        };
        
        BoundingBoxes.Add(meshName, asset);
    }

    public static void AddTexture(string name, TextureType type, string path)
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        ImageResult? image = ImageResult.FromStream(
            File.OpenRead(path),
            ColorComponents.RedGreenBlueAlpha
        );
        if (image == null)
        {
            throw new Exception("Texture resource loading error");
        }
        
        int handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, handle);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
            image.Width, image.Height, 0, PixelFormat.Rgba, 
            PixelType.UnsignedByte, image.Data);
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        
        const float AnisotropicFilteringLevel = 8.0f;
        GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, AnisotropicFilteringLevel);
        
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        TextureAsset asset = new() { Id = handle, Type = type };
        
        Textures.Add(name, asset);
    }

    public static void AddShader(string name, string path)
    {
        string vertexShaderPath = Path.Combine(path, "vertex.glsl");
        string fragmentShaderPath = Path.Combine(path, "fragment.glsl");
        
        Shader vertexShader = new(vertexShaderPath, ShaderType.VertexShader);
        Shader fragmentShader = new(fragmentShaderPath, ShaderType.FragmentShader);

        vertexShader.Create();
        fragmentShader.Create();

        int handle = GL.CreateProgram();

        GL.AttachShader(handle, vertexShader.Handle);
        GL.AttachShader(handle, fragmentShader.Handle);

        GL.LinkProgram(handle);

        GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int linked);
        if (linked != 1)
        {
            string infoLog = GL.GetProgramInfoLog(handle);
            throw new Exception($"Shader link error: {infoLog}");
        }

        GL.DetachShader(handle, vertexShader.Handle);
        GL.DetachShader(handle, fragmentShader.Handle);

        vertexShader.Delete();
        fragmentShader.Delete();

        ShaderAsset shader = new() { Id = handle };

        Shaders.Add(name, shader);
    }
    
    public static void AddMeshFromFile(string name, string path)
    {
        AssimpContext importer = new();
        Scene? scene = importer.ImportFile(path,
            PostProcessSteps.Triangulate |
            PostProcessSteps.GenerateSmoothNormals |
            PostProcessSteps.GenerateBoundingBoxes
        );

        if (scene == null)
        {
            throw new Exception("Failed to load model");
        }

        Mesh mesh = scene.Meshes.First();
        
        List<float> floatVertices = [];
        var textures = mesh.TextureCoordinateChannels[0];

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
        float[] vertices = floatVertices.ToArray();

        BoundingBox aabb = mesh.BoundingBox;
        Vector3 min = new(aabb.Min.X, aabb.Min.Y, aabb.Min.Z);
        Vector3 max = new(aabb.Max.X, aabb.Max.Y, aabb.Max.Z);

        MeshAsset asset = new()
        {
            Vao = GetPositionNormalTextureVao(vertices, indices),
            IndicesCount = indices.Length,
            Minimum = min,
            Maximum = max
        };
        
        Meshes.Add(name, asset);
    }

    public static void AddSphere(string name, float radius, int sectors, int stacks)
    {
        List<float> vertices = [];
        List<uint> indices = [];
        Vector3 min = new(-radius);
        Vector3 max = new(radius);
        
        for (int i = 0; i <= stacks; ++i)
        {
            double stackAngle = Math.PI / 2 - i * Math.PI / stacks;
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
                float t = 1.0f - (float)i / stacks;

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

                if (i == stacks - 1) continue;
                
                indices.Add(k1 + 1);
                indices.Add(k2);
                indices.Add(k2 + 1);
            }
        }
        
        MeshAsset asset = new()
        {
            Vao = GetPositionNormalTextureVao(vertices.ToArray(), indices.ToArray()),
            IndicesCount = indices.Count,
            Minimum = min,
            Maximum = max
        };
        
        Meshes.Add(name, asset);
    }
    
    private static int GetPositionNormalTextureVao(float[] vertices, uint[] indices)
    {
        int vao = GL.GenVertexArray();
        int vbo = GL.GenBuffer();
        int ebo = GL.GenBuffer();
        
        GL.BindVertexArray(vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        
        // Note: VBO and EBO are not deleted here as they are referenced by the VAO
        // They will be cleaned up when the OpenGL context is destroyed

        return vao;
    }
    
    private static int GetPositionVao(float[] vertices, uint[] indices)
    {
        int vbo = GL.GenBuffer();
        int ebo = GL.GenBuffer();
        int vao = GL.GenVertexArray();
        
        GL.BindVertexArray(vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        
        // Note: VBO and EBO are not deleted here as they are referenced by the VAO
        // They will be cleaned up when the OpenGL context is destroyed

        return vao;
    }
}
