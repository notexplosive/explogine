using System.Collections.Generic;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public class HitTestStack
{
    private readonly List<HitTestTarget> _list = new();

    public HitTestTarget? LastTopHit { get; private set; }

    public void Add(HitTestTarget target)
    {
        _list.Add(target);
    }

    public void Resolve(Vector2 position)
    {
        LastTopHit = GetTopHit(position);
    }

    public HitTestTarget? GetTopHit(Vector2 position)
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
    }
}
