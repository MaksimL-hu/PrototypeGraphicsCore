using System;
using System.Runtime.InteropServices;

namespace MyMath
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec4 : IEquatable<Vec4>
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vec4(float x, float y, float z, float w) { X = x; Y = y; Z = z; W = w; }
        public Vec4(Vec3 v, float w) : this(v.X, v.Y, v.Z, w) { }
        public Vec4(float value) : this(value, value, value, value) { }

        public float this[int index]
        {
            get => index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                3 => W,
                _ => throw new IndexOutOfRangeException("Vec4 index must be 0..3.")
            };
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    case 3: W = value; break;
                    default: throw new IndexOutOfRangeException("Vec4 index must be 0..3.");
                }
            }
        }

        public Vec3 Xyz => new Vec3(X, Y, Z);

        public static readonly Vec4 Zero = new(0f, 0f, 0f, 0f);
        public static readonly Vec4 One = new(1f, 1f, 1f, 1f);

        public static readonly Vec4 UnitX = new(1f, 0f, 0f, 0f);
        public static readonly Vec4 UnitY = new(0f, 1f, 0f, 0f);
        public static readonly Vec4 UnitZ = new(0f, 0f, 1f, 0f);
        public static readonly Vec4 UnitW = new(0f, 0f, 0f, 1f);

        public float LengthSquared => X * X + Y * Y + Z * Z + W * W;
        public float Length => MathF.Sqrt(LengthSquared);

        public void Normalize()
        {
            float len = Length;
            if (len > 1e-8f)
            {
                X /= len; Y /= len; Z /= len; W /= len;
            }
            else
            {
                X = 0f; Y = 0f; Z = 0f; W = 0f;
            }
        }

        public Vec4 Normalized()
        {
            var v = this;
            v.Normalize();
            return v;
        }

        public static Vec4 Normalize(Vec4 v) { v.Normalize(); return v; }
        public static Vec4 Normalized(Vec4 v) => Normalize(v);

        public static float Dot(Vec4 a, Vec4 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;

        public static Vec4 Lerp(Vec4 a, Vec4 b, float t) => a + (b - a) * t;

        public static Vec4 operator +(Vec4 a, Vec4 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        public static Vec4 operator -(Vec4 a, Vec4 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        public static Vec4 operator -(Vec4 v) => new(-v.X, -v.Y, -v.Z, -v.W);

        public static Vec4 operator *(Vec4 v, float s) => new(v.X * s, v.Y * s, v.Z * s, v.W * s);
        public static Vec4 operator *(float s, Vec4 v) => new(v.X * s, v.Y * s, v.Z * s, v.W * s);

        public static Vec4 operator /(Vec4 v, float s) => new(v.X / s, v.Y / s, v.Z / s, v.W / s);

        public static bool operator ==(Vec4 a, Vec4 b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        public static bool operator !=(Vec4 a, Vec4 b) => !(a == b);

        public bool Equals(Vec4 other) => this == other;
        public override bool Equals(object? obj) => obj is Vec4 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

        public override string ToString() => $"({X:0.###}, {Y:0.###}, {Z:0.###}, {W:0.###})";

        public static Vec4 TransformRow(in Vec4 v, in Mat4 m)
        {
            // row-vector * matrix: r_j = sum_i v_i * m[i,j]
            return new Vec4(
                v.X * m[0, 0] + v.Y * m[1, 0] + v.Z * m[2, 0] + v.W * m[3, 0],
                v.X * m[0, 1] + v.Y * m[1, 1] + v.Z * m[2, 1] + v.W * m[3, 1],
                v.X * m[0, 2] + v.Y * m[1, 2] + v.Z * m[2, 2] + v.W * m[3, 2],
                v.X * m[0, 3] + v.Y * m[1, 3] + v.Z * m[2, 3] + v.W * m[3, 3]
            );
        }

        public static Vec4 TransformColumn(in Vec4 v, in Mat4 m)
        {
            return m * v;
        }
    }
}