using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Layout;

public readonly record struct ArrangementSettings(Orientation Orientation = Orientation.Horizontal,
    int PaddingBetweenElements = 0, Vector2 Margin = default, Alignment Alignment = default)
{
    public Axis Axis
    {
        get
        {
            return Orientation switch
            {
                Orientation.Horizontal => Axis.X,
                Orientation.Vertical => Axis.Y,
                _ => throw new Exception("Invalid orientation")
            };
        }
    }
}
