using System;
using ExplogineCore.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

internal interface IHitTestZone
{
    Depth Depth { get; }
    Action? BeforeResolve { get; }
    Action Callback { get; }
    bool PassThrough { get; }
    bool Contains(Vector2 position, Matrix worldMatrix);
}

internal readonly record struct HitTestZone(RectangleF Rectangle, Depth Depth, Action? BeforeResolve, Action Callback,
    bool PassThrough) : IHitTestZone
{
    public bool Contains(Vector2 position, Matrix worldMatrix)
    {
        return Rectangle.Contains(Vector2.Transform(position, worldMatrix));
    }
}

internal readonly record struct InfiniteHitTestZone(Depth Depth, Action? BeforeResolve, Action Callback,
    bool PassThrough) : IHitTestZone
{
    public bool Contains(Vector2 position, Matrix worldMatrix)
    {
        return true;
    }
}
