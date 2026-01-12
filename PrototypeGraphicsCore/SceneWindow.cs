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
        private float[] _depthBlock = [];

        private Shader _shader = null!;
        private Mesh _cube = null!;
        private Mesh _sphere = null!;
        private Mesh _pyramid = null!;
        private Mesh _torus = null!;

        private Mat4 _projection;
        private float _time;

        // --- Two lights ---
        private Vec3 _lightPos0;
        private Vec3 _lightPos1;

        // Lamp rendering (same mesh/shader for both)
        private float _lampHaloRadius = AppConfig.LampHaloRadius;
        private float _lampCoreRadius = AppConfig.LampCoreRadius;
        private Shader _lampShader = null!;
        private Mesh _lampMesh = null!;

        // Glow (same shader for both)
        private int _glowVao;
        private float _glowRadiusPx = AppConfig.GlowBaseRadiusPx;
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

        // fps counter
        private double _fpsTime;
        private int _fpsFrames;
        private double _fpsValue;

        public SceneWindow(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

        protected override void OnLoad()
        {
            base.OnLoad();

            int o = AppConfig.GlowSampleOffsetPx;
            int size = 2 * o + 1;
            _depthBlock = new float[size * size];

            Console.WriteLine("OpenGL: " + GL.GetString(StringName.Version));
            Console.WriteLine("GLSL:   " + GL.GetString(StringName.ShadingLanguageVersion));
            Console.WriteLine("Vendor: " + GL.GetString(StringName.Vendor));
            Console.WriteLine("GPU:    " + GL.GetString(StringName.Renderer));

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.ClearColor(AppConfig.ClearColor.X, AppConfig.ClearColor.Y, AppConfig.ClearColor.Z, 1.0f);

            // Shaders
            _shader = Shader.FromFiles(AppConfig.PhongVert, AppConfig.PhongFrag);
            _lampShader = Shader.FromFiles(AppConfig.LampVert, AppConfig.LampFrag);
            _glowShader = Shader.FromFiles(AppConfig.GlowVert, AppConfig.GlowFrag);

            // Meshes
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

            _lampMesh = PrototypeGraphicsCore.Primitives.CreateSphere(
                stacks: AppConfig.LampSphereStacks,
                slices: AppConfig.LampSphereSlices
            );

            _glowVao = GL.GenVertexArray();

            // Initial lights (t=0)
            UpdateLights();

            // Build scene objects (initially around barycenter)
            float radius = AppConfig.ObjectsCircleRadius;

            var meshes = new[] { _cube, _sphere, _pyramid, _torus };
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

            Vec3 bary = ComputeBarycenter(_lightPos0, _lightPos1);

            for (int i = 0; i < n; i++)
            {
                float angle = TwoPi * (i / (float)n);
                Vec3 pos = bary + new Vec3(MathF.Cos(angle) * radius, 0.0f, MathF.Sin(angle) * radius);

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

            CursorState = CursorState.Grabbed;

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

            float dt = (float)args.Time;
            _time += dt;

            // --- Update lights + object orbits ---
            UpdateLights();
            UpdateObjectsOrbit();

            // --- Camera movement ---
            float speed = _moveSpeed * dt;
            if (KeyboardState.IsKeyDown(Keys.LeftShift))
                speed *= AppConfig.SprintMultiplier;

            // ground movement (XZ)
            Vec3 forward = new Vec3(_cameraFront.X, 0f, _cameraFront.Z);
            if (forward.LengthSquared > 1e-8f) forward = forward.Normalized();
            else forward = -Vec3.UnitZ;

            Vec3 right = Vec3.Cross(forward, _worldUp).Normalized();

            if (KeyboardState.IsKeyDown(Keys.W)) _cameraPos += forward * speed;
            if (KeyboardState.IsKeyDown(Keys.S)) _cameraPos -= forward * speed;
            if (KeyboardState.IsKeyDown(Keys.A)) _cameraPos -= right * speed;
            if (KeyboardState.IsKeyDown(Keys.D)) _cameraPos += right * speed;

            // optional fly up/down
            if (KeyboardState.IsKeyDown(Keys.Space)) _cameraPos += _worldUp * speed;
            if (KeyboardState.IsKeyDown(Keys.LeftControl)) _cameraPos -= _worldUp * speed;
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            if (CursorState != CursorState.Grabbed) return;

            var d = e.Delta; // OpenTK vector2

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

            // --- Phong shader uniforms ---
            _shader.Use();
            _shader.SetMatrix4(ShaderNames.Phong.View, view);
            _shader.SetMatrix4(ShaderNames.Phong.Projection, _projection);

            // two lights (arrays)
            _shader.SetVector3(ShaderNames.Phong.LightPos0, _lightPos0);
            _shader.SetVector3(ShaderNames.Phong.LightPos1, _lightPos1);

            _shader.SetVector3(ShaderNames.Phong.LightColor0, AppConfig.Light0Color * AppConfig.LightIntensity0);
            _shader.SetVector3(ShaderNames.Phong.LightColor1, AppConfig.Light1Color * AppConfig.LightIntensity1);

            _shader.SetVector3(ShaderNames.Phong.ViewPos, _cameraPos);

            // barycenter for facing/orbit
            Vec3 bary = ComputeBarycenter(_lightPos0, _lightPos1);

            // --- Draw scene objects ---
            for (int i = 0; i < _objects.Count; i++)
            {
                var obj = _objects[i];

                Vec3 toCenter = (bary - obj.BasePosition);
                float yawToCenter = MathF.Atan2(toCenter.X, toCenter.Z);

                var spin =
                    obj.SpinEnabled
                        ? Mat4.CreateFromAxisAngle(obj.SpinAxis.Normalized(), _time * obj.SpinSpeed)
                        : Mat4.Identity;

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

            // --- Lamps (2x) ---
            _lampShader.Use();
            _lampShader.SetMatrix4(ShaderNames.Lamp.View, view);
            _lampShader.SetMatrix4(ShaderNames.Lamp.Projection, _projection);

            DrawLampAt(_lightPos0, AppConfig.Light0Color, AppConfig.LightIntensity0);
            DrawLampAt(_lightPos1, AppConfig.Light1Color, AppConfig.LightIntensity1);

            // --- Glow (2x) ---
            DrawGlow(view, _lightPos0, AppConfig.Light0Color);
            DrawGlow(view, _lightPos1, AppConfig.Light1Color);

            SwapBuffers();
            ShowFPS(args);
        }

        private void DrawLampAt(in Vec3 pos, in Vec3 lightColor, float intensity)
        {
            // Core: opaque
            GL.Disable(EnableCap.Blend);
            GL.DepthMask(true);

            Vec3 coreColor = Mul(lightColor, AppConfig.LampCoreColor) * intensity;

            {
                var core = Mat4.CreateTranslation(pos) * Mat4.CreateScale(_lampCoreRadius);
                _lampShader.SetMatrix4(ShaderNames.Lamp.Model, core);
                _lampShader.SetVector3(ShaderNames.Lamp.Color, coreColor);
                _lampShader.SetFloat(ShaderNames.Lamp.Alpha, AppConfig.LampAlpha);
                _lampMesh.Draw();
            }

            // Halos: additive shells
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            GL.DepthMask(false);

            for (int i = 0; i < AppConfig.LampHaloScales.Length; i++)
            {
                float scale = AppConfig.LampHaloScales[i];
                Vec3 baseHalo = AppConfig.LampHaloColors[i];
                float alpha = AppConfig.LampHaloAlphas[i];

                Vec3 haloColor = Mul(baseHalo, lightColor) * intensity;

                var m = Mat4.CreateTranslation(pos) * Mat4.CreateScale(_lampHaloRadius * scale);
                _lampShader.SetMatrix4(ShaderNames.Lamp.Model, m);
                _lampShader.SetVector3(ShaderNames.Lamp.Color, haloColor);
                _lampShader.SetFloat(ShaderNames.Lamp.Alpha, alpha);
                _lampMesh.Draw();
            }

            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);
        }

        private void ShowFPS(FrameEventArgs args)
        {
            _fpsTime += args.Time;
            _fpsFrames++;

            if (_fpsTime >= 0.5)
            {
                _fpsValue = _fpsFrames / _fpsTime;
                Title = $"{AppConfig.Title} | FPS: {_fpsValue:0.0}";
                _fpsTime = 0.0;
                _fpsFrames = 0;
            }
        }

        private void UpdateLights()
        {
            // phi=0 => cos=+1, sin=0
            // Мы хотим при t=0:
            //   light0 в a1 => x = -RxA => angle = PI
            //   light1 в b2 => x = +RxB => angle = 0
            // => light0: phi + PI, light1: phi
            float phi = _time * AppConfig.LightsAngularSpeed + AppConfig.LightsStartPhase;

            float a0 = phi + MathF.PI; // for a1 at start
            float a1 = phi;            // for b2 at start

            _lightPos0 = AppConfig.EllipseACenter + new Vec3(
                MathF.Cos(a0) * AppConfig.EllipseARadiusX,
                0f,
                MathF.Sin(a0) * AppConfig.EllipseARadiusZ
            );

            _lightPos1 = AppConfig.EllipseBCenter + new Vec3(
                MathF.Cos(a1) * AppConfig.EllipseBRadiusX,
                0f,
                MathF.Sin(a1) * AppConfig.EllipseBRadiusZ
            );
        }

        private void UpdateObjectsOrbit()
        {
            Vec3 bary = ComputeBarycenter(_lightPos0, _lightPos1);

            float radius = AppConfig.ObjectsCircleRadius;
            float w = AppConfig.ObjectsOrbitSpeed;

            // равномерное распределение фаз по индексам
            int n = _objects.Count;
            for (int i = 0; i < n; i++)
            {
                float basePhase = TwoPi * (i / (float)n);
                float angle = basePhase + _time * w;

                _objects[i].BasePosition = bary + new Vec3(
                    MathF.Cos(angle) * radius,
                    0f,
                    MathF.Sin(angle) * radius
                );
            }
        }

        private static Vec3 ComputeBarycenter(in Vec3 p0, in Vec3 p1)
        {
            float m0 = AppConfig.LightMass0;
            float m1 = AppConfig.LightMass1;
            float denom = m0 + m1;

            if (denom < 1e-6f) return (p0 + p1) * 0.5f;

            return (p0 * m0 + p1 * m1) / denom;
        }

        private static Vec3 Mul(in Vec3 a, in Vec3 b)
            => new Vec3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        private void UpdateCameraVectors()
        {
            float yawRad = DegreesToRadians(_yaw);
            float pitchRad = DegreesToRadians(_pitch);

            Vec4 forward0 = new Vec4(0f, 0f, -1f, 0f);

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

        // ----- Glow (per-light) -----

        private void DrawGlow(in Mat4 view, in Vec3 lightPos, in Vec3 lightColor)
        {
            Vec4 clip = _projection * (view * new Vec4(lightPos, 1.0f));
            if (clip.W <= 1e-4f) return;

            Vec3 ndc = clip.Xyz / clip.W;

            float cull = AppConfig.GlowNdcCull;
            if (ndc.X < -cull || ndc.X > cull || ndc.Y < -cull || ndc.Y > cull) return;

            float px = (ndc.X * 0.5f + 0.5f) * Size.X;
            float py = (ndc.Y * 0.5f + 0.5f) * Size.Y;

            float lightDepth01 = ndc.Z * 0.5f + 0.5f;

            float vis = ComputeGlowVisibilityBlock(px, py, lightDepth01);
            if (vis <= AppConfig.GlowVisibleThreshold) return;

            Vec4 lightView = view * new Vec4(lightPos, 1.0f);
            float viewZ = MathF.Max(AppConfig.GlowMinViewZ, -lightView.Z);

            float radiusPx = _glowRadiusPx / (AppConfig.GlowRadiusA + AppConfig.GlowRadiusB * viewZ);
            radiusPx = Clamp(radiusPx, AppConfig.GlowMinRadiusPx, AppConfig.GlowMaxRadiusPx);

            float intensity = AppConfig.GlowBaseIntensity / (AppConfig.GlowIntensityC + AppConfig.GlowIntensityD * viewZ);
            intensity = Clamp(intensity, AppConfig.GlowMinIntensity, AppConfig.GlowMaxIntensity) * vis;

            Vec3 glowColor = Mul(AppConfig.GlowColor, lightColor);

            RenderGlowOverlay(px, py, radiusPx, intensity, glowColor);
        }

        private void RenderGlowOverlay(float px, float py, float radiusPx, float intensity, in Vec3 color)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);

            _glowShader.Use();
            _glowShader.SetVector2(ShaderNames.Glow.LightScreenPx, new Vec2(px, py));
            _glowShader.SetFloat(ShaderNames.Glow.RadiusPx, radiusPx);
            _glowShader.SetVector3(ShaderNames.Glow.Color, color);
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

            int ix = (int)Clamp(px, 0, Size.X - 1);
            int iy = (int)Clamp(py, 0, Size.Y - 1);

            int x0 = (int)Clamp(ix - o, 0, Size.X - 1);
            int y0 = (int)Clamp(iy - o, 0, Size.Y - 1);
            int x1 = (int)Clamp(ix + o, 0, Size.X - 1);
            int y1 = (int)Clamp(iy + o, 0, Size.Y - 1);

            int w = x1 - x0 + 1;
            int h = y1 - y0 + 1;

            GL.ReadPixels(x0, y0, w, h, PixelFormat.DepthComponent, PixelType.Float, _depthBlock);

            float Sample(int sx, int sy)
            {
                sx = (int)Clamp(sx, x0, x1);
                sy = (int)Clamp(sy, y0, y1);

                int lx = sx - x0;
                int ly = sy - y0;
                return _depthBlock[ly * w + lx];
            }

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

            return vis * 0.2f;
        }
    }
}