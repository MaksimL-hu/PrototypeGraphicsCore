using MyMath;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Config;
using System;
using System.Collections.Generic;
using static MyMath.MathConstants;
using static MyMath.MyMath;

namespace PrototypeGraphicsCore
{
    public sealed class SceneWindow : GameWindow
    {
        private readonly List<SceneObject> _objects = new();
        private readonly float[] _depth1 = new float[1];

        private Shader _shader = null!;
        private Mesh _cube = null!;
        private Mesh _sphere = null!;
        private Mesh _pyramid = null!;
        private Mesh _torus = null!;

        private Mat4 _projection;

        private float _time;
        private float[] _depthBlock = Array.Empty<float>();

        // Light in center
        private float _lampHaloRadius = AppConfig.LampHaloRadius;  // объёмный halo
        private float _lampCoreRadius = AppConfig.LampCoreRadius;
        private Vec3 _lightPos = AppConfig.LightPos;
        private Vec3 _lampCoreColor = AppConfig.LampCoreColor;
        private Shader _lampShader = null!;
        private Mesh _lampMesh = null!;

        // Glow
        private int _glowVao;
        private float _glowRadiusPx = AppConfig.GlowBaseRadiusPx;            // радиус ореола в пикселях
        private Vec3 _glowColor = AppConfig.GlowColor; // цвет (мы ещё дополнительно приглушим)
        private Shader _glowShader = null!;

        // FPS camera
        private Vec3 _worldUp = Vec3.UnitY;
        private Vec3 _cameraFront = -Vec3.UnitZ;
        private Vec3 _cameraUp = Vec3.UnitY;

        private Vec3 _cameraPos = AppConfig.CameraStartPos;
        private float _yaw = AppConfig.CameraStartYaw;
        private float _pitch = AppConfig.CameraStartPitch;

        private float _moveSpeed = AppConfig.MoveSpeed;
        private float _mouseSensitivity = AppConfig.MouseSensitivity;
        private float _fov = AppConfig.FovDeg;

