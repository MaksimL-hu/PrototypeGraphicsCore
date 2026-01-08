using System;
using System.Runtime.InteropServices;

namespace MyMath
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Mat3 : IEquatable<Mat3>
    {
        // Column-major storage:
        // column 0
        public float M00, M10, M20;
        // column 1
        public float M01, M11, M21;
        // column 2
        public float M02, M12, M22;

        public static Mat3 Zero => new Mat3();

        public static Mat3 Identity => new Mat3
        {
            M00 = 1f,
            M11 = 1f,
            M22 = 1f
        };

        // [row, col]
        public float this[int row, int col]
        {
            get
            {
                if ((uint)row > 2 || (uint)col > 2) throw new IndexOutOfRangeException();
                return (col, row) switch
                {
                    (0, 0) => M00,
                    (0, 1) => M10,
                    (0, 2) => M20,

                    (1, 0) => M01,
                    (1, 1) => M11,
                    (1, 2) => M21,

                    (2, 0) => M02,
                    (2, 1) => M12,
                    (2, 2) => M22,

                    _ => throw new IndexOutOfRangeException()
                };
            }
            set
            {
                if ((uint)row > 2 || (uint)col > 2) throw new IndexOutOfRangeException();
                switch (col, row)
                {
                    case (0, 0): M00 = value; break;
                    case (0, 1): M10 = value; break;
                    case (0, 2): M20 = value; break;

                    case (1, 0): M01 = value; break;
                    case (1, 1): M11 = value; break;
                    case (1, 2): M21 = value; break;

                    case (2, 0): M02 = value; break;
                    case (2, 1): M12 = value; break;
                    case (2, 2): M22 = value; break;

                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        // Multiply: r = a * b
        public static Mat3 operator *(Mat3 a, Mat3 b)
        {
            Mat3 r = new();
            for (int col = 0; col < 3; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    float sum = 0f;
                    for (int k = 0; k < 3; k++)
                        sum += a[row, k] * b[k, col];
                    r[row, col] = sum;
                }
            }
            return r;
        }

        // Column-vector: v' = M * v
        public static Vec3 operator *(Mat3 m, Vec3 v)
        {
            return new Vec3(
                m[0, 0] * v.X + m[0, 1] * v.Y + m[0, 2] * v.Z,
                m[1, 0] * v.X + m[1, 1] * v.Y + m[1, 2] * v.Z,
                m[2, 0] * v.X + m[2, 1] * v.Y + m[2, 2] * v.Z
            );
        }

        public static Mat3 operator +(Mat3 a, Mat3 b)
        {
            return new Mat3
            {
                M00 = a.M00 + b.M00,
                M10 = a.M10 + b.M10,
                M20 = a.M20 + b.M20,
                M01 = a.M01 + b.M01,
                M11 = a.M11 + b.M11,
                M21 = a.M21 + b.M21,
                M02 = a.M02 + b.M02,
                M12 = a.M12 + b.M12,
                M22 = a.M22 + b.M22,
            };
        }

        public static Mat3 operator -(Mat3 a, Mat3 b)
        {
            return new Mat3
            {
                M00 = a.M00 - b.M00,
                M10 = a.M10 - b.M10,
                M20 = a.M20 - b.M20,
                M01 = a.M01 - b.M01,
                M11 = a.M11 - b.M11,
                M21 = a.M21 - b.M21,
                M02 = a.M02 - b.M02,
                M12 = a.M12 - b.M12,
                M22 = a.M22 - b.M22,
            };
        }

        public static Mat3 operator *(Mat3 m, float s)
        {
            return new Mat3
            {
                M00 = m.M00 * s,
                M10 = m.M10 * s,
                M20 = m.M20 * s,
                M01 = m.M01 * s,
                M11 = m.M11 * s,
                M21 = m.M21 * s,
                M02 = m.M02 * s,
                M12 = m.M12 * s,
                M22 = m.M22 * s,
            };
        }

        public static Mat3 operator *(float s, Mat3 m) => m * s;

        // --- Basic constructors ---
        public static Mat3 CreateScale(float s)
        {
            var m = Identity;
            m[0, 0] = s;
            m[1, 1] = s;
            m[2, 2] = s;
            return m;
        }

        public static Mat3 CreateScale(Vec3 s)
        {
            var m = Identity;
            m[0, 0] = s.X;
            m[1, 1] = s.Y;
            m[2, 2] = s.Z;
            return m;
        }

        // Rotation around Z (2D rotation in XY plane)
        public static Mat3 CreateRotationZ(float radians)
        {
            float c = MathF.Cos(radians);
            float s = MathF.Sin(radians);
            var m = Identity;
            m[0, 0] = c; m[0, 1] = -s;
            m[1, 0] = s; m[1, 1] = c;
            return m;
        }

        // Rotation around X
        public static Mat3 CreateRotationX(float radians)
        {
            float c = MathF.Cos(radians);
            float s = MathF.Sin(radians);
            var m = Identity;
            m[1, 1] = c; m[1, 2] = -s;
            m[2, 1] = s; m[2, 2] = c;
            return m;
        }

        // Rotation around Y
        public static Mat3 CreateRotationY(float radians)
        {
            float c = MathF.Cos(radians);
            float s = MathF.Sin(radians);
            var m = Identity;
            m[0, 0] = c; m[0, 2] = s;
            m[2, 0] = -s; m[2, 2] = c;
            return m;
        }

        // Rotation around arbitrary axis
        public static Mat3 CreateFromAxisAngle(Vec3 axis, float radians)
        {
            axis = axis.Normalized();
            float x = axis.X, y = axis.Y, z = axis.Z;

            float c = MathF.Cos(radians);
            float s = MathF.Sin(radians);
            float t = 1f - c;

            Mat3 m = Identity;

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

        public static Mat3 Transpose(Mat3 m)
        {
            Mat3 r = new();
            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    r[row, col] = m[col, row];
            return r;
        }

        public Mat3 Transposed() => Transpose(this);

        public float Determinant
        {
            get
            {
                float a = this[0, 0], b = this[0, 1], c = this[0, 2];
                float d = this[1, 0], e = this[1, 1], f = this[1, 2];
                float g = this[2, 0], h = this[2, 1], i = this[2, 2];

                return a * (e * i - f * h)
                     - b * (d * i - f * g)
                     + c * (d * h - e * g);
            }
        }

        public static bool Invert(Mat3 m, out Mat3 inv)
        {
            float a = m[0, 0], b = m[0, 1], c = m[0, 2];
            float d = m[1, 0], e = m[1, 1], f = m[1, 2];
            float g = m[2, 0], h = m[2, 1], i = m[2, 2];

            float det = a * (e * i - f * h)
                      - b * (d * i - f * g)
                      + c * (d * h - e * g);

            if (MathF.Abs(det) < 1e-8f)
            {
                inv = Identity;
                return false;
            }

            float invDet = 1f / det;

            inv = new Mat3();

            inv[0, 0] = (e * i - f * h) * invDet;
            inv[0, 1] = (c * h - b * i) * invDet;
            inv[0, 2] = (b * f - c * e) * invDet;

            inv[1, 0] = (f * g - d * i) * invDet;
            inv[1, 1] = (a * i - c * g) * invDet;
            inv[1, 2] = (c * d - a * f) * invDet;

            inv[2, 0] = (d * h - e * g) * invDet;
            inv[2, 1] = (b * g - a * h) * invDet;
            inv[2, 2] = (a * e - b * d) * invDet;

            return true;
        }

        public Mat3 Inverted()
        {
            Invert(this, out var inv);
            return inv;
        }

        public static bool operator ==(Mat3 a, Mat3 b)
            => a.M00 == b.M00 && a.M10 == b.M10 && a.M20 == b.M20
            && a.M01 == b.M01 && a.M11 == b.M11 && a.M21 == b.M21
            && a.M02 == b.M02 && a.M12 == b.M12 && a.M22 == b.M22;

        public static bool operator !=(Mat3 a, Mat3 b) => !(a == b);

        public bool Equals(Mat3 other) => this == other;
        public override bool Equals(object? obj) => obj is Mat3 other && Equals(other);

        public override int GetHashCode()
        {
            var hc = new HashCode();
            hc.Add(M00); hc.Add(M10); hc.Add(M20);
            hc.Add(M01); hc.Add(M11); hc.Add(M21);
            hc.Add(M02); hc.Add(M12); hc.Add(M22);
            return hc.ToHashCode();
        }

        public override string ToString()
            => $"[{this[0, 0]:0.###} {this[0, 1]:0.###} {this[0, 2]:0.###}; " +
               $"{this[1, 0]:0.###} {this[1, 1]:0.###} {this[1, 2]:0.###}; " +
               $"{this[2, 0]:0.###} {this[2, 1]:0.###} {this[2, 2]:0.###}]";

        // Column-major float[9] for glUniformMatrix3fv with transpose=false
        public void ToColumnMajorArray(float[] dst9)
        {
            if (dst9 is null || dst9.Length < 9) throw new ArgumentException("dst9 must have length >= 9");

            dst9[0] = M00; dst9[1] = M10; dst9[2] = M20;
            dst9[3] = M01; dst9[4] = M11; dst9[5] = M21;
            dst9[6] = M02; dst9[7] = M12; dst9[8] = M22;
        }
    }
}