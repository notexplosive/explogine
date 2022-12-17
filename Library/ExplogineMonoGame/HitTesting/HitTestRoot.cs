using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.HitTesting;

internal class HitTestRoot
{
    public void Resolve(Vector2 position)
    {
        if (!Client.IsInFocus)
        {
            return;
        }
        
        BaseStack.OnBeforeResolve();

        var hit = BaseStack.GetTopZoneAt(position);
        hit?.Callback?.Invoke();
    }

    public HitTestStack BaseStack { get; } = new(Matrix.Identity);
}
