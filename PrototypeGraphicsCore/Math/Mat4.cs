using System;
using System.Runtime.InteropServices;

namespace MyMath
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Mat4 : IEquatable<Mat4>
    {
        // Storage is COLUMN-major (like OpenGL):
        // column 0
        public float M00, M10, M20, M30;
        // column 1
        public float M01, M11, M21, M31;
        // column 2
        public float M02, M12, M22, M32;
        // column 3
        public float M03, M13, M23, M33;

        public static Mat4 Zero => new Mat4();

        public static Mat4 Identity => new Mat4
        {
            M00 = 1f,
            M11 = 1f,
            M22 = 1f,
            M33 = 1f
        };

        // Indexer uses [row, col] semantic.
        public float this[int row, int col]
        {
            get
            {
                if ((uint)row > 3 || (uint)col > 3) throw new IndexOutOfRangeException();
                return (col, row) switch
                {
                    (0, 0) => M00,
                    (0, 1) => M10,
                    (0, 2) => M20,
                    (0, 3) => M30,
                    (1, 0) => M01,
                    (1, 1) => M11,
                    (1, 2) => M21,
                    (1, 3) => M31,
                    (2, 0) => M02,
                    (2, 1) => M12,
                    (2, 2) => M22,
                    (2, 3) => M32,
                    (3, 0) => M03,
                    (3, 1) => M13,
                    (3, 2) => M23,
                    (3, 3) => M33,
                    _ => throw new IndexOutOfRangeException()
                };
            }
            set
            {
                if ((uint)row > 3 || (uint)col > 3) throw new IndexOutOfRangeException();
                switch (col, row)
                {
                    case (0, 0): M00 = value; break;
                    case (0, 1): M10 = value; break;
                    case (0, 2): M20 = value; break;
                    case (0, 3): M30 = value; break;

                    case (1, 0): M01 = value; break;
                    case (1, 1): M11 = value; break;
                    case (1, 2): M21 = value; break;
                    case (1, 3): M31 = value; break;

                    case (2, 0): M02 = value; break;
                    case (2, 1): M12 = value; break;
                    case (2, 2): M22 = value; break;
                    case (2, 3): M32 = value; break;

                    case (3, 0): M03 = value; break;
                    case (3, 1): M13 = value; break;
                    case (3, 2): M23 = value; break;
                    case (3, 3): M33 = value; break;

                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        // OpenTK-like Row/Column properties
        public Vec4 Row0
        {
            get => new Vec4(M00, M01, M02, M03);
            set { M00 = value.X; M01 = value.Y; M02 = value.Z; M03 = value.W; }
        }
        public Vec4 Row1
        {
            get => new Vec4(M10, M11, M12, M13);
            set { M10 = value.X; M11 = value.Y; M12 = value.Z; M13 = value.W; }
        }
        public Vec4 Row2
        {
            get => new Vec4(M20, M21, M22, M23);
            set { M20 = value.X; M21 = value.Y; M22 = value.Z; M23 = value.W; }
        }
        public Vec4 Row3
        {
            get => new Vec4(M30, M31, M32, M33);
            set { M30 = value.X; M31 = value.Y; M32 = value.Z; M33 = value.W; }
        }

        public Vec4 Column0
        {
            get => new Vec4(M00, M10, M20, M30);
            set { M00 = value.X; M10 = value.Y; M20 = value.Z; M30 = value.W; }
        }
        public Vec4 Column1
        {
            get => new Vec4(M01, M11, M21, M31);
            set { M01 = value.X; M11 = value.Y; M21 = value.Z; M31 = value.W; }
        }
        public Vec4 Column2
        {
            get => new Vec4(M02, M12, M22, M32);
            set { M02 = value.X; M12 = value.Y; M22 = value.Z; M32 = value.W; }
        }
        public Vec4 Column3
        {
            get => new Vec4(M03, M13, M23, M33);
            set { M03 = value.X; M13 = value.Y; M23 = value.Z; M33 = value.W; }
        }

        // Multiplication (standard math): r = a * b
        public static Mat4 operator *(Mat4 a, Mat4 b)
        {
            Mat4 r = new();
            for (int col = 0; col < 4; col++)
            {
                for (int row = 0; row < 4; row++)
                {
                    float sum = 0f;
                    for (int k = 0; k < 4; k++)
                        sum += a[row, k] * b[k, col];
                    r[row, col] = sum;
                }
            }
            return r;
        }

        // Column-vector convention: v' = M * v
        public static Vec4 operator *(Mat4 m, Vec4 v)
        {
            return new Vec4(
                m[0, 0] * v.X + m[0, 1] * v.Y + m[0, 2] * v.Z + m[0, 3] * v.W,
                m[1, 0] * v.X + m[1, 1] * v.Y + m[1, 2] * v.Z + m[1, 3] * v.W,
                m[2, 0] * v.X + m[2, 1] * v.Y + m[2, 2] * v.Z + m[2, 3] * v.W,
                m[3, 0] * v.X + m[3, 1] * v.Y + m[3, 2] * v.Z + m[3, 3] * v.W
            );
        }

        public static Mat4 operator +(Mat4 a, Mat4 b)
        {
            return new Mat4
            {
                M00 = a.M00 + b.M00,
                M10 = a.M10 + b.M10,
                M20 = a.M20 + b.M20,
                M30 = a.M30 + b.M30,
                M01 = a.M01 + b.M01,
                M11 = a.M11 + b.M11,
                M21 = a.M21 + b.M21,
                M31 = a.M31 + b.M31,
                M02 = a.M02 + b.M02,
                M12 = a.M12 + b.M12,
                M22 = a.M22 + b.M22,
                M32 = a.M32 + b.M32,
                M03 = a.M03 + b.M03,
                M13 = a.M13 + b.M13,
                M23 = a.M23 + b.M23,
                M33 = a.M33 + b.M33,
            };
        }

        public static Mat4 operator -(Mat4 a, Mat4 b)
        {
            return new Mat4
            {
                M00 = a.M00 - b.M00,
                M10 = a.M10 - b.M10,
                M20 = a.M20 - b.M20,
                M30 = a.M30 - b.M30,
                M01 = a.M01 - b.M01,
                M11 = a.M11 - b.M11,
                M21 = a.M21 - b.M21,
                M31 = a.M31 - b.M31,
                M02 = a.M02 - b.M02,
                M12 = a.M12 - b.M12,
                M22 = a.M22 - b.M22,
                M32 = a.M32 - b.M32,
                M03 = a.M03 - b.M03,
                M13 = a.M13 - b.M13,
                M23 = a.M23 - b.M23,
                M33 = a.M33 - b.M33,
            };
        }

        public static Mat4 operator *(Mat4 m, float s)
        {
            return new Mat4
            {
                M00 = m.M00 * s,
                M10 = m.M10 * s,
                M20 = m.M20 * s,
                M30 = m.M30 * s,
                M01 = m.M01 * s,
                M11 = m.M11 * s,
                M21 = m.M21 * s,
                M31 = m.M31 * s,
                M02 = m.M02 * s,
                M12 = m.M12 * s,
                M22 = m.M22 * s,
                M32 = m.M32 * s,
                M03 = m.M03 * s,
                M13 = m.M13 * s,
                M23 = m.M23 * s,
                M33 = m.M33 * s,
            };
        }

        public static Mat4 operator *(float s, Mat4 m) => m * s;

        public static Mat4 CreateTranslation(Vec3 t)
        {
            var m = Identity;
            m[0, 3] = t.X;
            m[1, 3] = t.Y;
            m[2, 3] = t.Z;
            return m;
        }

        public static Mat4 CreateScale(float s)
        {
            var m = Identity;
            m[0, 0] = s; m[1, 1] = s; m[2, 2] = s;
            return m;
        }

        public static Mat4 CreateScale(Vec3 s)
        {
            var m = Identity;
            m[0, 0] = s.X; m[1, 1] = s.Y; m[2, 2] = s.Z;
            return m;
        }

        public static Mat4 CreateRotationX(float radians)
        {
            float c = MathF.Cos(radians);
            float s = MathF.Sin(radians);
            var m = Identity;
            m[1, 1] = c; m[1, 2] = -s;
            m[2, 1] = s; m[2, 2] = c;
            return m;
        }

        public static Mat4 CreateRotationY(float radians)
        {
            float c = MathF.Cos(radians);
            float s = MathF.Sin(radians);
            var m = Identity;
            m[0, 0] = c; m[0, 2] = s;
            m[2, 0] = -s; m[2, 2] = c;
            return m;
        }

        public static Mat4 CreateRotationZ(float radians)
        {
            float c = MathF.Cos(radians);
            float s = MathF.Sin(radians);
            var m = Identity;
            m[0, 0] = c; m[0, 1] = -s;
            m[1, 0] = s; m[1, 1] = c;
            return m;
        }

        public static Mat4 CreateFromAxisAngle(Vec3 axis, float radians)
        {
            axis = axis.Normalized();
            float x = axis.X, y = axis.Y, z = axis.Z;
            float c = MathF.Cos(radians);
            float s = MathF.Sin(radians);
            float t = 1f - c;

            var m = Identity;

            m[0, 0] = t * x * x + c;
            m[0, 1] = t * x * y - s * z;
            m[0, 2] = t * x * z + s * y;

            m[1, 0] = t * x * y + s * z;
            m[1, 1] = t * y * y + c;
            m[1, 2] = t * y * z - s * x;

            m[2, 0] = t * x * z - s * y;
            m[2, 1] = t * y * z + s * x;
            m[2, 2] = t * z * z + c;

            return m;
        }

        public static Mat4 LookAt(Vec3 eye, Vec3 target, Vec3 up)
        {
            Vec3 f = (target - eye).Normalized();
            Vec3 s = Vec3.Cross(f, up).Normalized();
            Vec3 u = Vec3.Cross(s, f);

            var m = Identity;

            // basis
            m[0, 0] = s.X; m[1, 0] = s.Y; m[2, 0] = s.Z;
            m[0, 1] = u.X; m[1, 1] = u.Y; m[2, 1] = u.Z;
            m[0, 2] = -f.X; m[1, 2] = -f.Y; m[2, 2] = -f.Z;

            // translation
            m[0, 3] = -Vec3.Dot(s, eye);
            m[1, 3] = -Vec3.Dot(u, eye);
            m[2, 3] = Vec3.Dot(f, eye);

            return m;
        }

        public static Mat4 CreatePerspectiveFieldOfView(float fovYRadians, float aspect, float zNear, float zFar)
        {
            float f = 1f / MathF.Tan(fovYRadians * 0.5f);

            Mat4 m = new();
            m[0, 0] = f / aspect;
            m[1, 1] = f;
            m[2, 2] = (zFar + zNear) / (zNear - zFar);
            m[2, 3] = (2f * zFar * zNear) / (zNear - zFar);
            m[3, 2] = -1f;
            m[3, 3] = 0f;
            return m;
        }

        // ----- Transpose / Invert (OpenTK-like) -----

        public static Mat4 Transpose(Mat4 m)
        {
            Mat4 r = new();
            for (int row = 0; row < 4; row++)
                for (int col = 0; col < 4; col++)
                    r[row, col] = m[col, row];
            return r;
        }

        public Mat4 Transposed() => Transpose(this);

        public float Determinant
        {
            get
            {
                // Compute determinant via Laplace expansion / cofactors (explicit).
                // Uses [row,col] semantics, so it matches math regardless of storage.
                float a00 = this[0, 0], a01 = this[0, 1], a02 = this[0, 2], a03 = this[0, 3];
                float a10 = this[1, 0], a11 = this[1, 1], a12 = this[1, 2], a13 = this[1, 3];
                float a20 = this[2, 0], a21 = this[2, 1], a22 = this[2, 2], a23 = this[2, 3];
                float a30 = this[3, 0], a31 = this[3, 1], a32 = this[3, 2], a33 = this[3, 3];

                float b00 = a00 * a11 - a01 * a10;
                float b01 = a00 * a12 - a02 * a10;
                float b02 = a00 * a13 - a03 * a10;
                float b03 = a01 * a12 - a02 * a11;
                float b04 = a01 * a13 - a03 * a11;
                float b05 = a02 * a13 - a03 * a12;
                float b06 = a20 * a31 - a21 * a30;
                float b07 = a20 * a32 - a22 * a30;
                float b08 = a20 * a33 - a23 * a30;
                float b09 = a21 * a32 - a22 * a31;
                float b10 = a21 * a33 - a23 * a31;
                float b11 = a22 * a33 - a23 * a32;

                return b00 * b11 - b01 * b10 + b02 * b09 + b03 * b08 - b04 * b07 + b05 * b06;
            }
        }

        // OpenTK-style invert: returns bool success
        public static bool Invert(Mat4 m, out Mat4 inv)
        {
            float a00 = m[0, 0], a01 = m[0, 1], a02 = m[0, 2], a03 = m[0, 3];
            float a10 = m[1, 0], a11 = m[1, 1], a12 = m[1, 2], a13 = m[1, 3];
            float a20 = m[2, 0], a21 = m[2, 1], a22 = m[2, 2], a23 = m[2, 3];
            float a30 = m[3, 0], a31 = m[3, 1], a32 = m[3, 2], a33 = m[3, 3];

            float b00 = a00 * a11 - a01 * a10;
            float b01 = a00 * a12 - a02 * a10;
            float b02 = a00 * a13 - a03 * a10;
            float b03 = a01 * a12 - a02 * a11;
            float b04 = a01 * a13 - a03 * a11;
            float b05 = a02 * a13 - a03 * a12;
            float b06 = a20 * a31 - a21 * a30;
            float b07 = a20 * a32 - a22 * a30;
            float b08 = a20 * a33 - a23 * a30;
            float b09 = a21 * a32 - a22 * a31;
            float b10 = a21 * a33 - a23 * a31;
            float b11 = a22 * a33 - a23 * a32;

            float det = b00 * b11 - b01 * b10 + b02 * b09 + b03 * b08 - b04 * b07 + b05 * b06;

            if (MathF.Abs(det) < 1e-8f)
            {
                inv = Identity;
                return false;
            }

            float invDet = 1f / det;

            inv = new Mat4();

            inv[0, 0] = (a11 * b11 - a12 * b10 + a13 * b09) * invDet;
            inv[0, 1] = (-a01 * b11 + a02 * b10 - a03 * b09) * invDet;
            inv[0, 2] = (a31 * b05 - a32 * b04 + a33 * b03) * invDet;
            inv[0, 3] = (-a21 * b05 + a22 * b04 - a23 * b03) * invDet;

            inv[1, 0] = (-a10 * b11 + a12 * b08 - a13 * b07) * invDet;
            inv[1, 1] = (a00 * b11 - a02 * b08 + a03 * b07) * invDet;
            inv[1, 2] = (-a30 * b05 + a32 * b02 - a33 * b01) * invDet;
            inv[1, 3] = (a20 * b05 - a22 * b02 + a23 * b01) * invDet;

            inv[2, 0] = (a10 * b10 - a11 * b08 + a13 * b06) * invDet;
            inv[2, 1] = (-a00 * b10 + a01 * b08 - a03 * b06) * invDet;
            inv[2, 2] = (a30 * b04 - a31 * b02 + a33 * b00) * invDet;
            inv[2, 3] = (-a20 * b04 + a21 * b02 - a23 * b00) * invDet;

            inv[3, 0] = (-a10 * b09 + a11 * b07 - a12 * b06) * invDet;
            inv[3, 1] = (a00 * b09 - a01 * b07 + a02 * b06) * invDet;
            inv[3, 2] = (-a30 * b03 + a31 * b01 - a32 * b00) * invDet;
            inv[3, 3] = (a20 * b03 - a21 * b01 + a22 * b00) * invDet;

            return true;
        }

        public Mat4 Inverted()
        {
            Invert(this, out var inv);
            return inv;
        }

        // ----- Equality -----

        public static bool operator ==(Mat4 a, Mat4 b)
        {
            return a.M00 == b.M00 && a.M10 == b.M10 && a.M20 == b.M20 && a.M30 == b.M30 &&
                   a.M01 == b.M01 && a.M11 == b.M11 && a.M21 == b.M21 && a.M31 == b.M31 &&
                   a.M02 == b.M02 && a.M12 == b.M12 && a.M22 == b.M22 && a.M32 == b.M32 &&
                   a.M03 == b.M03 && a.M13 == b.M13 && a.M23 == b.M23 && a.M33 == b.M33;
        }

        public static bool operator !=(Mat4 a, Mat4 b) => !(a == b);

        public bool Equals(Mat4 other) => this == other;
        public override bool Equals(object? obj) => obj is Mat4 other && Equals(other);
        public override int GetHashCode()
        {
            int h0 = HashCode.Combine(M00, M10, M20, M30);
            int h1 = HashCode.Combine(M01, M11, M21, M31);
            int h2 = HashCode.Combine(M02, M12, M22, M32);
            int h3 = HashCode.Combine(M03, M13, M23, M33);
            return HashCode.Combine(h0, h1, h2, h3);
        }

        public override string ToString()
            => $"[{Row0} | {Row1} | {Row2} | {Row3}]";

        /// <summary>Column-major float[16] for glUniformMatrix4fv with transpose=false.</summary>
        public void ToColumnMajorArray(float[] dst16)
        {
            if (dst16 is null || dst16.Length < 16) throw new ArgumentException("dst16 must have length >= 16");

            dst16[0] = M00; dst16[1] = M10; dst16[2] = M20; dst16[3] = M30;
            dst16[4] = M01; dst16[5] = M11; dst16[6] = M21; dst16[7] = M31;
            dst16[8] = M02; dst16[9] = M12; dst16[10] = M22; dst16[11] = M32;
            dst16[12] = M03; dst16[13] = M13; dst16[14] = M23; dst16[15] = M33;
        }
    }
}