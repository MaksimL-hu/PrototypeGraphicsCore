using System;
using System.Collections.Generic;
using MyMath;

namespace PrototypeGraphicsCore
{
    public static class Primitives
    {
        private const float TwoPi = MyMath.MathConstants.TwoPi;

        // Vertex format: position (x,y,z) + normal (nx,ny,nz)
        private static void AddVertex(List<float> verts, in Vec3 p, in Vec3 n)
        {
            verts.Add(p.X); verts.Add(p.Y); verts.Add(p.Z);
            verts.Add(n.X); verts.Add(n.Y); verts.Add(n.Z);
        }

        private static void AddVertex(List<float> verts, float px, float py, float pz, float nx, float ny, float nz)
        {
            verts.Add(px); verts.Add(py); verts.Add(pz);
            verts.Add(nx); verts.Add(ny); verts.Add(nz);
        }

        public static Mesh CreateCube()
        {
            // 24 unique vertices (4 per face) so normals are flat per-face.
            float[] v =
            [
                // +Z
                -0.5f,-0.5f, 0.5f,  0,0,1,
                 0.5f,-0.5f, 0.5f,  0,0,1,
                 0.5f, 0.5f, 0.5f,  0,0,1,
                -0.5f, 0.5f, 0.5f,  0,0,1,

                // -Z
                 0.5f,-0.5f,-0.5f,  0,0,-1,
                -0.5f,-0.5f,-0.5f,  0,0,-1,
                -0.5f, 0.5f,-0.5f,  0,0,-1,
                 0.5f, 0.5f,-0.5f,  0,0,-1,

                // +X
                 0.5f,-0.5f, 0.5f,  1,0,0,
                 0.5f,-0.5f,-0.5f,  1,0,0,
                 0.5f, 0.5f,-0.5f,  1,0,0,
                 0.5f, 0.5f, 0.5f,  1,0,0,

                // -X
                -0.5f,-0.5f,-0.5f, -1,0,0,
                -0.5f,-0.5f, 0.5f, -1,0,0,
                -0.5f, 0.5f, 0.5f, -1,0,0,
                -0.5f, 0.5f,-0.5f, -1,0,0,

                // +Y
                -0.5f, 0.5f, 0.5f,  0,1,0,
                 0.5f, 0.5f, 0.5f,  0,1,0,
                 0.5f, 0.5f,-0.5f,  0,1,0,
                -0.5f, 0.5f,-0.5f,  0,1,0,

                // -Y
                -0.5f,-0.5f,-0.5f,  0,-1,0,
                 0.5f,-0.5f,-0.5f,  0,-1,0,
                 0.5f,-0.5f, 0.5f,  0,-1,0,
                -0.5f,-0.5f, 0.5f,  0,-1,0,
            ];

            uint[] idx =
            [
                0,1,2,  2,3,0,
                4,5,6,  6,7,4,
                8,9,10, 10,11,8,
                12,13,14, 14,15,12,
                16,17,18, 18,19,16,
                20,21,22, 22,23,20
            ];

            return new Mesh(v, idx);
        }

        public static Mesh CreatePyramid()
        {
            Vec3 p0 = new(-0.5f, -0.5f, 0.5f);
            Vec3 p1 = new(0.5f, -0.5f, 0.5f);
            Vec3 p2 = new(0.5f, -0.5f, -0.5f);
            Vec3 p3 = new(-0.5f, -0.5f, -0.5f);
            Vec3 top = new(0.0f, 0.6f, 0.0f);

            static Vec3 FaceNormal(in Vec3 a, in Vec3 b, in Vec3 c)
            {
                var n = Vec3.Cross(b - a, c - a);
                n.Normalize();
                return n;
            }

            // 6 faces * 2 triangles (base + 4 sides) = 6 triangles? actually:
            // sides: 4 tris, base: 2 tris => total 6 tris => 18 vertices
            var verts = new List<float>(18 * 6);
            var inds = new List<uint>(18);

            uint baseIndex = 0;

            void AddTri(in Vec3 a, in Vec3 b, in Vec3 c, in Vec3 n)
            {
                AddVertex(verts, a, n);
                AddVertex(verts, b, n);
                AddVertex(verts, c, n);

                inds.Add(baseIndex + 0);
                inds.Add(baseIndex + 1);
                inds.Add(baseIndex + 2);
                baseIndex += 3;
            }

            // sides (flat)
            AddTri(p0, p1, top, FaceNormal(p0, p1, top));
            AddTri(p1, p2, top, FaceNormal(p1, p2, top));
            AddTri(p2, p3, top, FaceNormal(p2, p3, top));
            AddTri(p3, p0, top, FaceNormal(p3, p0, top));

            // base (flat down)
            Vec3 nDown = new(0f, -1f, 0f);
            AddTri(p2, p1, p0, nDown);
            AddTri(p0, p3, p2, nDown);

            return new Mesh(verts.ToArray(), inds.ToArray());
        }

