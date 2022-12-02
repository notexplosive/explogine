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
    public Vector2 this[Index i] => RelativePoints[i] + CenterLocation;
}
