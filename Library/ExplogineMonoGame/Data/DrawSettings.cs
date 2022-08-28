using ExplogineCore.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public struct DrawSettings
{
    public DrawSettings()
    {
        Depth = default;
        Angle = 0;
        Origin = default;
        Flip = default;
        Color = Color.White;
        SourceRectangle = null;
    }

    public Color Color { get; set; }
    public Rectangle? SourceRectangle { get; set; }
    public Depth Depth { get; set; }
    public float Angle { get; set; }
    public DrawOrigin Origin { get; set; }
    public XyBool Flip { get; set; }

    public SpriteEffects FlipEffect => (Flip.X ? SpriteEffects.FlipHorizontally : SpriteEffects.None) |
                                       (Flip.Y ? SpriteEffects.FlipVertically : SpriteEffects.None);
}
