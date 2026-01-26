# PrototypeGraphicsCore

A 3D learning project in **C# + OpenTK (OpenGL 3.3)**: primitives (**cube / sphere / pyramid / torus**) around a light source, **Phong** lighting, a visible “lamp” and **glow**, and camera controls like in FPS. 
The scene’s math is custom (**MyMath**), with self-written mathematics for 3D graphics.

<p align="center">
  <img src="docs/gif.gif" alt="PrototypeGraphicsCore demo" />
</p>

<p align="center">
  <img alt="OpenGL" src="https://img.shields.io/badge/OpenGL-3.3-blue">
  <img alt="GLSL" src="https://img.shields.io/badge/GLSL-330-blueviolet">
  <img alt="C#" src="https://img.shields.io/badge/C%23-.NET-informational">
</p>

---

## Features

- OpenGL 3.3 + GLSL 330 
- Primitives: cube, sphere, pyramid, torus 
- Objects are arranged **in a circle** around the light source 
- **Phong** (ambient + diffuse + specular) 
- Visible light source (sphere-lamp) + **halo (glow)** 
- FPS-like camera: WASD + mouse + zoom with the wheel 
- Shaders are placed in files (`data/shaders/*`) 
- Config parameters in `Config/AppConfig` 
- Uniform names constants (to avoid writing `"uView"` by hand)

---

## Controls

| Action | Input |
|---|---|
| Move | **W A S D** |
| Look around | **Mouse** |
| Zoom (FOV) | **Mouse Wheel** |
| Velocity | **Shift** |
| Exit | **Esc** |

---

## Requirements

- GPU/driver with support for **OpenGL 3.3**
- **.NET SDK** (see `TargetFramework` in `.csproj`)
- Windows / Linux / macOS

---

## Build and run

### Visual Studio
1. New Solution
2. Select the "PrototypeGraphicsCore" project
3. Run

### CLI
```bash
dotnet restore
dotnet run --project PrototypeGraphicsCore/PrototypeGraphicsCore.csproj