        public SceneWindow(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

        protected override void OnLoad()
        {
            base.OnLoad();

            int o = AppConfig.GlowSampleOffsetPx;
            int size = 2 * o + 1;
            _depthBlock = new float[size * size];

            // --- Debug info ---
            Console.WriteLine("OpenGL: " + GL.GetString(StringName.Version));
            Console.WriteLine("GLSL:   " + GL.GetString(StringName.ShadingLanguageVersion));
            Console.WriteLine("Vendor: " + GL.GetString(StringName.Vendor));
            Console.WriteLine("GPU:    " + GL.GetString(StringName.Renderer));

            // --- GL state ---
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.ClearColor(AppConfig.ClearColor.X, AppConfig.ClearColor.Y, AppConfig.ClearColor.Z, 1.0f);

            // --- Shaders ---
            _shader = Shader.FromFiles(AppConfig.PhongVert, AppConfig.PhongFrag);
            _lampShader = Shader.FromFiles(AppConfig.LampVert, AppConfig.LampFrag);
            _glowShader = Shader.FromFiles(AppConfig.GlowVert, AppConfig.GlowFrag);

            // --- Meshes (scene primitives) ---
            _cube = PrototypeGraphicsCore.Primitives.CreateCube();
            _pyramid = PrototypeGraphicsCore.Primitives.CreatePyramid();
            _sphere = PrototypeGraphicsCore.Primitives.CreateSphere(
                stacks: AppConfig.SphereStacks,
                slices: AppConfig.SphereSlices
            );
            _torus = PrototypeGraphicsCore.Primitives.CreateTorus(
                majorRadius: AppConfig.TorusMajorRadius,
                minorRadius: AppConfig.TorusMinorRadius,
                majorSegments: AppConfig.TorusMajorSegments,
                minorSegments: AppConfig.TorusMinorSegments
            );

            // Lamp mesh (small sphere)
            _lampMesh = PrototypeGraphicsCore.Primitives.CreateSphere(
                stacks: AppConfig.LampSphereStacks,
                slices: AppConfig.LampSphereSlices
            );

            // Glow pass uses gl_VertexID -> only VAO needed
            _glowVao = GL.GenVertexArray();

            // --- Build scene objects around the light ---
            float radius = AppConfig.ObjectsCircleRadius;

            var meshes = new[] { _cube, _sphere, _pyramid, _torus };

            // Safety: config arrays must match object count
            int n = meshes.Length;
            if (AppConfig.ObjectColors.Length != n ||
                AppConfig.ObjectScales.Length != n ||
                AppConfig.ObjectSpinEnabled.Length != n ||
                AppConfig.ObjectSpinAxis.Length != n ||
                AppConfig.ObjectSpinSpeed.Length != n)
            {
                throw new InvalidOperationException("AppConfig object arrays must have the same length as meshes (4).");
            }

            _objects.Clear();

            for (int i = 0; i < n; i++)
            {
                float angle = TwoPi * (i / (float)n);
                Vec3 pos = _lightPos + new Vec3(MathF.Cos(angle) * radius, 0.0f, MathF.Sin(angle) * radius);

                _objects.Add(new SceneObject
                {
                    Mesh = meshes[i],
                    Color = AppConfig.ObjectColors[i],
                    BasePosition = pos,
                    Scale = AppConfig.ObjectScales[i],
                    SpinEnabled = AppConfig.ObjectSpinEnabled[i],
                    SpinAxis = AppConfig.ObjectSpinAxis[i],
                    SpinSpeed = AppConfig.ObjectSpinSpeed[i],
                });
            }

            // --- Input / camera ---
            CursorState = CursorState.Grabbed;

            // Ensures _cameraFront/_cameraUp are consistent at start
            UpdateCameraVectors();
            UpdateProjection();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            UpdateProjection();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            _time += (float)args.Time;

            float dt = (float)args.Time;
            float speed = _moveSpeed * dt;

            if (KeyboardState.IsKeyDown(Keys.LeftShift))
                speed *= AppConfig.SprintMultiplier;

            // FPS-forward: игнорируем pitch по Y, ходим по земле (XZ)
            Vec3 forward = new Vec3(_cameraFront.X, 0f, _cameraFront.Z);
            if (forward.LengthSquared > 1e-8f)
                forward = forward.Normalized();
            else
                forward = -Vec3.UnitZ;

            Vec3 right = Vec3.Cross(forward, _worldUp).Normalized();

            if (KeyboardState.IsKeyDown(Keys.W))
                _cameraPos += forward * speed;
            if (KeyboardState.IsKeyDown(Keys.S))
                _cameraPos -= forward * speed;
            if (KeyboardState.IsKeyDown(Keys.A))
                _cameraPos -= right * speed;
            if (KeyboardState.IsKeyDown(Keys.D))
                _cameraPos += right * speed;

            // (опционально) свободный полёт вверх/вниз:
            if (KeyboardState.IsKeyDown(Keys.Space))
                _cameraPos += _worldUp * speed;
            if (KeyboardState.IsKeyDown(Keys.LeftControl))
                _cameraPos -= _worldUp * speed;
        }


        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            if (CursorState != CursorState.Grabbed)
                return;

            // В Grabbed-режиме правильнее использовать относительное движение мыши
            var d = e.Delta; // OpenTK.Mathematics.Vector2

            _yaw -= d.X * _mouseSensitivity;
            _pitch -= d.Y * _mouseSensitivity;

            _pitch = Clamp(_pitch, AppConfig.PitchMinDeg, AppConfig.PitchMaxDeg);

            UpdateCameraVectors();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _fov -= e.OffsetY * AppConfig.FovWheelStepDeg;
            _fov = Clamp(_fov, AppConfig.FovMinDeg, AppConfig.FovMaxDeg);
            UpdateProjection();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var view = BuildView();

            _shader.Use();
            _shader.SetMatrix4(ShaderNames.Phong.View, view);
            _shader.SetMatrix4(ShaderNames.Phong.Projection, _projection);
            _shader.SetVector3(ShaderNames.Phong.LightPos, _lightPos);
            _shader.SetVector3(ShaderNames.Phong.LightColor, AppConfig.LightColor);
            _shader.SetVector3(ShaderNames.Phong.ViewPos, _cameraPos);

            for (int i = 0; i < _objects.Count; i++)
            {
                var obj = _objects[i];

                Vec3 toLight = (_lightPos - obj.BasePosition);
                float yawToCenter = MathF.Atan2(toLight.X, toLight.Z);

                var spin =
                    obj.SpinEnabled
                    ? Mat4.CreateFromAxisAngle(obj.SpinAxis.Normalized(), _time * obj.SpinSpeed)
                    : Mat4.Identity;

                // Column-vector convention: M = T * R * Spin * S
                var model =
                    Mat4.CreateTranslation(obj.BasePosition) *
                    Mat4.CreateRotationY(yawToCenter) *
                    spin *
                    Mat4.CreateScale(obj.Scale);

                _shader.SetMatrix4(ShaderNames.Phong.Model, model);
                _shader.SetVector3(ShaderNames.Phong.ObjectColor, obj.Color);
                _shader.SetFloat(ShaderNames.Phong.Shininess, AppConfig.MaterialShininess);

                obj.Mesh.Draw();
            }

            // Lamp
            _lampShader.Use();
            _lampShader.SetMatrix4(ShaderNames.Lamp.View, view);
            _lampShader.SetMatrix4(ShaderNames.Lamp.Projection, _projection);

            // 0) core (непрозрачная "лампочка")
            GL.Disable(EnableCap.Blend);
            GL.DepthMask(true);

            {
                var core = Mat4.CreateTranslation(_lightPos) * Mat4.CreateScale(_lampCoreRadius);
                _lampShader.SetMatrix4(ShaderNames.Lamp.Model, core);
                _lampShader.SetVector3(ShaderNames.Lamp.Color, _lampCoreColor);
                _lampShader.SetFloat(ShaderNames.Lamp.Alpha, AppConfig.LampAlpha);
                _lampMesh.Draw();
            }

            // 1..3) halo shells (аддитивные оболочки)
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            GL.DepthMask(false);

            // ореол
            for (int i = 0; i < AppConfig.LampHaloScales.Length; i++)
            {
                float scale = AppConfig.LampHaloScales[i];
                Vec3 color = AppConfig.LampHaloColors[i];
                float alpha = AppConfig.LampHaloAlphas[i];

                var m = Mat4.CreateTranslation(_lightPos) * Mat4.CreateScale(_lampHaloRadius * scale);
                _lampShader.SetMatrix4(ShaderNames.Lamp.Model, m);
                _lampShader.SetVector3(ShaderNames.Lamp.Color, color);
                _lampShader.SetFloat(ShaderNames.Lamp.Alpha, alpha);
                _lampMesh.Draw();
            }

            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);

