# PrototypeGraphicsCore

Учебный 3D-проект на **C# + OpenTK (OpenGL 3.3)**: примитивы (**cube / sphere / pyramid / torus**) вокруг источника света, **Phong**-освещение, видимая “лампа” и **glow**, управление камерой как в FPS.  
Математика сцены — своя (**MyMath**), без `OpenTK.Mathematics`.

<p align="center">
  <img src="docs/screenshot.png" alt="Screenshot" width="800" />
</p>

<!-- Бейджи (по желанию) -->
<p align="center">
  <img alt="OpenGL" src="https://img.shields.io/badge/OpenGL-3.3-blue">
  <img alt="GLSL" src="https://img.shields.io/badge/GLSL-330-blueviolet">
  <img alt="C#" src="https://img.shields.io/badge/C%23-.NET-informational">
</p>

---

## Features

- ✅ OpenGL 3.3 + GLSL 330  
- ✅ Примитивы: cube, sphere, pyramid, torus  
- ✅ Объекты расположены **по окружности** вокруг источника света  
- ✅ **Phong** (ambient + diffuse + specular)  
- ✅ Видимый источник света (сфера-лампа) + **ореол (glow)**  
- ✅ FPS-подобная камера: WASD + мышь + зум колесом  
- ✅ Шейдеры вынесены в файлы (`data/shaders/*`)  
- ✅ Конфиг параметров в `Config/AppConfig`  
- ✅ Константы имён uniform’ов (чтобы не писать `"uView"` руками)

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

- GPU/драйвер с поддержкой **OpenGL 3.3**
- **.NET SDK** (см. `TargetFramework` в `.csproj`)
- Windows / Linux / macOS

---

## Build & Run

### Visual Studio
1. Открой solution
2. Выбери проект `PrototypeGraphicsCore`
3. Run

### CLI
```bash
dotnet restore
dotnet run --project PrototypeGraphicsCore/PrototypeGraphicsCore.csproj
