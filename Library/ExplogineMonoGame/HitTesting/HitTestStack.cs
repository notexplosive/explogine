using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.HitTesting;

public class HitTestStack
{
    private readonly List<HitTestTarget> _list = new();
    private int _descriptorIndex;

    public event Action<HitTestDescriptor>? Resolved;

    private HitTestTarget AddTarget(HitTestTarget target)
    {
        _list.Add(target);
        return target;
    }

    public void Resolve(Vector2 position)
    {
        var hit = GetTopHit(position);
        if (hit != null)
        {
            Resolved?.Invoke(hit.Value.Descriptor);
        }
    }

    private HitTestTarget? GetTopHit(Vector2 position)
    {
        _list.Sort((x, y) => x.Depth - y.Depth);

        // First pass, look for just debug overlay items, anything in the debug overlay gets first dibs
        foreach (var item in _list)
        {
            if (item.Layer == HitTestLayer.DebugOverlay && item.Contains(position))
            {
                return item;
            }
        }

        // Second pass, allow anything
        foreach (var item in _list)
        {
            if (item.Contains(position))
            {
                return item;
            }
        }

        return null;
    }

    public void Clear()
    {
        _list.Clear();
        _descriptorIndex = 0;
    }

    public HitTestDescriptor Add(Rectangle rect, Depth depth, HitTestLayer layer = HitTestLayer.Game)
    {
        return AddTarget(new HitTestTarget(new HitTestDescriptor(_descriptorIndex++), rect, depth, layer)).Descriptor;
    }

    public HitTestDescriptor Add(Rectangle rect, Depth depth, Matrix worldMatrix)
    {
        // Assumes that if you're using WorldMatrix you don't care about debug layer.
        // Debug layer is always in screen-space
        return AddTarget(new HitTestTarget(new HitTestDescriptor(_descriptorIndex++), rect, depth, HitTestLayer.Game,
            worldMatrix)).Descriptor;
    }
}