            DrawGlow(view);

            SwapBuffers();
        }

        private void UpdateCameraVectors()
        {
            float yawRad = DegreesToRadians(_yaw);
            float pitchRad = DegreesToRadians(_pitch);

            // Берём базовое "вперёд" как -Z (как в OpenGL)
            Vec4 forward0 = new Vec4(0f, 0f, -1f, 0f);

            // Камерный поворот: yaw вокруг Y, затем pitch вокруг X
            Mat4 rotY = Mat4.CreateRotationY(yawRad);
            Mat4 rotX = Mat4.CreateRotationX(pitchRad);

            Vec4 f4 = rotY * (rotX * forward0);
            _cameraFront = f4.Xyz.Normalized();

            var right = Vec3.Cross(_cameraFront, _worldUp).Normalized();
            _cameraUp = Vec3.Cross(right, _cameraFront).Normalized();
        }

        private void UpdateProjection()
        {
            _projection = Mat4.CreatePerspectiveFieldOfView(
                DegreesToRadians(_fov),
                Size.X / (float)Size.Y,
                AppConfig.ZNear,
                AppConfig.ZFar
            );
        }

        private Mat4 BuildView()
        {
            float yaw = DegreesToRadians(_yaw);
            float pitch = DegreesToRadians(_pitch);

            // View = R^-1 * T^-1  ->  Rx(-pitch) * Ry(-yaw) * T(-pos)
            Mat4 rotX = Mat4.CreateRotationX(-pitch);
            Mat4 rotY = Mat4.CreateRotationY(-yaw);
            Mat4 trans = Mat4.CreateTranslation(-_cameraPos);

            return rotX * rotY * trans;
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _cube?.Dispose();
            _sphere?.Dispose();
            _pyramid?.Dispose();
            _torus?.Dispose();

            _shader?.Dispose();

            _lampMesh?.Dispose();
            _lampShader?.Dispose();

            if (_glowVao != 0) GL.DeleteVertexArray(_glowVao);
            _glowShader?.Dispose();
        }

        private float ReadDepth01(int x, int y)
        {
            _depth1[0] = 1.0f;
            GL.ReadPixels(x, y, 1, 1, PixelFormat.DepthComponent, PixelType.Float, _depth1);
            return _depth1[0];
        }

