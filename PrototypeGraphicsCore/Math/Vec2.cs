using System;
using System.Runtime.InteropServices;

namespace MyMath
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2 : IEquatable<Vec2>
    {
        public float X;
        public float Y;

        public Vec2(float x, float y) { X = x; Y = y; }
        public Vec2(float value) { X = value; Y = value; }

        public float this[int index]
        {
            get => index switch
            {
                0 => X,
                1 => Y,
                _ => throw new IndexOutOfRangeException("Vec2 index must be 0 or 1.")
            };
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    default: throw new IndexOutOfRangeException("Vec2 index must be 0 or 1.");
                }
            }
        }

        public static readonly Vec2 Zero = new(0f, 0f);
        public static readonly Vec2 One = new(1f, 1f);
        public static readonly Vec2 UnitX = new(1f, 0f);
        public static readonly Vec2 UnitY = new(0f, 1f);

        public float LengthSquared => X * X + Y * Y;
        public float Length => MathF.Sqrt(LengthSquared);

        public void Normalize()
        {
            float len = Length;
            if (len > 1e-8f)
            {
                X /= len;
                Y /= len;
            }
            else
            {
                X = 0f;
                Y = 0f;
            }
        }

        public Vec2 Normalized()
        {
            var v = this;
            v.Normalize();
            return v;
        }

        public static float Dot(Vec2 a, Vec2 b) => a.X * b.X + a.Y * b.Y;

        public static float DistanceSquared(Vec2 a, Vec2 b) => (a - b).LengthSquared;
        public static float Distance(Vec2 a, Vec2 b) => MathF.Sqrt(DistanceSquared(a, b));

        public static Vec2 Lerp(Vec2 a, Vec2 b, float t) => a + (b - a) * t;

        public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);
        public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);
        public static Vec2 operator -(Vec2 v) => new(-v.X, -v.Y);

        public static Vec2 operator *(Vec2 v, float s) => new(v.X * s, v.Y * s);
        public static Vec2 operator *(float s, Vec2 v) => new(v.X * s, v.Y * s);
        public static Vec2 operator /(Vec2 v, float s) => new(v.X / s, v.Y / s);

        public static bool operator ==(Vec2 a, Vec2 b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Vec2 a, Vec2 b) => !(a == b);

        public bool Equals(Vec2 other) => this == other;
        public override bool Equals(object? obj) => obj is Vec2 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);

        public override string ToString() => $"({X:0.###}, {Y:0.###})";
    }
}