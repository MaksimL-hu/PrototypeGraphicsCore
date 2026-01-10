using MyMath;
using static PrototypeGraphicsCore.UnitTests.TestUtil;

namespace PrototypeGraphicsCore.UnitTests;

public class Vec3iTests
{
    [Fact]
    public void CtorSetsFields()
    {
        var v = new Vec3i(1, -2, 3);
        Assert.Equal(1, v.X);
        Assert.Equal(-2, v.Y);
        Assert.Equal(3, v.Z);
    }

    [Fact]
    public void StaticVectorsAreCorrect()
    {
        Assert.Equal(new Vec3i(0, 0, 0), Vec3i.Zero);
        Assert.Equal(new Vec3i(1, 1, 1), Vec3i.One);

        Assert.Equal(new Vec3i(1, 0, 0), Vec3i.UnitX);
        Assert.Equal(new Vec3i(0, 1, 0), Vec3i.UnitY);
        Assert.Equal(new Vec3i(0, 0, 1), Vec3i.UnitZ);
    }

    [Fact]
    public void EqualityAndInequalityWork()
    {
        var a = new Vec3i(1, 2, 3);
        var b = new Vec3i(1, 2, 3);
        var c = new Vec3i(1, 2, 4);

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
        var a = new Vec3i(3, 4, 5);
        var b = new Vec3i(-10, 6, 2);

        Assert.Equal(new Vec3i(-7, 10, 7), a + b);
        Assert.Equal(new Vec3i(13, -2, 3), a - b);
        Assert.Equal(new Vec3i(-3, -4, -5), -a);
    }

    [Fact]
    public void ScalarMulDivWork()
    {
        var v = new Vec3i(6, -9, 15);

        Assert.Equal(new Vec3i(12, -18, 30), v * 2);
        Assert.Equal(new Vec3i(12, -18, 30), 2 * v);

        Assert.Equal(new Vec3i(3, -4, 7), v / 2);
    }

    [Fact]
    public void IndexerGetSetWork()
    {
        var v = new Vec3i(7, 8, 9);

        Assert.Equal(7, v[0]);
        Assert.Equal(8, v[1]);
        Assert.Equal(9, v[2]);

        v[0] = -1;
        v[1] = 99;
        v[2] = 5;

        Assert.Equal(-1, v.X);
        Assert.Equal(99, v.Y);
        Assert.Equal(5, v.Z);

        Assert.Throws<IndexOutOfRangeException>(() => _ = v[-1]);
        Assert.Throws<IndexOutOfRangeException>(() => _ = v[3]);
    }

    [Fact]
    public void LengthSquaredAndLengthWork()
    {
        var v = new Vec3i(3, 4, 12);

        Assert.Equal(169, v.LengthSquared);
        Near(13f, (float)v.Length);
    }

    [Fact]
    public void ToStringMatchesOpenTKStyle()
    {
        var v = new Vec3i(3, -4, 5);
        Assert.Equal("(3, -4, 5)", v.ToString());
    }

    [Fact]
    public void HashCodeEqualForEqualValues()
    {
        var a = new Vec3i(10, -7, 42);
        var b = new Vec3i(10, -7, 42);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}