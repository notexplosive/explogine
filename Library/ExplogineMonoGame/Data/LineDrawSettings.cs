using ExplogineCore.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public struct LineDrawSettings : IBasicDrawSettings
{
    public LineDrawSettings()
    {
        Depth = default;
        Color = Color.White;
        Thickness = 1f;
    }

    public float Thickness { get; set; }
    public Color Color { get; set; }
    public Depth Depth { get; set; }
}
