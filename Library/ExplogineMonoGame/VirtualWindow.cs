using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public class VirtualWindow : IWindow
{
    public Point RenderResolution { get; private set; } = new(1920, 1080);
    public bool IsInFocus => true;
    public Point Size { get; set; } = new(1920, 1080);
    public bool IsFullscreen { get; private set; }
    public Matrix ScreenToCanvas => Matrix.Identity;
    public Matrix CanvasToScreen => Matrix.Identity;

    /// <summary>
    ///     This is the only unsafe thing to ask a VirtualWindow for
    /// </summary>
    public Canvas Canvas => null!;

    public void SetRenderResolution(CartridgeConfig config)
    {
        var renderResolution = config.RenderResolution;
        RenderResolution = renderResolution ?? new Point(1920, 1080);
        // this does not set SamplerState.
    }

    public void SetFullscreen(bool toggle)
    {
        IsFullscreen = toggle;
    }
}
