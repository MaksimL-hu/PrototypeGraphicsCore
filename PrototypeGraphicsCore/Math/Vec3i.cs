using System;
using System.Runtime.InteropServices;

namespace MyMath
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec3i : IEquatable<Vec3i>
    {
        public int X;
        public int Y;
        public int Z;

        public Vec3i(int x, int y, int z) { X = x; Y = y; Z = z; }
        public Vec3i(int value) { X = value; Y = value; Z = value; }

        public int this[int index]
        {
            get => index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new IndexOutOfRangeException("Vector3i index must be 0, 1 or 2.")
            };
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default: throw new IndexOutOfRangeException("Vector3i index must be 0, 1 or 2.");
                }
            }
        }

        public static readonly Vec3i Zero = new(0, 0, 0);
        public static readonly Vec3i One = new(1, 1, 1);
        public static readonly Vec3i UnitX = new(1, 0, 0);
        public static readonly Vec3i UnitY = new(0, 1, 0);
        public static readonly Vec3i UnitZ = new(0, 0, 1);

        public int LengthSquared => X * X + Y * Y + Z * Z;
        public double Length => Math.Sqrt(LengthSquared);

        public static Vec3i operator +(Vec3i a, Vec3i b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vec3i operator -(Vec3i a, Vec3i b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vec3i operator -(Vec3i v) => new(-v.X, -v.Y, -v.Z);

        public static Vec3i operator *(Vec3i v, int s) => new(v.X * s, v.Y * s, v.Z * s);
        public static Vec3i operator *(int s, Vec3i v) => new(v.X * s, v.Y * s, v.Z * s);
        public static Vec3i operator /(Vec3i v, int s) => new(v.X / s, v.Y / s, v.Z / s);

        public static bool operator ==(Vec3i a, Vec3i b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        public static bool operator !=(Vec3i a, Vec3i b) => !(a == b);

        public bool Equals(Vec3i other) => this == other;
        public override bool Equals(object? obj) => obj is Vec3i other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public override string ToString() => $"({X}, {Y}, {Z})";
    }
}