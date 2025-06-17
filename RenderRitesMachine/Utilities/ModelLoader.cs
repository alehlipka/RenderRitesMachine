using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.ECS.Components;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace RenderRitesMachine.Utilities;

public static class ModelLoader
{
    public static MeshComponent CreateSphere(float radius, int sectors, int stacks)
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
        
        int vao = BindBuffers(vertices.ToArray(), indices.ToArray());
        
        return new MeshComponent
        {
            Vao = vao,
            PrimitiveType = PrimitiveType.Triangles,
            Count = indices.Count,
            DrawElementsType = DrawElementsType.UnsignedInt,
            IndicesStoreLocation = 0,
            Minimum = min,
            Maximum = max
        };
    }
    
    public static IEnumerable<MeshComponent> Load(string path)
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

        foreach (Mesh mesh in scene.Meshes)
        {
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
            
            int vao = BindBuffers(vertices, indices);
            
            BoundingBox aabb = mesh.BoundingBox;
            Vector3 min = new(aabb.Min.X, aabb.Min.Y, aabb.Min.Z);
            Vector3 max = new(aabb.Max.X, aabb.Max.Y, aabb.Max.Z);
        
            yield return new MeshComponent
            {
                Vao = vao,
                PrimitiveType = PrimitiveType.Triangles,
                Count = indices.Length,
                DrawElementsType = DrawElementsType.UnsignedInt,
                IndicesStoreLocation = 0,
                Minimum = min,
                Maximum = max
            };
        }
    }

    private static int BindBuffers(float[] vertices, uint[] indices)
    {
        int vbo = GL.GenBuffer();
        int ebo = GL.GenBuffer();
        int vao = GL.GenVertexArray();

        GL.BindVertexArray(vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices,
            BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices,
            BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo);

        return vao;
    }
}