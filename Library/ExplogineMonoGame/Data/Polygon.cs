using System;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public struct Polygon
{
    private float _angle;

    public Polygon(Vector2 centerLocation, Vector2[] relativePoints)
    {
        _angle = 0;
        RelativePoints = relativePoints;
        CenterLocation = centerLocation;
        VertexCount = relativePoints.Length;
        OriginalRelativePoints = new Vector2[relativePoints.Length];

        for (var i = 0; i < relativePoints.Length; i++)
        {
            OriginalRelativePoints[i] = relativePoints[i];
        }
    }

    public Vector2[] OriginalRelativePoints { get; }

    public float Angle
    {
        get => _angle;
        set
        {
            for (var i = 0; i < RelativePoints.Length; i++)
            {
                RelativePoints[i] = OriginalRelativePoints[i].Rotated(value, Vector2.Zero);
            }

            _angle = value;
        }
    }

    public Vector2[] RelativePoints { get; }
    public Vector2 CenterLocation { get; }
    public int VertexCount { get; }
    public Vector2 this[Index i] => CenterLocation + RelativePoints[i];

    /// <summary>
    /// Rotate the Polygon clockwise around the center
    /// </summary>
    /// <param name="radians"></param>
    public void Rotate(float radians)
    {
        Angle += radians;
    }
    
}
