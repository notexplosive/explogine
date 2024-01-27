using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Cartridges;

public readonly struct CartridgeConfig
{
    /// <summary>
    ///     The desired locked resolution of this cartridge.
    ///     If set to 1600x900, we will render the game to a 1600x900 canvas and letterbox the window where appropriate.
    ///     If set to null, we will stretch the rendered game canvas to be 1:1 with the window.
    /// </summary>
    public Point? RenderResolution { get; }

    public CartridgeConfig(Point? renderResolution, SamplerState? samplerState = null)
    {
        RenderResolution = renderResolution;
        SamplerState = samplerState;
    }

    public SamplerState? SamplerState { get; }
}
