using MyMath;
using static PrototypeGraphicsCore.UnitTests.TestUtil;

namespace PrototypeGraphicsCore.UnitTests;

public class Vec2Tests
{
    [Fact]
    public void CtorSetsFields()
    {
        var v = new Vec2(1.25f, -2.5f);
        Near(1.25f, v.X);
        Near(-2.5f, v.Y);
    }

    [Fact]
    public void AddSubMulDivWork()
    {
        var a = new Vec2(1, 2);
        var b = new Vec2(10, -3);

        Near(new Vec2(11, -1), a + b);
        Near(new Vec2(-9, 5), a - b);

        Near(new Vec2(2, 4), a * 2);
        Near(new Vec2(0.5f, 1f), a / 2);
    }

    [Fact]
    public void LengthAndLengthSquaredWork()
    {
        var v = new Vec2(3, 4);
        Near(25f, v.LengthSquared);
        Near(5f, v.Length);
    }

    [Fact]
    public void LengthSquaredMatchesDotSelf()
    {
        var v = new Vec2(1.5f, -2f);
        Near(v.LengthSquared, Vec2.Dot(v, v));
    }

    [Fact]
    public void DotIsCommutative()
    {
        var a = new Vec2(1.2f, -3.4f);
        var b = new Vec2(-5.6f, 7.8f);

        Near(Vec2.Dot(a, b), Vec2.Dot(b, a));
    }

    [Fact]
    public void NormalizedHasUnitLength()
    {
        var v = new Vec2(3, 4);
        var n = v.Normalized();

        Near(1f, n.Length, 1e-5f);
        Near(new Vec2(0.6f, 0.8f), n);
    }

    [Fact]
    public void NormalizedZeroReturnsZero()
    {
        var z = new Vec2(0, 0);
        Near(new Vec2(0, 0), z.Normalized());
    }

    [Fact]
    public void NormalizedTinyVectorReturnsZeroOrStable()
    {
        var v = new Vec2(1e-12f, -1e-12f);
        var n = v.Normalized();

        Assert.False(float.IsNaN(n.X) || float.IsInfinity(n.X));
        Assert.False(float.IsNaN(n.Y) || float.IsInfinity(n.Y));

        Assert.True(n.Length <= 1.00001f);
    }
}