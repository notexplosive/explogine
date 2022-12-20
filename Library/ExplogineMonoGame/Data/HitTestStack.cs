using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public class HitTestStack
{
    private readonly RectangleF? _maskRectangle;
    private readonly List<HitTestStack> _subLayers = new();
    private readonly List<HitTestZone> _zones = new();

    public HitTestStack(Matrix worldMatrix, RectangleF? maskRectangle = null)
    {
        WorldMatrix = worldMatrix;
        _maskRectangle = maskRectangle;
    }

    public Matrix WorldMatrix { get; }
    public event Action? BeforeLayerResolved;

    public bool IsWithinMaskRectangle(Vector2 transformedLocation)
    {
        return !_maskRectangle.HasValue || _maskRectangle.Value.Contains(transformedLocation);
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
            if (layer.IsWithinMaskRectangle(position))
            {
                var zone = layer.GetTopZoneAt(position);
                if (zone.HasValue)
                {
                    return zone;
                }
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

    public HitTestStack AddLayer(Matrix layerMatrix, RectangleF? layerRectangle = null)
    {
        var hitTestStack = new HitTestStack(WorldMatrix * layerMatrix, layerRectangle);
        _subLayers.Add(hitTestStack);
        return hitTestStack;
    }
}
