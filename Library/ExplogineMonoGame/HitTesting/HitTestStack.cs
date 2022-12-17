using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.HitTesting;

public class HitTestStack
{
    private readonly Matrix _worldMatrix;
    private readonly List<HitTestStack> _subLayers = new();
    private readonly List<HitTestZone> _zones = new();
    public event Action? BeforeResolved;

    public HitTestStack(Matrix worldMatrix)
    {
        _worldMatrix = worldMatrix;
    }

    internal void OnBeforeResolve()
    {
        BeforeResolved?.Invoke();

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
            if (zone.Contains(position, _worldMatrix))
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

    public void Clear()
    {
        _zones.Clear();
    }

    public void AddZone(RectangleF rect, Depth depth, Action? callback = null)
    {
        _zones.Add(new HitTestZone(rect, depth, callback));
    }

    public HitTestStack AddLayer(Matrix layerMatrix)
    {
        var hitTestStack = new HitTestStack(_worldMatrix * layerMatrix);
        _subLayers.Add(hitTestStack);
        return hitTestStack;
    }
}
