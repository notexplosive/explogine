using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace MachinaLite.Components;


public class BoxRenderer : BaseComponent
{
    private readonly Box _box;

    public BoxRenderer(Actor actor) : base(actor)
    {
        _box = RequireComponent<Box>();
    }

    public Color Color { get; set; } = Color.White;

    public override void Draw(Painter painter)
    {
        painter.DrawRectangle(_box.Rectangle, new DrawSettings {Color = Color});
    }
}

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