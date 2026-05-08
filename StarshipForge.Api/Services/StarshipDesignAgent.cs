using Microsoft.AspNetCore.SignalR;
using StarshipForge.Api.Hubs;
using StarshipForge.Api.Plugins;
using System.Numerics;

namespace StarshipForge.Api.Services;

/// <summary>
/// AI agent service for starship design using local mock implementation.
/// Generates deterministic designs without external API dependencies.
/// </summary>
public class StarshipDesignAgent
{
    private readonly HardwareCatalog _hardwareCatalog;
    private readonly IHubContext<DesignHub> _hubContext;
    private readonly GeometryPlugin _geometry;

    public StarshipDesignAgent(HardwareCatalog hardwareCatalog, IHubContext<DesignHub> hubContext)
    {
        _hardwareCatalog = hardwareCatalog;
        _hubContext = hubContext;
        _geometry = new GeometryPlugin();
    }

    /// <summary>
    /// Processes a design request and streams updates via SignalR.
    /// Uses local mock implementation to generate starship designs.
    /// </summary>
    public async Task ProcessDesignRequest(string request)
    {
        try
        {
            var design = GenerateMockDesign(request);
            
            // Stream design update via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveStateUpdate", new
            {
                type = "design_update",
                design = design,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveStateUpdate", new
            {
                type = "error",
                message = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Generates a mock starship design with realistic specifications.
    /// </summary>
    private object GenerateMockDesign(string request)
    {
        var boosterSpec = _hardwareCatalog.GetSpec("SuperHeavyBooster") ?? throw new InvalidOperationException("Booster spec not found");
        var starshipSpec = _hardwareCatalog.GetSpec("StarshipTank") ?? throw new InvalidOperationException("Starship spec not found");
        var raptorSpec = _hardwareCatalog.GetSpec("RaptorEngine") ?? throw new InvalidOperationException("Engine spec not found");

        // Calculate design based on request keywords
        var engineCount = request.Contains("heavy", StringComparison.OrdinalIgnoreCase) ? 33 : 6;
        var tankSections = request.Contains("cargo", StringComparison.OrdinalIgnoreCase) ? 3 : 2;
        
        // Geometry calculations
        var boosterVolume = _geometry.CalculateCylinderVolume(4.5, 70); // 9m diameter, 70m tall
        var starshipVolume = _geometry.CalculateCylinderVolume(4.5, 50); // 9m diameter, 50m tall
        var totalEngineThrust = raptorSpec.Thrust * engineCount;
        var totalMass = boosterSpec.Mass + (starshipSpec.Mass * tankSections) + (raptorSpec.Mass * engineCount);
        
        return new
        {
            id = Guid.NewGuid(),
            name = "Starship Design v1",
            description = "AI-generated starship design optimized for your specifications",
            components = new
            {
                booster = new
                {
                    name = boosterSpec.Name,
                    mass = boosterSpec.Mass,
                    fuel_capacity = boosterSpec.FuelCapacity,
                    dimensions = new { width = 9.0, height = 70.0, length = 9.0 }
                },
                starship = new
                {
                    name = starshipSpec.Name,
                    sections = tankSections,
                    mass_per_section = starshipSpec.Mass,
                    fuel_capacity = starshipSpec.FuelCapacity,
                    dimensions = new { width = 9.0, height = 50.0, length = 9.0 }
                },
                engines = new
                {
                    type = raptorSpec.Name,
                    count = engineCount,
                    mass_per_engine = raptorSpec.Mass,
                    thrust_per_engine = raptorSpec.Thrust,
                    total_thrust = totalEngineThrust
                }
            },
            performance = new
            {
                total_mass_kg = Math.Round(totalMass, 2),
                booster_volume_m3 = Math.Round(boosterVolume, 2),
                starship_volume_m3 = Math.Round(starshipVolume, 2),
                thrust_to_weight = Math.Round(totalEngineThrust / (totalMass * 9.81), 4),
                estimated_payload_capacity_kg = 100000
            },
            validation = new
            {
                mass_valid = totalMass > 0,
                thrust_valid = totalEngineThrust > 0,
                structure_valid = boosterVolume > 0 && starshipVolume > 0,
                feasible = true
            },
            timestamp = DateTime.UtcNow
        };
    }
}