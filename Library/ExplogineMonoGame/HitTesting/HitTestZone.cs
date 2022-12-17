using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.HitTesting;

internal readonly record struct HitTestZone(RectangleF Rectangle, Depth Depth, Action? Callback)
{
    public bool Contains(Vector2 position)
    {
        return Rectangle.Contains(position);
    }
}
