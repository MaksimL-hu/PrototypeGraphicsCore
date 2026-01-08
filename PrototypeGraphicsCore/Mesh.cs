using System;
using OpenTK.Graphics.OpenGL4;

namespace PrototypeGraphicsCore;

public sealed class Mesh : IDisposable
{
    private int _vao;
    private int _vbo;
    private int _ebo;
    private int _indexCount;
    private bool _disposed;

    public int IndexCount => _indexCount;
    public bool IsDisposed => _disposed;

    public Mesh(float[] vertices, uint[] indices)
    {
        if (vertices is null || vertices.Length == 0) throw new ArgumentException("vertices is null/empty");
        if (indices is null || indices.Length == 0) throw new ArgumentException("indices is null/empty");

        _indexCount = indices.Length;

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        // layout(location=0): position vec3
        // layout(location=1): normal vec3
        int stride = 6 * sizeof(float);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));

        GL.BindVertexArray(0);

        // Важно: ArrayBuffer можно отвязать, EBO НЕ надо (он хранится в VAO)
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void Draw()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(Mesh));

        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        if (_ebo != 0) { GL.DeleteBuffer(_ebo); _ebo = 0; }
        if (_vbo != 0) { GL.DeleteBuffer(_vbo); _vbo = 0; }
        if (_vao != 0) { GL.DeleteVertexArray(_vao); _vao = 0; }

        _indexCount = 0;
    }
}