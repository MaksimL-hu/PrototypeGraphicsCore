namespace Config
{
    public static class ShaderNames
    {
        public static class Phong
        {
            public const string Model = "uModel";
            public const string View = "uView";
            public const string Projection = "uProjection";

            public const string ObjectColor = "uObjectColor";
            public const string LightPos = "uLightPos";
            public const string LightColor = "uLightColor";

            public const string ViewPos = "uViewPos";
            public const string Shininess = "uShininess";
        }

        public static class Lamp
        {
            public const string Model = "uModel";
            public const string View = "uView";
            public const string Projection = "uProjection";

            public const string Color = "uColor";
            public const string Alpha = "uAlpha";
        }

        public static class Glow
        {
            public const string LightScreenPx = "uLightScreenPx";
            public const string RadiusPx = "uRadiusPx";
            public const string Color = "uColor";
            public const string Intensity = "uIntensity";
        }
    }
}
