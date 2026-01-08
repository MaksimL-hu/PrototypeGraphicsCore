namespace MyMath
{
    public static class MyMath
    {
        public static float DegreesToRadians(float degrees) => degrees * (MathConstants.Pi / 180f);
        public static float RadiansToDegrees(float radians) => radians * (180f / MathConstants.Pi);

        public static float Clamp(float v, float min, float max) => v < min ? min : (v > max ? max : v);
    }
}
