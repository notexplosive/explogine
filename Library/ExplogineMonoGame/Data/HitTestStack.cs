using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public class HitTestStack
{
    private readonly List<IHitTestZone> _zones = new();

    public HitTestStack(Matrix worldMatrix)
    {
        WorldMatrix = worldMatrix;
    }

    public Matrix WorldMatrix { get; }
    public event Action? BeforeLayerResolved;

    internal void OnBeforeResolve()
    {
        BeforeLayerResolved?.Invoke();

        foreach (var zone in _zones)
        {
            zone.BeforeResolve?.Invoke();

            if (zone is NestedHitTestZone nest)
            {
                nest.HitTestStack.OnBeforeResolve();
            }
        }
    }

    /// <summary>
    ///     Gets all PassThrough Zones at the position, as well as the top non-PassThrough Zone at the position if it exists
    /// </summary>
    /// <param name="position">The position, before being transformed by the world matrix</param>
    /// <returns></returns>
    internal List<IHitTestZone> GetZonesAt(Vector2 position)
    {
        _zones.Sort((x, y) => x.Depth - y.Depth);

        var result = new List<IHitTestZone>();

        foreach (var zone in _zones)
        {
            if (zone.Contains(position, WorldMatrix))
            {
                if (zone is NestedHitTestZone nestedHitTestZone)
                {
                    var lowerZones = nestedHitTestZone.HitTestStack.GetZonesAt(position);
                    result.AddRange(lowerZones);
                    foreach (var lowerZone in lowerZones)
                    {
                        if (!lowerZone.PassThrough)
                        {
                            return result;
                        }
                    }
                }
                else
                {
                    result.Add(zone);
                }

                if (!zone.PassThrough)
                {
                    return result;
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

    public HitTestStack AddLayer(Matrix layerMatrix, Depth depth, RectangleF? rect = null)
    {
        void DoNothing()
        {
            // clumsy, in any other scenario I want HitTestZones to have a callback, so I don't want it as `Action?`
        }

        IHitTestZone zone;
        if (rect.HasValue)
        {
            zone = new HitTestZone(rect.Value, depth, null, DoNothing, true);
        }
        else
        {
            zone = new InfiniteHitTestZone(depth, null, DoNothing, true);
        }

        var hitTestStack = new HitTestStack(WorldMatrix * layerMatrix);
        _zones.Add(new NestedHitTestZone(hitTestStack, zone));
        return hitTestStack;
    }

    private HitTestStack AddLayer(Matrix layerMatrix, IHitTestZone zone)
    {
        var hitTestStack = new HitTestStack(WorldMatrix * layerMatrix);
        _zones.Add(new NestedHitTestZone(hitTestStack, zone));
        return hitTestStack;
    }
}
