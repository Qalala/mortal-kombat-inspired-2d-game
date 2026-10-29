# Mortal Kombat-Inspired 2D Game

A two-player fighting game recreated in pure **C# / Windows Forms**, with custom sprite-frame animation, special moves, round flow, and keyboard controls — built **without a game engine**.

The stack is:
- **Language**: C#
- **UI / runtime**: WinForms on .NET 8 (`net8.0-windows`)
- **Assets**: local sprite sheets / frame folders + WAV/MP3 cues

This project demonstrates multimedia programming fundamentals: frame loading, input state, collision/hit logic, sound triggers, and arena UI composition.

## Tech stack
- C#
- Windows Forms (.NET 8)
- Custom sprite animation (no game engine)

## Architecture (high level)

```
Subzero.sln / Subzero.csproj
├─ Program.cs                 [WinForms entry]
├─ Form1.cs                   [game loop, rendering, combat, input]
├─ Form1.Designer.cs / .resx  [form designer artifacts]
├─ Game controls.txt          [P1 / P2 keybindings]
└─ Assets/                    [expected media layout]
   ├─ Characters/
   │  ├─ Scorpion/Sprites + Sounds
   │  └─ Subzero/Sprites + Sounds
   └─ Images/                 [arena / UI / announcer assets]
```

### Runtime behavior (conceptual)
- Character animations are loaded as ordered `.png` frame lists from Assets folders.
- Keyboard state drives movement, stance, jumps, ducks, sprints, and specials.
- Special examples:
  - Scorpion spear / rope pull
  - Sub-Zero freeze and slide
- Round announcer / win / flawless cues are triggered from sound assets when present.
- Escape exits the game window.

### Assets note (important)
The full sprite/audio pack is **not shipped in this GitHub repo** (~80MB+ and third-party copyrighted media).

Locally, assets typically live in the build output:

```text
bin/Debug/net8.0-windows/Assets/
```

Copy that `Assets` folder into the project root (same level as `Form1.cs`) before running.  
`Assets/README.md` documents the expected layout. The `.csproj` is set up to copy `Assets/**` to the output directory when present.

---

## Running the project

### Prerequisites
- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 **or** `dotnet` CLI
- Local `Assets/` folder (see above)

### Install / restore

```bash
dotnet restore
```

### Run

```bash
dotnet run --project Subzero.csproj
```

Or open `Subzero.sln` in Visual Studio and press **F5**.

If Assets are missing, the app may crash or fail while loading images/sounds — that is expected until Assets are placed correctly.

---

## Controls

### Scorpion (Player 1)
| Key | Action |
| --- | --- |
| A / D | Move left / right |
| Space | Jump |
| S | Duck / stand |
| Shift | Toggle sprint |
| G | Throw rope (spear) |
| P | Flip stance |
| E / Q | Flip directions |
| Z / X | Test damage hooks |
| Esc | Exit |

### Sub-Zero (Player 2)
| Key | Action |
| --- | --- |
| ← / → | Move |
| ↑ | Jump |
| ↓ | Duck / stand |
| M | Toggle sprint |
| L | Flip stance |
| F | Freeze special |
| N | Slide attack |
| I / O | Flip directions |
| Esc | Exit |

Full list: `Game controls.txt`.

---

## How to navigate the repo
- Start in `Form1.cs` — almost all gameplay logic lives here
- Use `Game controls.txt` while testing both players
- Keep `Assets/` beside the project for a runnable build
- Ignore `bin/` / `obj/` / `.vs/` (gitignored)

---

## Demo walkthrough
1. Copy your local Assets into `./Assets`.
2. `dotnet run --project Subzero.csproj`.
3. Player 1 moves with WASD; Player 2 with arrows.
4. Trigger Scorpion spear (`G`) and Sub-Zero freeze (`F`) / slide (`N`).
5. Play a round and verify announcer / win sounds if those assets are present.

### Notes / limitations
- Windows-only WinForms desktop app
- Asset media intentionally excluded from GitHub
- This is an academic multimedia recreation, not an official Mortal Kombat product

---

## Project purpose
Demonstrate a from-scratch 2D fighter in C# WinForms: animation frames, two-player controls, special moves, and multimedia asset integration without relying on Unity/Godot/Unreal.
