using MyMath;
using static PrototypeGraphicsCore.UnitTests.TestUtil;

namespace PrototypeGraphicsCore.UnitTests;

public class Vec4iTests
{
    [Fact]
    public void CtorSetsFields()
    {
        var v = new Vec4i(1, -2, 3, -4);
        Assert.Equal(1, v.X);
        Assert.Equal(-2, v.Y);
        Assert.Equal(3, v.Z);
        Assert.Equal(-4, v.W);
    }

    [Fact]
    public void StaticVectorsAreCorrect()
    {
        Assert.Equal(new Vec4i(0, 0, 0, 0), Vec4i.Zero);
        Assert.Equal(new Vec4i(1, 1, 1, 1), Vec4i.One);

        Assert.Equal(new Vec4i(1, 0, 0, 0), Vec4i.UnitX);
        Assert.Equal(new Vec4i(0, 1, 0, 0), Vec4i.UnitY);
        Assert.Equal(new Vec4i(0, 0, 1, 0), Vec4i.UnitZ);
        Assert.Equal(new Vec4i(0, 0, 0, 1), Vec4i.UnitW);
    }

    [Fact]
    public void EqualityAndInequalityWork()
    {
        var a = new Vec4i(1, 2, 3, 4);
        var b = new Vec4i(1, 2, 3, 4);
        var c = new Vec4i(1, 2, 3, 5);

        Assert.True(a == b);
        Assert.False(a != b);

        Assert.True(a != c);
        Assert.False(a == c);

        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
    }

    [Fact]
    public void AddSubNegWork()
    {
        var a = new Vec4i(3, 4, 5, 6);
        var b = new Vec4i(-10, 6, 2, -8);

        Assert.Equal(new Vec4i(-7, 10, 7, -2), a + b);
        Assert.Equal(new Vec4i(13, -2, 3, 14), a - b);
        Assert.Equal(new Vec4i(-3, -4, -5, -6), -a);
    }

    [Fact]
    public void ScalarMulDivWork()
    {
        var v = new Vec4i(6, -9, 15, 8);

        Assert.Equal(new Vec4i(12, -18, 30, 16), v * 2);
        Assert.Equal(new Vec4i(12, -18, 30, 16), 2 * v);

        Assert.Equal(new Vec4i(3, -4, 7, 4), v / 2);
    }

    [Fact]
    public void IndexerGetSetWork()
    {
        var v = new Vec4i(7, 8, 9, 10);

        Assert.Equal(7, v[0]);
        Assert.Equal(8, v[1]);
        Assert.Equal(9, v[2]);
        Assert.Equal(10, v[3]);

        v[0] = -1;
        v[1] = 99;
        v[2] = 5;
        v[3] = 123;

        Assert.Equal(-1, v.X);
        Assert.Equal(99, v.Y);
        Assert.Equal(5, v.Z);
        Assert.Equal(123, v.W);

        Assert.Throws<IndexOutOfRangeException>(() => _ = v[-1]);
        Assert.Throws<IndexOutOfRangeException>(() => _ = v[4]);
    }

    [Fact]
    public void LengthSquaredAndLengthWork()
    {
        var v = new Vec4i(1, 2, 2, 1);

        Assert.Equal(10, v.LengthSquared);
        Near(MathF.Sqrt(10f), (float)v.Length);
    }

    [Fact]
    public void ToStringMatchesOpenTKStyle()
    {
        var v = new Vec4i(3, -4, 5, 6);
        Assert.Equal("(3, -4, 5, 6)", v.ToString());
    }

    [Fact]
    public void HashCodeEqualForEqualValues()
    {
        var a = new Vec4i(10, -7, 42, 0);
        var b = new Vec4i(10, -7, 42, 0);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}