        public static Mesh CreateSphere(int stacks, int slices)
        {
            if (stacks < 2) throw new ArgumentOutOfRangeException(nameof(stacks), "stacks must be >= 2");
            if (slices < 3) throw new ArgumentOutOfRangeException(nameof(slices), "slices must be >= 3");

            int vertCount = (stacks + 1) * (slices + 1);
            int indexCount = stacks * slices * 6;

            var verts = new List<float>(vertCount * 6);
            var inds = new List<uint>(indexCount);

            // vertices
            for (int stack = 0; stack <= stacks; stack++)
            {
                float v = stack / (float)stacks;
                float phi = MathF.PI * v;

                float y = MathF.Cos(phi);
                float r = MathF.Sin(phi);

                for (int slice = 0; slice <= slices; slice++)
                {
                    float u = slice / (float)slices;
                    float theta = TwoPi * u;

                    float x = r * MathF.Cos(theta);
                    float z = r * MathF.Sin(theta);

                    // sphere of radius 1: normal = position
                    AddVertex(verts, x, y, z, x, y, z);
                }
            }

            // indices
            int stride = slices + 1;
            for (int stack = 0; stack < stacks; stack++)
            {
                int row0 = stack * stride;
                int row1 = (stack + 1) * stride;

                for (int slice = 0; slice < slices; slice++)
                {
                    uint a = (uint)(row0 + slice);
                    uint b = (uint)(row1 + slice);
                    uint c = (uint)(row1 + (slice + 1));
                    uint d = (uint)(row0 + (slice + 1));

                    // keep winding consistent with your other meshes
                    inds.Add(a); inds.Add(b); inds.Add(d);
                    inds.Add(d); inds.Add(b); inds.Add(c);
                }
            }

            return new Mesh(verts.ToArray(), inds.ToArray());
        }

        public static Mesh CreateTorus(float majorRadius, float minorRadius, int majorSegments, int minorSegments)
        {
            if (majorRadius <= 0f) throw new ArgumentOutOfRangeException(nameof(majorRadius));
            if (minorRadius <= 0f) throw new ArgumentOutOfRangeException(nameof(minorRadius));
            if (majorSegments < 3) throw new ArgumentOutOfRangeException(nameof(majorSegments), "majorSegments must be >= 3");
            if (minorSegments < 3) throw new ArgumentOutOfRangeException(nameof(minorSegments), "minorSegments must be >= 3");

            int vertCount = (majorSegments + 1) * (minorSegments + 1);
            int indexCount = majorSegments * minorSegments * 6;

            var verts = new List<float>(vertCount * 6);
            var inds = new List<uint>(indexCount);

            // vertices
            for (int i = 0; i <= majorSegments; i++)
            {
                float u = i / (float)majorSegments;
                float a = TwoPi * u;

                float cosA = MathF.Cos(a);
                float sinA = MathF.Sin(a);

                for (int j = 0; j <= minorSegments; j++)
                {
                    float v = j / (float)minorSegments;
                    float b = TwoPi * v;

                    float cosB = MathF.Cos(b);
                    float sinB = MathF.Sin(b);

                    float cx = majorRadius + minorRadius * cosB; // center + tube offset

                    float x = cx * cosA;
                    float y = minorRadius * sinB;
                    float z = cx * sinA;

                    // For torus, normal is direction from tube center to surface:
                    // (cosB*cosA, sinB, cosB*sinA) already unit length, but safe normalize not needed.
                    float nx = cosB * cosA;
                    float ny = sinB;
                    float nz = cosB * sinA;

                    AddVertex(verts, x, y, z, nx, ny, nz);
                }
            }

            int stride = minorSegments + 1;
            for (int i = 0; i < majorSegments; i++)
            {
                int row0 = i * stride;
                int row1 = (i + 1) * stride;

                for (int j = 0; j < minorSegments; j++)
                {
                    uint a = (uint)(row0 + j);
                    uint b = (uint)(row1 + j);
                    uint c = (uint)(row1 + (j + 1));
                    uint d = (uint)(row0 + (j + 1));

                    inds.Add(a); inds.Add(b); inds.Add(d);
                    inds.Add(d); inds.Add(b); inds.Add(c);
                }
            }

            return new Mesh(verts.ToArray(), inds.ToArray());
        }
    }
}