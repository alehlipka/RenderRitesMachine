using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace RenderRitesMachine.GraphicsResources.Objects;

public abstract class Object3D(string name, Vector3 position) : Resource(name)
{
    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            _translationMatrix = Matrix4.CreateTranslation(_position);
        }
    }

    public Vector3 Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            _scaleMatrix = Matrix4.CreateScale(_scale);
        }
    }
    
    public readonly RotationInfo Rotation = new();

    public Matrix4 ModelMatrix => _scaleMatrix * Rotation.Matrix * _translationMatrix;

    private float[] _vertices = [];
    private uint[] _indices = [];
    private int _vao;
    private int _vbo;
    private int _ebo;

    private Vector3 _position = position;
    private Vector3 _scale = Vector3.One;
    private Matrix4 _scaleMatrix;
    private Matrix4 _translationMatrix;

    protected abstract float[] GetVertices();
    protected abstract uint[] GetIndices();

    protected override void Load()
    {
        _vertices = GetVertices();
        _indices = GetIndices();

        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();
        _vao = GL.GenVertexArray();

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
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
        GL.DeleteBuffer(_vao);
        GL.DeleteBuffer(_ebo);

        Position = _position;
        Scale = _scale;
        
        Loaded();
    }

    protected abstract void Loaded();
    
    public abstract void Update(double deltaTime);
    public abstract void Resize(int width, int height);

    public virtual void Render(double deltaTime)
    {
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    protected override void Unload()
    {
        GL.BindVertexArray(0);
        GL.DeleteVertexArray(_vao);
    }
}