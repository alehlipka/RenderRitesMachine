using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.ECS.Components;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace RenderRitesMachine.Utilities;

public static class ModelLoader
{
    public static IEnumerable<MeshComponent> Load(string path)
    {
        AssimpContext importer = new();
        Scene? scene = importer.ImportFile(path,
            PostProcessSteps.Triangulate |
            PostProcessSteps.GenerateSmoothNormals |
            PostProcessSteps.GenerateUVCoords
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
            GL.DeleteBuffer(vao);
            GL.DeleteBuffer(ebo);
        
            yield return new MeshComponent()
            {
                Vao = vao,
                PrimitiveType = PrimitiveType.Triangles,
                Count = indices.Length,
                DrawElementsType = DrawElementsType.UnsignedInt,
                IndicesStoreLocation = 0
            };
        }
    }
}