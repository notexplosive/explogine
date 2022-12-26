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
    string Name { get; }
    bool Contains(Vector2 position, Matrix worldMatrix);
}

internal readonly record struct HitTestZone(RectangleF Rectangle, Depth Depth, Action? BeforeResolve, Action Callback,
    bool PassThrough) : IHitTestZone
{
    public bool Contains(Vector2 position, Matrix worldMatrix)
    {
        return Rectangle.Contains(Vector2.Transform(position, worldMatrix));
    }

    public string Name => $"Zone {Rectangle}";
}

internal class NestedHitTestZone : IHitTestZone
{
    public HitTestStack HitTestStack { get; }
    private readonly IHitTestZone _childZone;
    
    public Action? BeforeResolve => _childZone.BeforeResolve;
    public Action Callback => _childZone.Callback;
    public bool PassThrough => _childZone.PassThrough;
    public bool Contains(Vector2 position, Matrix worldMatrix)
    {
        return _childZone.Contains(position, worldMatrix);
    }

    public Depth Depth => _childZone.Depth;

    public NestedHitTestZone(HitTestStack hitTestStack, IHitTestZone zone)
    {
        _childZone = zone;
        HitTestStack = hitTestStack;
    }

    public string Name => $"NestedZone using {_childZone.Name}";
}

internal readonly record struct InfiniteHitTestZone(Depth Depth, Action? BeforeResolve, Action Callback,
    bool PassThrough) : IHitTestZone
{
    public bool Contains(Vector2 position, Matrix worldMatrix)
    {
        return true;
    }
    
    public string Name => "InfiniteZone";
}
