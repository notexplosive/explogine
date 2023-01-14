using System.Collections.Generic;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public class HitTestRoot
{
    public HitTestStack BaseStack { get; } = new(Matrix.Identity);

    public void Resolve(Vector2 position)
    {
        BaseStack.OnBeforeResolve();
        
        if (!Client.IsInFocus)
        {
            return;
        }

        foreach (var zone in BaseStack.GetZonesAt(position))
        {
            zone.Callback.Invoke();
        }
    }
}
