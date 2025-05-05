This project is a prototype tower-defense style game built in Unity 6, focused on expandable architecture, AI movement strategies, and efficient spatial queries via a custom grid system. It was designed to demonstrate scalable, maintainable systems while staying within a ~5-hour scope.
Features & Architecture Highlights

    Custom Generic Grid System
    Efficient spatial indexing using a Grid<TCell> structure with fast unit registration and lookups. Used to reduce unnecessary distance checks during AI decisions.

    Modular AI Strategies (IMovementStrategy)
    Easily expandable: defenders use pluggable behaviors such as:

        Closest Enemy

        Threat Scoring (ETA + point value)

        Rearguard (flag proximity defense)

        Vanguard (fastest-enemy cluster response)

        Player Controlled

    ScriptableObject-Driven Config
    Game settings (AI type, spawn data, difficulty multipliers) and unit stats are fully data-driven for designer flexibility.

    Clean Separation of Concerns
    GameManager orchestrates setup; UnitFactory handles instantiation; each Unit is autonomous and stateless outside its setup.

    Grid-Aware Movement & Targeting
    All enemy scanning operations are optimized by expanding outward from key cells (unit’s cell or flag’s cell), avoiding global iteration.

    Target Caching & Cleanup
    AI strategies cache their current target and only search again when the target is lost or dead — this greatly reduces per-frame overhead.

    Debug & UI Layer
    The game includes:

        Start menu with AI & difficulty selection

        Pause & restart handling

        Live debug panel showing targeting reasons

        End screen with stats

 Notes on Optimization

    Strategies scale with O(U) in practical scenarios due to grid optimizations, vs. naïve O(U²).

    All unit-grid syncing is done only when units move — no polling per frame.
