using System.Numerics;

namespace StarshipForge.Api.Plugins;

/// <summary>
/// Plugin providing geometric calculations for starship design.
/// </summary>
public class GeometryPlugin
{
    /// <summary>
    /// Calculates the volume of a cylinder, useful for propellant tanks.
    /// </summary>
    public double CalculateCylinderVolume(double radius, double height)
    {
        if (radius <= 0 || height <= 0)
            throw new ArgumentException("Radius and height must be positive.");
        return Math.PI * radius * radius * height;
    }

    /// <summary>
    /// Calculates the surface area of a cylinder.
    /// </summary>
    public double CalculateCylinderSurfaceArea(double radius, double height)
    {
        if (radius <= 0 || height <= 0)
            throw new ArgumentException("Radius and height must be positive.");
        return 2 * Math.PI * radius * (radius + height);
    }

    /// <summary>
    /// Creates a 3D vector from coordinates.
    /// </summary>
    public Vector3 CreateVector3(double x, double y, double z)
    {
        return new Vector3((float)x, (float)y, (float)z);
    }

    /// <summary>
    /// Adds two 3D vectors.
    /// </summary>
    public Vector3 AddVectors(Vector3 a, Vector3 b)
    {
        return a + b;
    }

    /// <summary>
    /// Calculates the distance between two points in 3D space.
    /// </summary>
    public double CalculateDistance(Vector3 point1, Vector3 point2)
    {
        return Vector3.Distance(point1, point2);
    }
}