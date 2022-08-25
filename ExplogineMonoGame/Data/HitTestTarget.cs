using ExplogineCore.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public enum HitTestLayer
{
    Game,
    DebugOverlay
}

public readonly record struct HitTestTarget(Rectangle Rectangle, Depth Depth, HitTestLayer Layer, Matrix WorldMatrix)
{
    public HitTestTarget(Rectangle rectangle, Depth depth) : this(rectangle, depth, HitTestLayer.Game, Matrix.Identity)
    {
    }
    
    public HitTestTarget(Rectangle rectangle, Depth depth, Matrix matrix) : this(rectangle, depth, HitTestLayer.Game, matrix)
    {
    }
    
    public HitTestTarget(Rectangle rectangle, Depth depth, HitTestLayer layer) : this(rectangle, depth, layer, Matrix.Identity)
    {
    }

    public bool Contains(Vector2 position)
    {
        return Rectangle.Contains(Vector2.Transform(position, WorldMatrix));
    }
}
