using System;
using OpenTK.Graphics.OpenGL4;

namespace PrototypeGraphicsCore;

public sealed class LineMesh : IDisposable
{
    private readonly int _vao;
    private readonly int _vbo;
    private readonly int _capacityVerts;
    private readonly BufferUsageHint _usage;

    public int Count { get; private set; }

    // Вершина: (x,y,z, age) => 4 float
    private const int FloatsPerVertex = 4;
    private const int StrideBytes = FloatsPerVertex * sizeof(float);

    public LineMesh(int capacityVerts, BufferUsageHint usage = BufferUsageHint.DynamicDraw)
    {
        _capacityVerts = Math.Max(2, capacityVerts);
        _usage = usage;

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

        GL.BufferData(BufferTarget.ArrayBuffer,
            _capacityVerts * FloatsPerVertex * sizeof(float),
            IntPtr.Zero,
            _usage);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, StrideBytes, 0);

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, StrideBytes, 3 * sizeof(float));

        GL.BindVertexArray(0);
    }

    public void SetData(float[] verticesXYZAge, int vertexCount)
    {
        if (verticesXYZAge == null) throw new ArgumentNullException(nameof(verticesXYZAge));
        if (vertexCount < 0) throw new ArgumentOutOfRangeException(nameof(vertexCount));
        if (vertexCount > _capacityVerts) throw new ArgumentException("vertexCount > capacity");
        if (verticesXYZAge.Length < vertexCount * FloatsPerVertex) throw new ArgumentException("vertices array too small");

        Count = vertexCount;

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero,
            vertexCount * FloatsPerVertex * sizeof(float),
            verticesXYZAge);
    }

    public void Draw(PrimitiveType prim)
    {
        if (Count < 2) return;
        GL.BindVertexArray(_vao);
        GL.DrawArrays(prim, 0, Count);
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);
    }
}