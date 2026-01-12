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
            public const string Shininess = "uShininess";
            public const string ViewPos = "uViewPos";

            public const string LightPos0 = "uLightPos[0]";
            public const string LightPos1 = "uLightPos[1]";
            public const string LightColor0 = "uLightColor[0]";
            public const string LightColor1 = "uLightColor[1]";
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
