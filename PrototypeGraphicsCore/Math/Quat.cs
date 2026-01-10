using System;
using System.Runtime.InteropServices;

namespace MyMath
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Quat
    {
        public readonly float X, Y, Z, W;
        public Quat(float x, float y, float z, float w) { X = x; Y = y; Z = z; W = w; }

        public static readonly Quat Identity = new(0, 0, 0, 1);

        public float Length => MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);

        public Quat Normalized()
        {
            float len = Length;
            return len > 1e-8f ? new Quat(X / len, Y / len, Z / len, W / len) : Identity;
        }

        public static Quat FromAxisAngle(Vec3 axis, float radians)
        {
            axis = axis.Normalized();
            float half = radians * 0.5f;
            float s = MathF.Sin(half);
            float c = MathF.Cos(half);
            return new Quat(axis.X * s, axis.Y * s, axis.Z * s, c).Normalized();
        }

        public static Quat operator *(Quat a, Quat b)
        {
            return new Quat(
                a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
                a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
                a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W,
                a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z
            );
        }

        public Mat4 ToMat4()
        {
            // unit quaternion -> rotation matrix
            float xx = X * X, yy = Y * Y, zz = Z * Z;
            float xy = X * Y, xz = X * Z, yz = Y * Z;
            float wx = W * X, wy = W * Y, wz = W * Z;

            Mat4 m = Mat4.Identity;
            m[0, 0] = 1f - 2f * (yy + zz);
            m[1, 0] = 2f * (xy + wz);
            m[2, 0] = 2f * (xz - wy);

            m[0, 1] = 2f * (xy - wz);
            m[1, 1] = 1f - 2f * (xx + zz);
            m[2, 1] = 2f * (yz + wx);

            m[0, 2] = 2f * (xz + wy);
            m[1, 2] = 2f * (yz - wx);
            m[2, 2] = 1f - 2f * (xx + yy);
            return m;
        }
    }
}
