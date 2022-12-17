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

        var hit = BaseStack.GetTopZoneAt(position);
        hit?.Callback?.Invoke();
    }
}
