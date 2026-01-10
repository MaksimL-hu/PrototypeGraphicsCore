using MyMath;
using static PrototypeGraphicsCore.UnitTests.TestUtil;

namespace PrototypeGraphicsCore.UnitTests;

public class Vec4Tests
{
    [Fact]
    public void CtorFromVec3SetsComponents()
    {
        var v = new Vec3(1, 2, 3);
        var p = new Vec4(v, 1);

        Near(new Vec4(1, 2, 3, 1), p);
        Near(new Vec3(1, 2, 3), p.Xyz);
    }

    [Fact]
    public void PlusWork()
    {
        var v1 = new Vec4(1, 1, 1, 1);
        var v2 = new Vec4(1, 1, 1, 1);
        Near(new Vec4(2, 2, 2, 2), v1 + v2, 0);
    }

    [Fact]
    public void MinusWork()
    {
        var v1 = new Vec4(2, 2, 2, 2);
        var v2 = new Vec4(1, 1, 1, 1);
        Near(new Vec4(1, 1, 1, 1), v1 - v2, 0);
    }

    [Fact]
    public void MultiplyWork()
    {
        var v = new Vec4(2, 2, 2, 2);
        Near(new Vec4(4, 4, 4, 4), v * 2, 0);
    }

    [Fact]
    public void LerpWork()
    {
        var v1 = Vec4.Zero;
        var v2 = new Vec4(2,2,2,2);
        Near(Vec4.One, Vec4.Lerp(v1, v2, 0.5f), 0);
    }

    [Fact]
    public void DivideWork()
    {
        var v = new Vec4(2, 4, 6, 8);
        Near(new Vec4(1, 2, 3, 4), v / 2, 0);
    }

    [Fact]
    public void TransformColumnIdentityReturnsSame()
    {
        var v = new Vec4(1, 2, 3, 1);
        Near(v, Vec4.TransformColumn(v, Mat4.Identity));
    }

    [Fact]
    public void TransformRowIdentityReturnsSame()
    {
        var v = new Vec4(1, 2, 3, 1);
        Near(v, Vec4.TransformRow(v, Mat4.Identity));
    }

    [Fact]
    public void TransformColumnTranslationMovesPoint()
    {
        var t = Mat4.CreateTranslation(new Vec3(10, 0, -5));
        var p = new Vec4(new Vec3(1, 2, 3), 1);

        Near(new Vec4(11, 2, -2, 1), Vec4.TransformColumn(p, t));
    }

    [Fact]
    public void TransformColumnTranslationDoesNotMoveDirection()
    {
        var t = Mat4.CreateTranslation(new Vec3(10, 0, -5));
        var d = new Vec4(new Vec3(1, 2, 3), 0);

        Near(new Vec4(1, 2, 3, 0), Vec4.TransformColumn(d, t));
    }

    [Fact]
    public void TransformRowMatchesManualRowVectorRule()
    {
        var m = Mat4.Identity;
        m[0, 1] = 2f;

        var v = new Vec4(1, 0, 0, 0);

        var r = Vec4.TransformRow(v, m);
        Near(new Vec4(1, 2, 0, 0), r);
    }
}