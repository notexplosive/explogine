using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace MachinaLite.Components;

public class Hoverable : BaseComponent
{
    private readonly Box _box;

    public Hoverable(Actor actor) : base(actor)
    {
        _box = RequireComponent<Box>();
    }

    public bool IsHovered { get; private set; }

    public event Action? Hovered;

    public override void OnMouseUpdate(Vector2 currentPosition, Vector2 worldDelta, Vector2 rawDelta,
        HitTestStack hitTestStack)
    {
        IsHovered = false;
        if (Actor.Visible)
        {
            hitTestStack.AddZone(_box.Rectangle, Transform.Depth, OnHovered);
        }
    }

    private void OnHovered()
    {
        IsHovered = true;
        Hovered?.Invoke();
    }
}
