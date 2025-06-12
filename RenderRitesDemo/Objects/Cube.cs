using OpenTK.Mathematics;
using RenderRitesMachine;
using RenderRitesMachine.GraphicsResources.Objects;

namespace RenderRitesDemo.Objects;

public class Cube(string name, Vector3 position) : Object3D(name, position)
{
    protected Vector3 CalculateFaceNormal(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        Vector3 edge1 = v1 - v0;
        Vector3 edge2 = v2 - v0;
        return Vector3.Normalize(Vector3.Cross(edge1, edge2));
    }

    protected List<Vector3> ComputeFlatNormals(float[] vertices, uint[] indices)
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
    
    protected List<Vector3> ComputeSmoothNormals(float[] vertices, uint[] indices)
    {
        Vector3[] normals = new Vector3[vertices.Length / 5];
    
        for (int i = 0; i < indices.Length; i += 3)
        {
            uint i0 = indices[i];
            uint i1 = indices[i + 1];
            uint i2 = indices[i + 2];
        
            Vector3 v0 = new(vertices[i0 * 5], vertices[i0 * 5 + 1], vertices[i0 * 5 + 2]);
            Vector3 v1 = new(vertices[i1 * 5], vertices[i1 * 5 + 1], vertices[i1 * 5 + 2]);
            Vector3 v2 = new(vertices[i2 * 5], vertices[i2 * 5 + 1], vertices[i2 * 5 + 2]);
        
            Vector3 normal = CalculateFaceNormal(v0, v1, v2);
        
            // Добавляем нормаль к каждой вершине треугольника
            normals[i0] += normal;
            normals[i1] += normal;
            normals[i2] += normal;
        }
    
        // Нормализуем
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.Normalize(normals[i]);
        }
    
        return normals.ToList();
    }
    
    protected override float[] GetVertices()
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
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
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
        var normals = ComputeSmoothNormals(vertices, GetIndices());
        
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

    protected override uint[] GetIndices()
    {
        return [
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
    }

    protected override void Loaded()
    {
        
    }

    public override void Update(double deltaTime)
    {
        
    }

    public override void Resize(int width, int height)
    {
        
    }
}