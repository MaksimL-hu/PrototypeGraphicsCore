using System;
using System.Runtime.InteropServices;

namespace MyMath
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2i : IEquatable<Vec2i>
    {
        public int X;
        public int Y;

        public Vec2i(int x, int y) { X = x; Y = y; }
        public Vec2i(int value) { X = value; Y = value; }

        public int this[int index]
        {
            get => index switch
            {
                0 => X,
                1 => Y,
                _ => throw new IndexOutOfRangeException("Vec2i index must be 0 or 1.")
            };
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    default: throw new IndexOutOfRangeException("Vec2i index must be 0 or 1.");
                }
            }
        }

        public static readonly Vec2i Zero = new(0, 0);
        public static readonly Vec2i One = new(1, 1);
        public static readonly Vec2i UnitX = new(1, 0);
        public static readonly Vec2i UnitY = new(0, 1);

        public int LengthSquared => X * X + Y * Y;
        public double Length => Math.Sqrt(LengthSquared);

        public static Vec2i operator +(Vec2i a, Vec2i b) => new(a.X + b.X, a.Y + b.Y);
        public static Vec2i operator -(Vec2i a, Vec2i b) => new(a.X - b.X, a.Y - b.Y);
        public static Vec2i operator -(Vec2i v) => new(-v.X, -v.Y);

        public static Vec2i operator *(Vec2i v, int s) => new(v.X * s, v.Y * s);
        public static Vec2i operator *(int s, Vec2i v) => new(v.X * s, v.Y * s);
        public static Vec2i operator /(Vec2i v, int s) => new(v.X / s, v.Y / s);

        public static bool operator ==(Vec2i a, Vec2i b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Vec2i a, Vec2i b) => !(a == b);

        public bool Equals(Vec2i other) => this == other;
        public override bool Equals(object? obj) => obj is Vec2i other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);

        public override string ToString() => $"({X}, {Y})";
    }
}