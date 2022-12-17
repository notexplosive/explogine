using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.HitTesting;

public class HitTestStack
{
    private readonly List<HitTestZone> _list = new();
    private readonly List<Action> _beforeResolveEvents = new();

    private HitTestZone AddTarget(HitTestZone zone)
    {
        _list.Add(zone);
        return zone;
    }

    public void Resolve(Vector2 position)
    {
        if (!Client.IsInFocus)
        {
            return;
        }
        
        foreach (var beforeResolve in _beforeResolveEvents)
        {
            beforeResolve();
        }

        var hit = GetTopHit(position);
        if (hit != null)
        {
            hit.Value.Callback?.Invoke();
        }
    }

    private HitTestZone? GetTopHit(Vector2 position)
    {
        _list.Sort((x, y) => x.Depth - y.Depth);

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
        _beforeResolveEvents.Clear();
    }

    public void Add(RectangleF rect, Depth depth, Action? callback = null)
    {
        AddTarget(new HitTestZone(rect, depth, callback));
    }

    public void AddBeforeResolve(Action callback)
    {
        _beforeResolveEvents.Add(callback);
    }
}