        private void DrawGlow(in Mat4 view)
        {
            // --- 1) Project light to screen ---
            Vec4 clip = _projection * (view * new Vec4(_lightPos, 1.0f));
            if (clip.W <= 1e-4f) return;

            Vec3 ndc = clip.Xyz / clip.W;

            float cull = AppConfig.GlowNdcCull;
            if (ndc.X < -cull || ndc.X > cull || ndc.Y < -cull || ndc.Y > cull) return;

            float px = (ndc.X * 0.5f + 0.5f) * Size.X;
            float py = (ndc.Y * 0.5f + 0.5f) * Size.Y;

            float lightDepth01 = ndc.Z * 0.5f + 0.5f;

            // --- 2) Soft occlusion (5 depth taps) ---
            float vis = ComputeGlowVisibilityBlock(px, py, lightDepth01);
            if (vis <= AppConfig.GlowVisibleThreshold) return;

            // --- 3) Distance-based radius & intensity ---
            Vec4 lightView = view * new Vec4(_lightPos, 1.0f);
            float viewZ = MathF.Max(AppConfig.GlowMinViewZ, -lightView.Z);

            float radiusPx = _glowRadiusPx / (AppConfig.GlowRadiusA + AppConfig.GlowRadiusB * viewZ);
            radiusPx = Clamp(radiusPx, AppConfig.GlowMinRadiusPx, AppConfig.GlowMaxRadiusPx);

            float intensity = AppConfig.GlowBaseIntensity / (AppConfig.GlowIntensityC + AppConfig.GlowIntensityD * viewZ);
            intensity = Clamp(intensity, AppConfig.GlowMinIntensity, AppConfig.GlowMaxIntensity) * vis;

            // --- 4) Draw fullscreen tri glow ---
            RenderGlowOverlay(px, py, radiusPx, intensity);
        }

        private void RenderGlowOverlay(float px, float py, float radiusPx, float intensity)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);

            _glowShader.Use();
            _glowShader.SetVector2(ShaderNames.Glow.LightScreenPx, new Vec2(px, py));
            _glowShader.SetFloat(ShaderNames.Glow.RadiusPx, radiusPx);
            _glowShader.SetVector3(ShaderNames.Glow.Color, _glowColor);
            _glowShader.SetFloat(ShaderNames.Glow.Intensity, intensity);

            GL.BindVertexArray(_glowVao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            GL.BindVertexArray(0);

            GL.Disable(EnableCap.Blend);

            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);
        }

        private float ComputeGlowVisibilityBlock(float px, float py, float lightDepth01)
        {
            float eps = AppConfig.GlowDepthEps;
            int o = AppConfig.GlowSampleOffsetPx;

            // целевые пиксели
            int ix = (int)Clamp(px, 0, Size.X - 1);
            int iy = (int)Clamp(py, 0, Size.Y - 1);

            // блок чтения вокруг центра (clamp к экрану)
            int x0 = (int)Clamp(ix - o, 0, Size.X - 1);
            int y0 = (int)Clamp(iy - o, 0, Size.Y - 1);
            int x1 = (int)Clamp(ix + o, 0, Size.X - 1);
            int y1 = (int)Clamp(iy + o, 0, Size.Y - 1);

            int w = x1 - x0 + 1;
            int h = y1 - y0 + 1;

            // один вызов ReadPixels
            GL.ReadPixels(x0, y0, w, h, PixelFormat.DepthComponent, PixelType.Float, _depthBlock);

            float Sample(int sx, int sy)
            {
                // на случай, если точка ушла за границы блока
                sx = (int)Clamp(sx, x0, x1);
                sy = (int)Clamp(sy, y0, y1);

                int lx = sx - x0;
                int ly = sy - y0;
                return _depthBlock[ly * w + lx]; // stride = w (фактическая ширина блока)
            }

            // 5 сэмплов: центр, вправо, влево, вверх, вниз
            float vis = 0f;

            float d0 = Sample(ix, iy);
            if (lightDepth01 <= d0 + eps) vis += 1f;

            float d1 = Sample(ix + o, iy);
            if (lightDepth01 <= d1 + eps) vis += 1f;

            float d2 = Sample(ix - o, iy);
            if (lightDepth01 <= d2 + eps) vis += 1f;

            float d3 = Sample(ix, iy + o);
            if (lightDepth01 <= d3 + eps) vis += 1f;

            float d4 = Sample(ix, iy - o);
            if (lightDepth01 <= d4 + eps) vis += 1f;

            return vis * 0.2f; // /5
        }

    }
}