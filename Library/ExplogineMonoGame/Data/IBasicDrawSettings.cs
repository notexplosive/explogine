using ExplogineCore.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public interface IBasicDrawSettings
{
    public Color Color { get; set; }
    public Depth Depth { get; set; }
}
