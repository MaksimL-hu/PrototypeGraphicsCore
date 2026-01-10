using MyMath;
using static PrototypeGraphicsCore.UnitTests.TestUtil;

namespace PrototypeGraphicsCore.UnitTests;

public class Vec3Tests
{
    [Fact]
    public void CtorSetsFields()
    {
        var v = new Vec3(1.25f, -2.5f, 3.75f);
        Near(1.25f, v.X);
        Near(-2.5f, v.Y);
        Near(3.75f, v.Z);
    }

    [Fact]
    public void StaticVectorsAreCorrect()
    {
        Near(new Vec3(0, 0, 0), Vec3.Zero);
        Near(new Vec3(1, 1, 1), Vec3.One);

        Near(new Vec3(1, 0, 0), Vec3.UnitX);
        Near(new Vec3(0, 1, 0), Vec3.UnitY);
        Near(new Vec3(0, 0, 1), Vec3.UnitZ);
    }

    [Fact]
    public void AddSubNegMulDivWork()
    {
        var a = new Vec3(1, 2, 3);
        var b = new Vec3(10, -2, 0.5f);

        Near(new Vec3(11, 0, 3.5f), a + b);
        Near(new Vec3(-9, 4, 2.5f), a - b);
        Near(new Vec3(-1, -2, -3), -a);

        Near(new Vec3(2, 4, 6), a * 2);
        Near(new Vec3(0.5f, 1f, 1.5f), a / 2);
    }

    [Fact]
    public void LengthAndLengthSquaredWork()
    {
        var v = new Vec3(3, 0, 4);
        Near(25f, v.LengthSquared);
        Near(5f, v.Length);
    }

    [Fact]
    public void LengthSquaredMatchesDotSelf()
    {
        var v = new Vec3(1.5f, -2f, 0.25f);
        Near(v.LengthSquared, Vec3.Dot(v, v));
    }

    [Fact]
    public void DotIsCommutative()
    {
        var a = new Vec3(1.2f, -3.4f, 5.6f);
        var b = new Vec3(-7.8f, 9.0f, -1.1f);
        Near(Vec3.Dot(a, b), Vec3.Dot(b, a));
    }

    [Fact]
    public void CrossBasicAxesWork()
    {
        var x = Vec3.UnitX;
        var y = Vec3.UnitY;
        var z = Vec3.UnitZ;

        Near(z, Vec3.Cross(x, y));
        Near(x, Vec3.Cross(y, z));
        Near(y, Vec3.Cross(z, x));
    }

    [Fact]
    public void CrossIsOrthogonalToInputs()
    {
        var a = new Vec3(1, 2, 3);
        var b = new Vec3(-4, 5, 2);
        var c = Vec3.Cross(a, b);

        Near(0f, Vec3.Dot(c, a));
        Near(0f, Vec3.Dot(c, b));
    }

    [Fact]
    public void NormalizeAndNormalizedWork()
    {
        var v = new Vec3(3, 0, 4);
        var n = v.Normalized();

        Near(1f, n.Length);
        Near(new Vec3(0.6f, 0, 0.8f), n);

        v.Normalize();
        Near(new Vec3(0.6f, 0, 0.8f), v);
    }

    [Fact]
    public void NormalizeZeroStaysZero()
    {
        var v = Vec3.Zero;
        v.Normalize();
        Near(Vec3.Zero, v);
        Near(Vec3.Zero, Vec3.Zero.Normalized());
    }

    [Fact]
    public void NormalizedDoesNotChangeOriginal()
    {
        var v = new Vec3(10, 0, 0);
        var n = v.Normalized();

        Near(new Vec3(10, 0, 0), v);
        Near(new Vec3(1, 0, 0), n);
    }
}