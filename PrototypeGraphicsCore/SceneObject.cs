using MyMath;

namespace PrototypeGraphicsCore
{
    public sealed class SceneObject
    {
        public Mesh Mesh = null!;
        public Vec3 Color;
        public Vec3 BasePosition;
        public float Scale;

        public bool SpinEnabled;
        public Vec3 SpinAxis;
        public float SpinSpeed;
    }
}