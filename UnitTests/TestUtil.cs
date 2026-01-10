using MyMath;

namespace PrototypeGraphicsCore.UnitTests;

internal static class TestUtil
{
    public const float Eps = 1e-5f;

    public static void Near(float expected, float actual, float eps = Eps)
        => Assert.True(MathF.Abs(expected - actual) <= eps, $"Expected {expected}, got {actual}");

    public static void Near(Quat e, Quat a, float eps = Eps)
    {
        Near(e.X, a.X, eps);
        Near(e.Y, a.Y, eps);
        Near(e.Z, a.Z, eps);
        Near(e.W, a.W, eps);
    }

    public static void Near(Vec2 e, Vec2 a, float eps = Eps)
    {
        Near(e.X, a.X, eps);
        Near(e.Y, a.Y, eps);
    }

    public static void Near(Vec3 e, Vec3 a, float eps = Eps)
    {
        Near(e.X, a.X, eps);
        Near(e.Y, a.Y, eps);
        Near(e.Z, a.Z, eps);
    }

    public static void Near(Vec4 e, Vec4 a, float eps = Eps)
    {
        Near(e.X, a.X, eps);
        Near(e.Y, a.Y, eps);
        Near(e.Z, a.Z, eps);
        Near(e.W, a.W, eps);
    }

    public static void Near(Mat4 e, Mat4 a, float eps = Eps)
    {
        for (int r = 0; r < 4; r++)
            for (int c = 0; c < 4; c++)
                Near(e[r, c], a[r, c], eps);
    }
}