using System.Numerics;

namespace StarshipForge.Api.Services;

/// <summary>
/// Service providing hardware specifications for starship components.
/// The AI agent must fetch specs from this catalog instead of hallucinating.
/// </summary>
public class HardwareCatalog
{
    private readonly Dictionary<string, HardwareSpec> _specs = new()
    {
        ["RaptorEngine"] = new HardwareSpec
        {
            Name = "Raptor Engine",
            Mass = 2000, // kg
            Thrust = 2300000, // N (sea level)
            Dimensions = new Vector3(4.0f, 4.0f, 3.0f) // meters
        },
        ["SuperHeavyBooster"] = new HardwareSpec
        {
            Name = "Super Heavy Booster",
            Mass = 200000, // kg (empty)
            FuelCapacity = 1200000, // liters of methane
            Dimensions = new Vector3(9.0f, 9.0f, 70.0f) // meters
        },
        ["StarshipTank"] = new HardwareSpec
        {
            Name = "Starship Tank Section",
            Mass = 120000, // kg (empty)
            FuelCapacity = 1200000, // liters
            Dimensions = new Vector3(9.0f, 9.0f, 50.0f) // meters
        }
    };

    /// <summary>
    /// Retrieves hardware specifications by component name.
    /// </summary>
    public HardwareSpec? GetSpec(string componentName)
    {
        return _specs.TryGetValue(componentName, out var spec) ? spec : null;
    }

    /// <summary>
    /// Lists all available component names.
    /// </summary>
    public IEnumerable<string> GetAvailableComponents()
    {
        return _specs.Keys;
    }
}

/// <summary>
/// Hardware specification data structure.
/// </summary>
public readonly struct HardwareSpec
{
    public required string Name { get; init; }
    public double Mass { get; init; } // kg
    public double Thrust { get; init; } // N
    public double FuelCapacity { get; init; } // liters
    public Vector3 Dimensions { get; init; } // meters (width, height, length)
}