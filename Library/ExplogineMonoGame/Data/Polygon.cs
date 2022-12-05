using System;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public struct Polygon
{
    public Polygon(Vector2 centerLocation, Vector2[] relativePoints)
    {
        RelativePoints = relativePoints;
        CenterLocation = centerLocation;
        VertexCount = relativePoints.Length;
    }

    public Vector2[] RelativePoints { get; }
    public Vector2 CenterLocation { get; }
    public int VertexCount { get; }
    public Vector2 this[Index i] => CenterLocation + RelativePoints[i];

    /// <summary>
    /// Rotate the Polygon clockwise around its own center
    /// </summary>
    /// <param name="radians"></param>
    /// <returns></returns>
    public Polygon Rotated(float radians)
    {
        var newPoints = new Vector2[RelativePoints.Length];
        for (var i = 0; i < RelativePoints.Length; i++)
        {
            newPoints[i] = RelativePoints[i].Rotated(radians, Vector2.Zero);
        }

        return new Polygon(CenterLocation, newPoints);
    }
}
