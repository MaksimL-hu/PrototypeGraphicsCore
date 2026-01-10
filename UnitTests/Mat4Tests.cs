using System;
using MyMath;
using Xunit;
using static PrototypeGraphicsCore.UnitTests.TestUtil;

namespace PrototypeGraphicsCore.UnitTests;

public class Mat4Tests
{
    [Fact]
    public void IdentityHasCorrectDiagonal()
    {
        var m = Mat4.Identity;

        for (int r = 0; r < 4; r++)
            for (int c = 0; c < 4; c++)
            {
                float expected = (r == c) ? 1f : 0f;
                Near(expected, m[r, c], 0f);
            }
    }

    [Fact]
    public void IndexerGetSetWork()
    {
        var m = Mat4.Identity;

        m[0, 3] = 10f;   // tx
        m[2, 1] = -7.5f; // произвольный элемент
        m[3, 0] = 0.25f;

        Near(10f, m[0, 3], 0f);
        Near(-7.5f, m[2, 1], 0f);
        Near(0.25f, m[3, 0], 0f);

        Assert.Throws<IndexOutOfRangeException>(() => _ = m[-1, 0]);
        Assert.Throws<IndexOutOfRangeException>(() => _ = m[0, -1]);
        Assert.Throws<IndexOutOfRangeException>(() => _ = m[4, 0]);
        Assert.Throws<IndexOutOfRangeException>(() => _ = m[0, 4]);
    }

    [Fact]
    public void ToColumnMajorArrayMatchesIndexer()
    {
        // Проверяем правило: arr[col*4 + row] == m[row,col]
        var m = new Mat4();
        for (int r = 0; r < 4; r++)
            for (int c = 0; c < 4; c++)
                m[r, c] = 10f * r + c; // уникальные значения

        var a = new float[16];
        m.ToColumnMajorArray(a);

        for (int r = 0; r < 4; r++)
            for (int c = 0; c < 4; c++)
            {
                float expected = m[r, c];
                float actual = a[c * 4 + r];
                Near(expected, actual, 0f);
            }
    }

    [Fact]
    public void IdentityMultiplyIsNeutral()
    {
        var a = Mat4.CreateTranslation(new Vec3(1, 2, 3)) * Mat4.CreateScale(new Vec3(2, 3, 4));

        Near(a, Mat4.Identity * a);
        Near(a, a * Mat4.Identity);
    }

    [Fact]
    public void MatrixMultiplyAssociativityHoldsOnVector()
    {
        var a = Mat4.CreateTranslation(new Vec3(1, 2, 3));
        var b = Mat4.CreateRotationY(0.7f);
        var c = Mat4.CreateScale(new Vec3(2, 3, 4));

        var v = new Vec4(new Vec3(1.25f, -2f, 0.5f), 1f);

        var left = (a * b * c) * v;
        var right = a * (b * (c * v));

        Near(left, right, 1e-4f);
    }

    [Fact]
    public void TranslationMovesPointButNotDirection()
    {
        var t = Mat4.CreateTranslation(new Vec3(10, -2, 5));

        var p = new Vec4(new Vec3(1, 2, 3), 1f); // point
        var d = new Vec4(new Vec3(1, 2, 3), 0f); // direction

        Near(new Vec4(11, 0, 8, 1), t * p, 1e-6f);
        Near(new Vec4(1, 2, 3, 0), t * d, 1e-6f);
    }

    [Fact]
    public void ScaleFloatScalesXYZ()
    {
        var s = Mat4.CreateScale(2f);
        var p = new Vec4(new Vec3(1, -2, 3), 1f);

        Near(new Vec4(2, -4, 6, 1), s * p, 1e-6f);
    }

    [Fact]
    public void ScaleVec3ScalesPerAxis()
    {
        var s = Mat4.CreateScale(new Vec3(2, 3, 4));
        var p = new Vec4(new Vec3(1, -2, 3), 1f);

        Near(new Vec4(2, -6, 12, 1), s * p, 1e-6f);
    }

    [Fact]
    public void RotationX90RotatesYToZ()
    {
        var r = Mat4.CreateRotationX(MathF.PI * 0.5f);
        var v = new Vec4(new Vec3(0, 1, 0), 0f);

        Near(new Vec4(0, 0, 1, 0), r * v, 1e-4f);
    }

    [Fact]
    public void RotationY90RotatesXToMinusZ()
    {
        var r = Mat4.CreateRotationY(MathF.PI * 0.5f);
        var v = new Vec4(new Vec3(1, 0, 0), 0f);

        // по твоей реализации CreateRotationY: (1,0,0) -> (0,0,-1)
        Near(new Vec4(0, 0, -1, 0), r * v, 1e-4f);
    }

