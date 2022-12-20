using System.Collections.Generic;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

internal class HitTestRoot
{
    public HitTestStack BaseStack { get; } = new(Matrix.Identity);

    public void Resolve(Vector2 position)
    {
        if (!Client.IsInFocus)
        {
            return;
        }

        BaseStack.OnBeforeResolve();
        foreach (var zone in BaseStack.GetZonesAt(position))
        {
            zone.Callback.Invoke();
        }
    }
}
