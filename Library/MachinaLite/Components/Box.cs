using Microsoft.Xna.Framework;

namespace MachinaLite.Components;

public class Box : BaseComponent
{
    public Box(Actor actor, Point size, Point? offset = null) : base(actor)
    {
        Size = size;
        Offset = offset ?? Point.Zero;
    }

    public Point Offset { get; set; }
    public Point Size { get; set; }
    public Rectangle Rectangle => new(Transform.Position.ToPoint() - Offset, Size);
}