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
        var origin = new DrawOrigin(_box.Offset.ToVector2());
        painter.DrawRectangle(new Rectangle(Transform.Position.ToPoint(), _box.Size),
            new DrawSettings
                {Color = Color, Angle = Transform.Angle, Origin = origin, Depth = Transform.Depth});
    }

    public override void DebugDraw(Painter painter)
    {
        var origin = new DrawOrigin(_box.Offset.ToVector2());
        painter.DrawRectangle(new Rectangle(Transform.Position.ToPoint(), _box.Size),
            new DrawSettings
                {Color = Color, Angle = Transform.Angle, Origin = origin, Depth = Transform.Depth});
    }
}
