using MyMath;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PrototypeGraphicsCore;

public sealed class Shader : IDisposable
{
    private readonly float[] _mat16 = new float[16];
    private readonly float[] _mat9 = new float[9];
    private readonly Dictionary<string, int> _uniformCache = new();

    private bool _disposed;

    public int Handle { get; }

    public Shader(string vertexSource, string fragmentSource)
    {
        int vertex = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertex, vertexSource);
        GL.CompileShader(vertex);
        CheckShader(vertex, "VERTEX", vertexSource);

        int fragment = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragment, fragmentSource);
        GL.CompileShader(fragment);
        CheckShader(fragment, "FRAGMENT", fragmentSource);

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vertex);
        GL.AttachShader(Handle, fragment);
        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int ok);
        if (ok == 0)
        {
            string log = GL.GetProgramInfoLog(Handle);
            throw new Exception($"Shader program link error:\n{log}");
        }

        GL.DetachShader(Handle, vertex);
        GL.DetachShader(Handle, fragment);
        GL.DeleteShader(vertex);
        GL.DeleteShader(fragment);
    }

    public void Use() => GL.UseProgram(Handle);

    private int GetLocation(string name)
    {
        if (_uniformCache.TryGetValue(name, out int loc))
            return loc;

        loc = GL.GetUniformLocation(Handle, name);
        _uniformCache[name] = loc; // кешируем и -1 тоже
        return loc;
    }

    // ---------- Matrices ----------
    public void SetMatrix4(string name, in Mat4 value, bool transpose = false)
    {
        int loc = GetLocation(name);
        if (loc == -1) return;

        value.ToColumnMajorArray(_mat16);
        GL.UniformMatrix4(loc, 1, transpose, _mat16);
    }

    public void SetMatrix3(string name, in Mat3 value, bool transpose = false)
    {
        int loc = GetLocation(name);
        if (loc == -1) return;

        value.ToColumnMajorArray(_mat9);
        GL.UniformMatrix3(loc, 1, transpose, _mat9);
    }

    // ---------- Float vectors ----------
    public void SetVector2(string name, in Vec2 value)
    {
        int loc = GetLocation(name);
        if (loc == -1) return;

        GL.Uniform2(loc, value.X, value.Y);
    }

    public void SetVector3(string name, in Vec3 value)
    {
        int loc = GetLocation(name);
        if (loc == -1) return;

        GL.Uniform3(loc, value.X, value.Y, value.Z);
    }

    public void SetVector4(string name, in Vec4 value)
    {
        int loc = GetLocation(name);
        if (loc == -1) return;

        GL.Uniform4(loc, value.X, value.Y, value.Z, value.W);
    }

    // ---------- Int vectors ----------
    public void SetVector2i(string name, in Vec2i value)
    {
        int loc = GetLocation(name);
        if (loc == -1) return;

        GL.Uniform2(loc, value.X, value.Y);
    }

    public void SetVector3i(string name, in Vec3i value)
    {
        int loc = GetLocation(name);
        if (loc == -1) return;

        GL.Uniform3(loc, value.X, value.Y, value.Z);
    }

    public void SetVector4i(string name, in Vec4i value)
    {
        int loc = GetLocation(name);
        if (loc == -1) return;

        GL.Uniform4(loc, value.X, value.Y, value.Z, value.W);
    }

    // ---------- Scalars ----------
    public void SetFloat(string name, float value)
    {
        int loc = GetLocation(name);
        if (loc == -1) return;

        GL.Uniform1(loc, value);
    }

    public void SetInt(string name, int value)
    {
        int loc = GetLocation(name);
        if (loc == -1) return;

        GL.Uniform1(loc, value);
    }

    public void SetBool(string name, bool value) => SetInt(name, value ? 1 : 0);

    /// <summary>Для sampler2D/samplerCube и т.п.: просто задаём texture unit.</summary>
    public void SetTextureUnit(string samplerUniformName, int unit) => SetInt(samplerUniformName, unit);

    // ---------- Compile helper ----------
    private static void CheckShader(int shader, string stage, string source)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int ok);
        if (ok != 0) return;

        string log = GL.GetShaderInfoLog(shader);

        var lines = source.Replace("\r\n", "\n").Split('\n');
        var numbered = new StringBuilder();
        for (int i = 0; i < lines.Length; i++)
            numbered.AppendLine($"{i + 1:000}: {lines[i]}");

        throw new Exception(
            $"{stage} shader compile error:\n{log}\n" +
            $"--- Source length: {source.Length} ---\n" +
            $"{numbered}"
        );
    }

    // ---------- FromFiles ----------
    public static Shader FromFiles(string vertexPath, string fragmentPath)
    {
        string vFull = Path.Combine(AppContext.BaseDirectory, vertexPath);
        string fFull = Path.Combine(AppContext.BaseDirectory, fragmentPath);

        if (!File.Exists(vFull))
            throw new FileNotFoundException("Vertex shader file not found", vFull);
        if (!File.Exists(fFull))
            throw new FileNotFoundException("Fragment shader file not found", fFull);

        string vSrc = File.ReadAllText(vFull, Encoding.UTF8);
        string fSrc = File.ReadAllText(fFull, Encoding.UTF8);

        return new Shader(vSrc, fSrc);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _uniformCache.Clear();
        GL.DeleteProgram(Handle);
    }
}