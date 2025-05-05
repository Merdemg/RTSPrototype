This project is a prototype RTS / tower-defense style game built in Unity 6, focused on expandable architecture, AI movement strategies, and efficient spatial queries via a custom grid system.

Features & Architecture Highlights

Custom Generic Grid System
  - Generic `Grid<TCell>` with `Add`, `Move`, `Delete`, and fast `GetCellCoords`
  - Units register/unregister automatically
  - Used to drastically reduce per-frame distance checks

Modular AI Movement Strategies
  - Implements `IMovementStrategy` for plug-and-play AI behavior
  - Current strategies:
	- Closest Enemy
	- Threat Scoring (ETA and point value)
	- Rearguard (defend flag radius)
	- Vanguard (hunt clusters of fastest enemies)
	- Player Controlled

ScriptableObject-Driven Config
  - All unit stats and game settings are externalized via SOs
  - Includes AI type, spawn config, enemy multipliers, and defender count

Grid-Aware Targeting Logic
  - AI scans grid cells outward from relevant positions (not entire grid)
  - Closest/threat assessment done per cell, avoids O(N²) searches

Efficient Target Caching
  - (Most) Strategies only look for a new target if the current one is lost or dead
  - Avoids redundant evaluation every frame

Clean System Separation
  - `GameManager` handles game flow
  - `UnitFactory` spawns and initializes units
  - `UnitRegistry` tracks active units by faction
  - `HUDController`, `EndGameCanvasController`, and menu logic are modular

Functional UI Layer
  - Main menu with difficulty & AI type selectors
  - Pause and restart support
  - Live score and enemy kill count tracking
  - Debug panel with per-unit targeting reasons

Performance Notes

- **Grid-based scanning** ensures per-frame cost is closer to `O(U)` than `O(U²)`
- **`GridManager.Move()`** is called each frame a unit moves, but internally it only updates the grid if the unit’s cell has actually changed
- **All object destruction is done through central registries** for clean resets

---

If I Had More Time

- Add object pooling for enemies/defenders to avoid GC hits
- Layer in animation events or blend trees for idle/attack cycles
- Expand player control (e.g. click-to-move defenders, assign priorities)
- Add pathfinding or cell weight systems for maze-style mechanics
- Bake in runtime difficulty scaling and wave progression

---

Summary

This project demonstrates:
- Performance-conscious AI design
- Expandable and clean Unity architecture
- Strong usage of Unity 6 systems (Input, SOs, Canvas, events)
- Real-world decision making for gameplay systems



