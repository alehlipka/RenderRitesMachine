using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesDemo.Scenes.Preloader.ECS.Components;

public static class CubeMeshComponentFactory
{
    public static MeshComponent CreateCube()
    {
        uint[] indices = [
            0, 1, 2,
            2, 3, 0,
            5, 4, 7,
            7, 6, 5,
            8, 9, 10,
            10, 11, 8,
            12, 15, 14,
            14, 13, 12,
            16, 17, 18,
            18, 19, 16,
            20, 23, 22,
            22, 21, 20
        ];
        
        float[] vertices = GetVertices(indices);

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
        
        return new MeshComponent()
        {
            Vao = vao,
            PrimitiveType = PrimitiveType.Triangles,
            Count = indices.Length,
            DrawElementsType = DrawElementsType.UnsignedInt,
            IndicesStoreLocation = 0
        };
    }
    
    private static float[] GetVertices(uint[] indices)
    {
        float[] vertices =
        [
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            
            -0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            
            -0.5f, -0.5f,  0.5f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
            
             0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            
            -0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 1.0f 
        ];
        
        List<float> vertexDataWithNormals = [];

        // var normals = ComputeFlatNormals(vertices, GetIndices());
        var normals = ComputeSmoothNormals(vertices, indices);
        
        for (int i = 0; i < vertices.Length / 5; i++)
        {
            vertexDataWithNormals.Add(vertices[i * 5]);
            vertexDataWithNormals.Add(vertices[i * 5 + 1]);
            vertexDataWithNormals.Add(vertices[i * 5 + 2]);
    
            Vector3 normal = normals[i];
            vertexDataWithNormals.Add(normal.X);
            vertexDataWithNormals.Add(normal.Y);
            vertexDataWithNormals.Add(normal.Z);
    
            vertexDataWithNormals.Add(vertices[i * 5 + 3]);
            vertexDataWithNormals.Add(vertices[i * 5 + 4]);
        }
        
        return vertexDataWithNormals.ToArray();
    }
    
    private static Vector3 CalculateFaceNormal(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        Vector3 edge1 = v1 - v0;
        Vector3 edge2 = v2 - v0;
        return Vector3.Normalize(Vector3.Cross(edge1, edge2));
    }

    private static List<Vector3> ComputeFlatNormals(float[] vertices, uint[] indices)
    {
        var normals = new List<Vector3>(vertices.Length / 5);
    
        for (int i = 0; i < indices.Length; i += 3)
        {
            uint i0 = indices[i];
            uint i1 = indices[i + 1];
            uint i2 = indices[i + 2];
        
            Vector3 v0 = new(vertices[i0 * 5], vertices[i0 * 5 + 1], vertices[i0 * 5 + 2]);
            Vector3 v1 = new(vertices[i1 * 5], vertices[i1 * 5 + 1], vertices[i1 * 5 + 2]);
            Vector3 v2 = new(vertices[i2 * 5], vertices[i2 * 5 + 1], vertices[i2 * 5 + 2]);
        
            Vector3 normal = CalculateFaceNormal(v0, v1, v2);
            
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
        }
    
        return normals;
    }
    
    private static List<Vector3> ComputeSmoothNormals(float[] vertices, uint[] indices)
    {
        var normals = new Vector3[vertices.Length / 5];
    
        for (int i = 0; i < indices.Length; i += 3)
        {
            uint i0 = indices[i];
            uint i1 = indices[i + 1];
            uint i2 = indices[i + 2];
        
            Vector3 v0 = new(vertices[i0 * 5], vertices[i0 * 5 + 1], vertices[i0 * 5 + 2]);
            Vector3 v1 = new(vertices[i1 * 5], vertices[i1 * 5 + 1], vertices[i1 * 5 + 2]);
            Vector3 v2 = new(vertices[i2 * 5], vertices[i2 * 5 + 1], vertices[i2 * 5 + 2]);
        
            Vector3 normal = CalculateFaceNormal(v0, v1, v2);
        
            normals[i0] += normal;
            normals[i1] += normal;
            normals[i2] += normal;
        }
    
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.Normalize(normals[i]);
        }
    
        return normals.ToList();
    }
}