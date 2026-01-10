using MyMath;
using static PrototypeGraphicsCore.UnitTests.TestUtil;

namespace PrototypeGraphicsCore.UnitTests;

public class Mat3AdvancedTests
{
    private static Mat3 MakeNonSingular()
    {
        // Не вырожденная матрица
        var m = Mat3.Identity;
        m[0, 0] = 2; m[0, 1] = 1; m[0, 2] = 3;
        m[1, 0] = 0; m[1, 1] = 4; m[1, 2] = 5;
        m[2, 0] = 1; m[2, 1] = 0; m[2, 2] = 6;
        return m;
    }

    [Fact]
    public void TransposeTwiceReturnsOriginal()
    {
        var m = MakeNonSingular();
        var t = Mat3.Transpose(m);
        var tt = Mat3.Transpose(t);

        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
                Near(m[r, c], tt[r, c], 0);
    }

    [Fact]
    public void DeterminantIdentityIsOne()
    {
        Near(1f, Mat3.Identity.Determinant, 1e-6f);
    }

    [Fact]
    public void InverseMultiplyReturnsIdentity()
    {
        var m = MakeNonSingular();
        var inv = m.Inverted();

        var prod = m * inv;

        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
            {
                float expected = (r == c) ? 1f : 0f;
                Near(expected, prod[r, c]);
            }
    }

    [Fact]
    public void InverseUndoesTransformOnVector()
    {
        var m = MakeNonSingular();
        var inv = m.Inverted();

        var v = new Vec3(1.25f, -2f, 0.5f);
        var v2 = m * v;
        var v3 = inv * v2;

        Near(v, v3, 1e-3f);
    }
}