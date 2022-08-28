using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Cartridges;

public readonly struct CartridgeConfig
{
    public Point? RenderResolution { get; }

    public CartridgeConfig(Point? renderResolution)
    {
        RenderResolution = renderResolution;
    }
}