    [Fact]
    public void RotationZ90RotatesXToY()
    {
        var r = Mat4.CreateRotationZ(MathF.PI * 0.5f);
        var v = new Vec4(new Vec3(1, 0, 0), 0f);

        Near(new Vec4(0, 1, 0, 0), r * v, 1e-4f);
    }

    [Fact]
    public void RotationPreservesLengthForDirections()
    {
        var r = Mat4.CreateFromAxisAngle(new Vec3(1, 2, 3), 1.234f);
        var v = new Vec4(new Vec3(1.25f, -2f, 0.5f), 0f); // direction

        float len0 = v.Xyz.Length;
        float len1 = (r * v).Xyz.Length;

        Near(len0, len1, 1e-4f);
    }

    [Fact]
    public void AxisAngleMatchesRotationYOnVector()
    {
        float a = 0.9f;

        var ry = Mat4.CreateRotationY(a);
        var aa = Mat4.CreateFromAxisAngle(Vec3.UnitY, a);

        var v = new Vec4(new Vec3(1, 2, 3), 0f);
        Near(ry * v, aa * v, 1e-4f);
    }

    [Fact]
    public void LookAtMovesEyeToOrigin()
    {
        var eye = new Vec3(5, 2, 10);
        var target = new Vec3(5, 2, 9); // смотрим примерно вдоль -Z
        var up = Vec3.UnitY;

        var view = Mat4.LookAt(eye, target, up);

        var eyeView = view * new Vec4(eye, 1f);
        Near(new Vec4(0, 0, 0, 1), eyeView, 1e-4f);
    }

    [Fact]
    public void LookAtDefaultIsIdentity()
    {
        var view = Mat4.LookAt(new Vec3(0, 0, 0), new Vec3(0, 0, -1), Vec3.UnitY);
        Near(Mat4.Identity, view, 1e-4f);
    }

    [Fact]
    public void LookAtBasisIsOrthonormal()
    {
        var eye = new Vec3(1, 2, 3);
        var target = new Vec3(-4, 1, 0);
        var up = Vec3.UnitY;

        var m = Mat4.LookAt(eye, target, up);

        Vec3 s = new Vec3(m[0, 0], m[1, 0], m[2, 0]);
        Vec3 u = new Vec3(m[0, 1], m[1, 1], m[2, 1]);
        Vec3 fneg = new Vec3(m[0, 2], m[1, 2], m[2, 2]);

        Near(1f, s.Length, 1e-4f);
        Near(1f, u.Length, 1e-4f);
        Near(1f, fneg.Length, 1e-4f);

        Near(0f, Vec3.Dot(s, u), 1e-4f);
        Near(0f, Vec3.Dot(s, fneg), 1e-4f);
        Near(0f, Vec3.Dot(u, fneg), 1e-4f);
    }

    [Fact]
    public void PerspectiveHasExpectedFixedCells()
    {
        var p = Mat4.CreatePerspectiveFieldOfView(
            fovYRadians: MathF.PI / 2f,
            aspect: 16f / 9f,
            zNear: 0.1f,
            zFar: 100f
        );

        // из твоей реализации:
        Near(-1f, p[3, 2], 0f);
        Near(0f, p[3, 3], 0f);

        // типичные знаки для z-части при near<far
        Assert.True(p[2, 2] < 0f);
        Assert.True(p[2, 3] < 0f);
    }

    [Fact]
    public void PerspectiveProjectsNearAndFarToNdcRangeRoughly()
    {
        // Этот тест проверяет “в целом корректно”, без привязки к конкретной handness.
        float near = 0.1f;
        float far = 100f;

        var p = Mat4.CreatePerspectiveFieldOfView(
            fovYRadians: MathF.PI / 3f,
            aspect: 1.0f,
            zNear: near,
            zFar: far
        );

        // Точки на оси -Z в view space (как обычно в твоём пайплайне)
        var vNear = new Vec4(0, 0, -near, 1f);
        var vFar = new Vec4(0, 0, -far, 1f);

        Vec4 cNear = p * vNear;
        Vec4 cFar = p * vFar;

        Assert.True(MathF.Abs(cNear.W) > 1e-8f);
        Assert.True(MathF.Abs(cFar.W) > 1e-8f);

        float zNearNdc = cNear.Z / cNear.W;
        float zFarNdc = cFar.Z / cFar.W;

        // В OpenGL NDC z обычно в [-1..1]. Не требуем точных границ, просто “в диапазоне” и упорядоченность.
        Assert.True(zNearNdc >= -1.5f && zNearNdc <= 1.5f);
        Assert.True(zFarNdc >= -1.5f && zFarNdc <= 1.5f);

        // near должен быть ближе, чем far (по NDC обычно near < far или наоборот — зависит от матрицы,
        // но они не должны совпасть).
        Assert.True(MathF.Abs(zNearNdc - zFarNdc) > 1e-4f);
    }
}
