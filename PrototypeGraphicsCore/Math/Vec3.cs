using System;
using System.Runtime.InteropServices;

namespace MyMath
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec3 : IEquatable<Vec3>
    {
        public float X;
        public float Y;
        public float Z;

        public Vec3(float x, float y, float z) { X = x; Y = y; Z = z; }
        public Vec3(float value) { X = value; Y = value; Z = value; }

        public float this[int index]
        {
            get => index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new IndexOutOfRangeException("Vec3 index must be 0, 1 or 2.")
            };
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default: throw new IndexOutOfRangeException("Vec3 index must be 0, 1 or 2.");
                }
            }
        }

        public static readonly Vec3 Zero = new(0f, 0f, 0f);
        public static readonly Vec3 One = new(1f, 1f, 1f);
        public static readonly Vec3 UnitX = new(1f, 0f, 0f);
        public static readonly Vec3 UnitY = new(0f, 1f, 0f);
        public static readonly Vec3 UnitZ = new(0f, 0f, 1f);

        public float LengthSquared => X * X + Y * Y + Z * Z;
        public float Length => MathF.Sqrt(LengthSquared);

        public void Normalize()
        {
            float len = Length;
            if (len > 1e-8f)
            {
                X /= len; Y /= len; Z /= len;
            }
            else
            {
                X = 0f; Y = 0f; Z = 0f;
            }
        }

        public Vec3 Normalized()
        {
            var v = this;
            v.Normalize();
            return v;
        }

        public static Vec3 Normalize(Vec3 v) { v.Normalize(); return v; }
        public static Vec3 Normalized(Vec3 v) => Normalize(v);

        public static float Dot(Vec3 a, Vec3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        public static Vec3 Cross(Vec3 a, Vec3 b) =>
            new(a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X);

        public static float DistanceSquared(Vec3 a, Vec3 b) => (a - b).LengthSquared;
        public static float Distance(Vec3 a, Vec3 b) => MathF.Sqrt(DistanceSquared(a, b));

        public static Vec3 Lerp(Vec3 a, Vec3 b, float t) => a + (b - a) * t;

        public static Vec3 operator +(Vec3 a, Vec3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vec3 operator -(Vec3 a, Vec3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vec3 operator -(Vec3 v) => new(-v.X, -v.Y, -v.Z);

        public static Vec3 operator *(Vec3 v, float s) => new(v.X * s, v.Y * s, v.Z * s);
        public static Vec3 operator *(float s, Vec3 v) => new(v.X * s, v.Y * s, v.Z * s);
        public static Vec3 operator /(Vec3 v, float s) => new(v.X / s, v.Y / s, v.Z / s);

        public static bool operator ==(Vec3 a, Vec3 b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        public static bool operator !=(Vec3 a, Vec3 b) => !(a == b);

        public bool Equals(Vec3 other) => this == other;
        public override bool Equals(object? obj) => obj is Vec3 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public override string ToString() => $"({X:0.###}, {Y:0.###}, {Z:0.###})";
    }
}