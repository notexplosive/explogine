using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.HitTesting;

internal readonly record struct HitTestTarget(HitTestDescriptor Descriptor, RectangleF Rectangle, Depth Depth,
    HitTestLayer Layer,
    Matrix WorldMatrix)
{
    public HitTestTarget(HitTestDescriptor descriptor, RectangleF rectangle, Depth depth, HitTestLayer layer) : this(
        descriptor, rectangle, depth, layer,
        Matrix.Identity)
    {
    }

    public bool Contains(Vector2 position)
    {
        return Rectangle.Contains(Vector2.Transform(position, WorldMatrix));
    }
}
