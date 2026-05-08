# Starship Forge: AI Agentic Design Instructions
- Focus: "The machine that builds the machine."
- Backend (C#): Use Semantic Kernel plugins. Any geometry logic must be a [KernelFunction] so the AI agent can call it.
- Frontend (TS): Use Three.js for rendering. The UI must be "Mission Control" themed (Dark, High-Density Data).
- Architecture: API-first. The AI Agent lives on the backend but streams 3D state updates to the frontend via SignalR.

# SpaceX Starship Design Style Guide

## General Principles
- **Safety First:** When generating C# code, always include validation for physical constraints (e.g., mass cannot be negative).
- **Performance:** Use `System.Numerics` for vector math in C#. Prefer `readonly struct` for frequently allocated geometric data.
- **Agentic Logic:** When writing AI agent code, use "Tool Calling" patterns. The agent should never "hallucinate" hardware specs; it must fetch them from the `HardwareCatalog` service.

## Tech Stack Rules
- **Backend:** C#/.NET 9 using Clean Architecture. Use gRPC for high-performance internal communication if applicable.
- **Frontend:** TypeScript with strict typing. Components must be modular and reusable (Atomic Design).
- **AI:** Use Semantic Kernel patterns. Focus on "Planner" and "Plugin" architecture to let the AI interact with the 3D canvas.

## Coding Style
- Follow the "Boring but Reliable" principle. Use standard libraries before pulling in niche dependencies.
- Every function must have a concise summary explaining its engineering purpose.
