using MyMath;
using System;

namespace Config
{
    internal static class AppConfig
    {
        // ---------- Materials ----------
        public const float MaterialShininess = 64f;

        // ---------- Geometry / mesh quality ----------
        public const int SphereStacks = 32;
        public const int SphereSlices = 48;

        public const float TorusMajorRadius = 1.1f;
        public const float TorusMinorRadius = 0.35f;
        public const int TorusMajorSegments = 64;
        public const int TorusMinorSegments = 32;

        public const int LampSphereStacks = 12;
        public const int LampSphereSlices = 18;

        // Window / GL
        public const string Title = "OpenTK 3D Scene: Cube / Sphere / Pyramid / Torus";
        public const int Width = 1280;
        public const int Height = 720;
        public static readonly Version GLVersion = new Version(3, 3);

        // Camera
        public const float FovDeg = 60f;
        public const float MoveSpeed = 5.0f;
        public const float MouseSensitivity = 0.12f;

        // ---------- Camera start / limits ----------
        public static readonly Vec3 CameraStartPos = new Vec3(0f, 3.0f, 9.0f);
        public const float CameraStartYaw = 0f;
        public const float CameraStartPitch = 0f;

        public const float PitchMinDeg = -89f;
        public const float PitchMaxDeg = 89f;

        public const float FovMinDeg = 20f;
        public const float FovMaxDeg = 90f;
        public const float FovWheelStepDeg = 2.0f;

        public const float SprintMultiplier = 2.0f;

        // Projection
        public const float ZNear = 0.1f;
        public const float ZFar = 200f;

        // Scene
        public static readonly Vec3 ClearColor = new Vec3(0.07f, 0.08f, 0.10f);
        public const float ObjectsCircleRadius = 3.2f;

        // Light
        public static readonly Vec3 LightPos = new Vec3(0f, 2.0f, 0f);
        public static readonly Vec3 LightColor = Vec3.One;

        // Lamp (3D)
        public const float LampCoreRadius = 0.10f;
        public static readonly Vec3 LampCoreColor = new Vec3(1.0f, 0.98f, 0.85f);
        public const float LampHaloRadius = 0.28f;
        public const float LampAlpha = 1f;

        // ---------- Lamp halo layers ----------
        public static readonly float[] LampHaloScales = { 1.00f, 1.75f, 2.60f };
        public static readonly Vec3[] LampHaloColors =
        {
            new Vec3(1.0f, 0.95f, 0.65f),
            new Vec3(1.0f, 0.85f, 0.45f),
            new Vec3(1.0f, 0.75f, 0.35f),
        };
        public static readonly float[] LampHaloAlphas = { 0.07f, 0.03f, 0.015f };

        // Glow (screen-space)
        public const float GlowBaseRadiusPx = 90f;
        public static readonly Vec3 GlowColor = new Vec3(1.0f, 0.9f, 0.55f);

        // Occlusion tuning (ReadPixels depth compare)
        public const float GlowDepthEps = 0.0030f;
        public const float GlowVisibleThreshold = 0.01f;

        // Glow distance response
        public const float GlowMinRadiusPx = 25f;
        public const float GlowMaxRadiusPx = 160f;

        public const float GlowMinIntensity = 0.15f;
        public const float GlowMaxIntensity = 1.0f;

        // Radius = BaseRadius / (A + B * viewZ)
        public const float GlowRadiusA = 0.35f;
        public const float GlowRadiusB = 0.12f;

        // Intensity = BaseIntensity / (C + D * viewZ)
        public const float GlowBaseIntensity = 0.95f;
        public const float GlowIntensityC = 0.60f;
        public const float GlowIntensityD = 0.15f;

        // Occlusion sample offsets (px)
        public const int GlowSampleOffsetPx = 4;

        // NDC bounds (skip when far off-screen)
        public const float GlowNdcCull = 1.2f;

        // Avoid div by 0 / behind camera
        public const float GlowMinViewZ = 0.15f;

        // Objects
        public static readonly Vec3[] ObjectColors =
        {
            new Vec3(0.95f, 0.35f, 0.25f),
            new Vec3(0.25f, 0.70f, 0.95f),
            new Vec3(0.55f, 0.95f, 0.40f),
            new Vec3(0.95f, 0.90f, 0.30f),
        };

        public static readonly float[] ObjectScales = { 0.9f, 1.0f, 1.0f, 1.0f };

        public static readonly bool[] ObjectSpinEnabled = { true, false, true, true };
        public static readonly Vec3[] ObjectSpinAxis = { Vec3.UnitY, Vec3.UnitY, Vec3.UnitY, Vec3.UnitX };
        public static readonly float[] ObjectSpinSpeed = { 1.0f, 0.0f, 0.8f, 1.6f };

        // Shaders root
        public const string ShadersDir = "data/shaders";

        // Shader files
        public static readonly string PhongVert = $"{ShadersDir}/phong.vert";
        public static readonly string PhongFrag = $"{ShadersDir}/phong.frag";

        public static readonly string LampVert = $"{ShadersDir}/lamp.vert";
        public static readonly string LampFrag = $"{ShadersDir}/lamp.frag";

        public static readonly string GlowVert = $"{ShadersDir}/glow.vert";
        public static readonly string GlowFrag = $"{ShadersDir}/glow.frag";
    }
}