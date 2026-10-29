# Mortal Kombat-Inspired 2D Game

## Brief description
A two-player fighting game recreated in pure **C# / WinForms** with custom animation frame loading and keyboard controls, built without a game engine.

## Project purpose
Demonstrate multimedia programming fundamentals (sprite animation, input handling, sound cues, round flow) in a classic 2D fighter layout.

## Project requirements
- Windows 10/11
- .NET 8 SDK
- Visual Studio 2022 or the `dotnet` CLI
- A local `Assets/` folder with character sprites and sounds (not shipped here — see note below)

### Assets note
Runtime media lives under `Assets/` (characters, UI, audio). Those files are **not included** in this GitHub repo because they are large (~81MB) and contain third-party copyrighted material. Keep your Assets locally (from `bin/Debug/net8.0-windows/Assets`) and place them in the project `Assets/` folder before running.

## How to navigate
- `Form1.cs` — main game loop, rendering, combat logic, controls
- `Program.cs` — app entry point
- `Subzero.sln` / `Subzero.csproj` — solution/project
- `Game controls.txt` — player 1 / player 2 key bindings
- `Assets/` — expected media layout (see `Assets/README.md`)

## How to run
1. Copy your `Assets` folder into the project root (same level as `Form1.cs`).
2. Restore and run:

```bash
dotnet restore
dotnet run --project Subzero.csproj
```

Or open `Subzero.sln` in Visual Studio and press F5.

### Controls
See `Game controls.txt` (Scorpion = WASD + specials, Sub-Zero = arrows + specials).