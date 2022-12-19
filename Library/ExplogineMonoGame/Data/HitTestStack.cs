using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public class HitTestStack
{
    public Matrix WorldMatrix { get; }
    private readonly List<HitTestStack> _subLayers = new();
    private readonly List<HitTestZone> _zones = new();
    public event Action? BeforeLayerResolved;

    public HitTestStack(Matrix worldMatrix)
    {
        WorldMatrix = worldMatrix;
    }

    internal void OnBeforeResolve()
    {
        BeforeLayerResolved?.Invoke();

        foreach (var zone in _zones)
        {
            zone.BeforeResolve?.Invoke();
        }
        
        foreach (var layer in _subLayers)
        {
            layer.OnBeforeResolve();
        }
    }

    internal HitTestZone? GetTopZoneAt(Vector2 position)
    {
        _zones.Sort((x, y) => x.Depth - y.Depth);

        foreach (var zone in _zones)
        {
            if (zone.Contains(position, WorldMatrix))
            {
                return zone;
            }
        }

        foreach (var layer in _subLayers)
        {
            var zone = layer.GetTopZoneAt(position);
            if (zone.HasValue)
            {
                return zone;
            }
        }

        return null;
    }

    public void AddZone(RectangleF rect, Depth depth, Action callback)
    {
        _zones.Add(new HitTestZone(rect, depth, null, callback));
    }
    
    public void AddZone(RectangleF rect, Depth depth, Action? beforeResolve, Action callback)
    {
        _zones.Add(new HitTestZone(rect, depth, beforeResolve, callback));
    }

    public HitTestStack AddLayer(Matrix layerMatrix)
    {
        var hitTestStack = new HitTestStack(WorldMatrix * layerMatrix);
        _subLayers.Add(hitTestStack);
        return hitTestStack;
    }
}
