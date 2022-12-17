using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

internal readonly record struct HitTestZone(RectangleF Rectangle, Depth Depth, Action? BeforeResolve, Action Callback)
{
    public bool Contains(Vector2 position, Matrix worldMatrix)
    {
        return Rectangle.Contains(Vector2.Transform(position, worldMatrix));
    }
}
