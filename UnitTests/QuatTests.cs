using MyMath;
using static PrototypeGraphicsCore.UnitTests.TestUtil;

namespace PrototypeGraphicsCore.UnitTests;

public class QuatTests
{
    [Fact]
    public void IdentityHasExpectedComponents()
    {
        Near(0f, Quat.Identity.X, 0f);
        Near(0f, Quat.Identity.Y, 0f);
        Near(0f, Quat.Identity.Z, 0f);
        Near(1f, Quat.Identity.W, 0f);
    }

    [Fact]
    public void NormalizedIdentityIsIdentity()
    {
        Near(Quat.Identity, Quat.Identity.Normalized(), 0f);
    }

    [Fact]
    public void NormalizedZeroReturnsIdentity()
    {
        var z = new Quat(0, 0, 0, 0);
        Near(Quat.Identity, z.Normalized(), 0f);
    }

    [Fact]
    public void FromAxisAngleZeroRadiansIsIdentity()
    {
        var q = Quat.FromAxisAngle(Vec3.UnitY, 0f);
        Near(Quat.Identity, q, 1e-6f);
    }

    [Fact]
    public void FromAxisAngleProducesUnitQuaternion()
    {
        var q = Quat.FromAxisAngle(new Vec3(10, -2, 5), 1.2345f);
        Near(1f, q.Length, 1e-5f);
    }

    [Fact]
    public void FromAxisAngleNonNormalizedAxisStillWorks()
    {
        // axis нормализуется внутри FromAxisAngle()
        var q1 = Quat.FromAxisAngle(new Vec3(1, 2, 3), 0.75f);
        var q2 = Quat.FromAxisAngle(new Vec3(10, 20, 30), 0.75f);

        // кватернионы могут отличаться знаком (q и -q эквивалентны),
        // поэтому сравним действие на векторе
        var v = new Vec4(new Vec3(1, -2, 0.5f), 0f);
        var r1 = q1.ToMat4() * v;
        var r2 = q2.ToMat4() * v;

        Near(r1, r2, 1e-4f);
    }

    [Fact]
    public void MultiplyWithIdentityReturnsSame()
    {
        var q = Quat.FromAxisAngle(Vec3.UnitX, 0.9f);

        Near(q, (q * Quat.Identity), 1e-6f);
        Near(q, (Quat.Identity * q), 1e-6f);
    }

    [Fact]
    public void MultiplicationIsAssociative()
    {
        var a = Quat.FromAxisAngle(Vec3.UnitX, 0.3f);
        var b = Quat.FromAxisAngle(Vec3.UnitY, 0.7f);
        var c = Quat.FromAxisAngle(Vec3.UnitZ, -1.1f);

        var left = (a * b) * c;
        var right = a * (b * c);

        Near(left.X, right.X, 1e-5f);
        Near(left.Y, right.Y, 1e-5f);
        Near(left.Z, right.Z, 1e-5f);
        Near(left.W, right.W, 1e-5f);
    }

    [Fact]
    public void ToMat4OfIdentityIsIdentity()
    {
        var m = Quat.Identity.ToMat4();
        Near(Mat4.Identity, m, 1e-6f);
    }

    [Fact]
    public void ToMat4MatchesMat4CreateFromAxisAngleOnVector()
    {
        var axis = new Vec3(0.3f, 1.0f, -0.2f);
        float radians = 1.25f;

        var q = Quat.FromAxisAngle(axis, radians);
        var mq = q.ToMat4();
        var ma = Mat4.CreateFromAxisAngle(axis, radians);

        var v = new Vec4(new Vec3(1, -2, 0.5f), 0f); // direction (w=0)

        Near(ma * v, mq * v, 1e-4f);
    }

    [Fact]
    public void ToMat4RotatesAsExpectedForSimpleCase()
    {
        // 90° вокруг Y: (1,0,0) -> (0,0,-1) по твоей правой системе и CreateRotationY
        var q = Quat.FromAxisAngle(Vec3.UnitY, MathF.PI * 0.5f);
        var m = q.ToMat4();

        var v = new Vec4(new Vec3(1, 0, 0), 0f);
        var r = m * v;

        Near(new Vec4(0, 0, -1, 0), r, 1e-4f);
    }

    [Fact]
    public void RotationPreservesLengthForDirection()
    {
        var q = Quat.FromAxisAngle(new Vec3(1, 2, 3), 0.9f);
        var m = q.ToMat4();

        var v = new Vec4(new Vec3(1.25f, -2f, 0.5f), 0f);
        float len0 = v.Xyz.Length;
        float len1 = (m * v).Xyz.Length;

        Near(len0, len1, 1e-4f);
    }
}