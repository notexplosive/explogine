using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public class HitTestStack
{
    private readonly RectangleF? _maskRectangle;
    private readonly List<HitTestStack> _subLayers = new();
    private readonly List<IHitTestZone> _zones = new();

    public HitTestStack(Matrix worldMatrix, RectangleF? maskRectangle = null)
    {
        WorldMatrix = worldMatrix;
        _maskRectangle = maskRectangle;
    }

    public Matrix WorldMatrix { get; }
    public event Action? BeforeLayerResolved;

    public bool IsWithinMaskRectangle(Vector2 position, Matrix parentMatrix)
    {
        return !_maskRectangle.HasValue || _maskRectangle.Value.Contains(Vector2.Transform(position, parentMatrix));
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

    /// <summary>
    /// Gets all PassThrough Zones at the position, as well as the top non-PassThrough Zone at the position if it exists
    /// </summary>
    /// <param name="position">The position, before being transformed by the world matrix</param>
    /// <returns></returns>
    internal List<IHitTestZone> GetZonesAt(Vector2 position)
    {
        _zones.Sort((x, y) => x.Depth - y.Depth);

        var result = new List<IHitTestZone>();
        
        // todo: one day layers and zones will be sorted in the same list so we won't do zones then layers, it'll just be one for loop where we do them all
        
        foreach (var zone in _zones)
        {
            if (zone.Contains(position, WorldMatrix))
            {
                result.Add(zone);

                if (!zone.PassThrough)
                {
                    return result;
                }
            }
        }

        foreach (var layer in _subLayers)
        {
            if (layer.IsWithinMaskRectangle(position, WorldMatrix))
            {
                var zones = layer.GetZonesAt(position);
                result.AddRange(zones);
                foreach (var zone in zones)
                {
                    if (!zone.PassThrough)
                    {
                        return result;
                    }
                }
            }
        }

        return result;
    }

    public void AddZone(RectangleF rect, Depth depth, HoverState hoverState, bool passThrough = false)
    {
        AddZone(rect, depth, hoverState.Unset, hoverState.Set, passThrough);
    }

    public void AddZone(RectangleF rect, Depth depth, Action callback, bool passThrough = false)
    {
        AddZone(rect, depth, null, callback, passThrough);
    }

    public void AddZone(RectangleF rect, Depth depth, Action? beforeResolve, Action callback, bool passThrough = false)
    {
        _zones.Add(new HitTestZone(rect, depth, beforeResolve, callback, passThrough));
    }

    public void AddInfiniteZone(Depth depth, HoverState hoverState, bool passThrough = false)
    {
        _zones.Add(new InfiniteHitTestZone(depth, hoverState.Unset, hoverState.Set, passThrough));
    }
    
    public void AddInfiniteZone(Depth depth, Action callback, bool passThrough = false)
    {
        _zones.Add(new InfiniteHitTestZone(depth, null, callback, passThrough));
    }

    public HitTestStack AddLayer(Matrix layerMatrix, RectangleF? layerRectangle = null)
    {
        var hitTestStack = new HitTestStack(WorldMatrix * layerMatrix, layerRectangle);
        _subLayers.Add(hitTestStack);
        return hitTestStack;
    }
}