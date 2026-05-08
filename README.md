# Starship Forge

"The machine that builds the machine" - An AI-agentic starship design system.

## Overview

Starship Forge is a cutting-edge application that combines AI-driven design with real-time 3D visualization. The system uses Semantic Kernel for intelligent starship design, with geometry calculations exposed as AI-callable functions, and streams design updates to a Mission Control-themed frontend via SignalR.

## Architecture

- **Backend**: C#/.NET 9 with Clean Architecture
  - Semantic Kernel for AI agent orchestration
  - SignalR for real-time 3D state streaming
  - Hardware catalog service for component specifications
  - Geometry plugin with KernelFunctions for calculations

- **Frontend**: TypeScript with Three.js rendering
  - Mission Control dark theme with high-density data display
  - Real-time SignalR connection for design updates
  - Modular Atomic Design components

## Features

- **Local Design Agent**: Mock implementation that generates deterministic starship designs with no external dependencies
- **Real-Time Visualization**: 3D rendering with Three.js, updated via SignalR streams
- **Hardware Validation**: Agent uses local catalog specifications instead of hallucinating
- **Safety First**: Physical constraint validation (mass > 0, dimensions > 0)
- **Performance Optimized**: System.Numerics for vector math, readonly structs
- **Zero Cost**: No API tokens or external services required

## Tech Stack

### Backend
- .NET 9
- ASP.NET Core
- SignalR
- System.Numerics

### Frontend
- TypeScript
- React
- Three.js
- SignalR Client
- Vite

## Getting Started

### Prerequisites
- .NET 9 SDK
- Node.js 18+
- npm

### Setup

1. **Clone and navigate to the project**:
   ```bash
   cd StarshipForge
   ```

2. **Backend Setup**:
   ```bash
   cd StarshipForge.Api
   dotnet restore
   dotnet run
   ```
   The API will be available at `http://localhost:5147`

3. **Frontend Setup**:
   ```bash
   cd StarshipForge.Web
   npm install
   npm run dev
   ```
   The frontend will be available at `http://localhost:5173`

### Usage

1. Open the frontend in your browser
2. Click "Generate Design" to trigger the AI agent
3. Watch the 3D visualization update in real-time
4. Monitor system status in the Mission Control panel

## API Endpoints

- `POST /api/design` - Submit a design request to the AI agent
- `GET /designHub` - SignalR hub for real-time updates

## Development Guidelines

### Backend
- Use Semantic Kernel plugins for AI functionality
- Expose geometry logic as `[KernelFunction]` methods
- Validate physical constraints in all calculations
- Use `System.Numerics` for vector operations
- Prefer `readonly struct` for geometric data

### Frontend
- Use Three.js for 3D rendering
- Maintain Mission Control dark theme
- Implement modular, reusable components
- Use strict TypeScript typing

### AI Agent
- Local mock design generation with no external API dependencies
- Deterministic starship specifications based on request parameters
- Hardware catalog integration for realistic components
- Geometry calculations for structural validation
- Thrust-to-weight ratio analysis
- Future: Can be upgraded to use Ollama for local LLM-based design

## Contributing

Follow the established patterns:
- Backend: Clean Architecture with Semantic Kernel plugins
- Frontend: Atomic Design with Three.js integration
- AI: Tool calling with hardware catalog validation

## License

[Add license information]