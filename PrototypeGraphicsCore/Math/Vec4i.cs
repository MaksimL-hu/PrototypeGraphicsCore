using System;
using System.Runtime.InteropServices;

namespace MyMath
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec4i : IEquatable<Vec4i>
    {
        public int X;
        public int Y;
        public int Z;
        public int W;

        public Vec4i(int x, int y, int z, int w) { X = x; Y = y; Z = z; W = w; }
        public Vec4i(int value) { X = value; Y = value; Z = value; W = value; }

        public int this[int index]
        {
            get => index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                3 => W,
                _ => throw new IndexOutOfRangeException("Vec4i index must be 0..3.")
            };
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    case 3: W = value; break;
                    default: throw new IndexOutOfRangeException("Vec4i index must be 0..3.");
                }
            }
        }

        public static readonly Vec4i Zero = new(0, 0, 0, 0);
        public static readonly Vec4i One = new(1, 1, 1, 1);

        public static readonly Vec4i UnitX = new(1, 0, 0, 0);
        public static readonly Vec4i UnitY = new(0, 1, 0, 0);
        public static readonly Vec4i UnitZ = new(0, 0, 1, 0);
        public static readonly Vec4i UnitW = new(0, 0, 0, 1);

        public int LengthSquared => X * X + Y * Y + Z * Z + W * W;
        public double Length => Math.Sqrt(LengthSquared);

        public static Vec4i operator +(Vec4i a, Vec4i b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        public static Vec4i operator -(Vec4i a, Vec4i b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        public static Vec4i operator -(Vec4i v) => new(-v.X, -v.Y, -v.Z, -v.W);

        public static Vec4i operator *(Vec4i v, int s) => new(v.X * s, v.Y * s, v.Z * s, v.W * s);
        public static Vec4i operator *(int s, Vec4i v) => new(v.X * s, v.Y * s, v.Z * s, v.W * s);
        public static Vec4i operator /(Vec4i v, int s) => new(v.X / s, v.Y / s, v.Z / s, v.W / s);

        public static bool operator ==(Vec4i a, Vec4i b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        public static bool operator !=(Vec4i a, Vec4i b) => !(a == b);

        public bool Equals(Vec4i other) => this == other;
        public override bool Equals(object? obj) => obj is Vec4i other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

        public override string ToString() => $"({X}, {Y}, {Z}, {W})";
    }
}