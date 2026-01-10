using MyMath;
using static PrototypeGraphicsCore.UnitTests.TestUtil;

namespace PrototypeGraphicsCore.UnitTests;

public class Vec2iTests
{
    [Fact]
    public void CtorSetsFields()
    {
        var v = new Vec2i(1, -2);
        Assert.Equal(1, v.X);
        Assert.Equal(-2, v.Y);
    }

    [Fact]
    public void StaticVectorsAreCorrect()
    {
        Assert.Equal(0, Vec2i.Zero.X);
        Assert.Equal(0, Vec2i.Zero.Y);

        Assert.Equal(1, Vec2i.One.X);
        Assert.Equal(1, Vec2i.One.Y);

        Assert.Equal(1, Vec2i.UnitX.X);
        Assert.Equal(0, Vec2i.UnitX.Y);

        Assert.Equal(0, Vec2i.UnitY.X);
        Assert.Equal(1, Vec2i.UnitY.Y);
    }

    [Fact]
    public void EqualityAndInequalityWork()
    {
        var a = new Vec2i(1, 2);
        var b = new Vec2i(1, 2);
        var c = new Vec2i(2, 1);

        Assert.True(a == b);
        Assert.False(a != b);

        Assert.True(a != c);
        Assert.False(a == c);

        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
    }

    [Fact]
    public void HashCodeEqualForEqualValues()
    {
        var a = new Vec2i(10, -7);
        var b = new Vec2i(10, -7);

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void AddSubNegWork()
    {
        var a = new Vec2i(3, 4);
        var b = new Vec2i(-10, 6);

        Assert.Equal(new Vec2i(-7, 10), a + b);
        Assert.Equal(new Vec2i(13, -2), a - b);
        Assert.Equal(new Vec2i(-3, -4), -a);
    }

    [Fact]
    public void ScalarMulDivWork()
    {
        var v = new Vec2i(6, -9);

        Assert.Equal(new Vec2i(12, -18), v * 2);
        Assert.Equal(new Vec2i(12, -18), 2 * v);

        Assert.Equal(new Vec2i(3, -4), v / 2);
    }

    [Fact]
    public void IndexerGetSetWork()
    {
        var v = new Vec2i(7, 8);

        Assert.Equal(7, v[0]);
        Assert.Equal(8, v[1]);

        v[0] = -1;
        v[1] = 99;

        Assert.Equal(-1, v.X);
        Assert.Equal(99, v.Y);

        Assert.Throws<IndexOutOfRangeException>(() => _ = v[-1]);
        Assert.Throws<IndexOutOfRangeException>(() => _ = v[2]);
    }

    [Fact]
    public void LengthSquaredAndLengthWork()
    {
        var v = new Vec2i(3, 4);

        Assert.Equal(25, v.LengthSquared);
        Near(5f, (float)v.Length, 0);
    }

    [Fact]
    public void ToStringStyle()
    {
        var v = new Vec2i(3, -4);
        Assert.Equal("(3, -4)", v.ToString());
    }
}