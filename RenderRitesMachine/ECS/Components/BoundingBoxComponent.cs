using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Utilities;

namespace RenderRitesMachine.ECS.Components;

public class BoundingBoxComponent : IComponent
{
    public Entity Parent;
    public int Vao;
    public PrimitiveType PrimitiveType;
    public int Count;
    public DrawElementsType DrawElementsType;
    public int IndicesStoreLocation;
    public Vector3 OriginalMinimum;
    public Vector3 OriginalMaximum;
    
    public void Transform(Matrix4 modelMatrix, Vector3 originalMin, Vector3 originalMax)
    {
        Vector3[] vertices =
        [
            new(originalMin.X, originalMin.Y, originalMin.Z),
            new(originalMax.X, originalMin.Y, originalMin.Z),
            new(originalMin.X, originalMax.Y, originalMin.Z),
            new(originalMax.X, originalMax.Y, originalMin.Z),
            new(originalMin.X, originalMin.Y, originalMax.Z),
            new(originalMax.X, originalMin.Y, originalMax.Z),
            new(originalMin.X, originalMax.Y, originalMax.Z),
            new(originalMax.X, originalMax.Y, originalMax.Z)
        ];

        var transformedVertices = new Vector3[8];
        for (int i = 0; i < 8; i++)
        {
            transformedVertices[i] = Vector3.TransformPosition(vertices[i], modelMatrix);
        }

        Vector3 newMin = transformedVertices[0];
        Vector3 newMax = transformedVertices[0];

        for (int i = 1; i < 8; i++)
        {
            newMin = Vector3.ComponentMin(newMin, transformedVertices[i]);
            newMax = Vector3.ComponentMax(newMax, transformedVertices[i]);
        }

        BoundingBoxComponent newBox = BoundingBoxCreator.Create(Parent, newMin, newMax);
        
        Vao = newBox.Vao;
        PrimitiveType = newBox.PrimitiveType;
        Count = newBox.Count;
        DrawElementsType = newBox.DrawElementsType;
        IndicesStoreLocation = newBox.IndicesStoreLocation;
    }
    
    public void Dispose()
    {
        GL.BindVertexArray(0);
        GL.DeleteVertexArray(Vao);
    }
}