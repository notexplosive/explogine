using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

/// <summary>
///     Represents either a window (could be the "Real" OS window, a Phone Screen, or a Virtual Window)
/// </summary>
public interface IWindow
{
    Point RenderResolution { get; }
    bool IsInFocus { get; }
    Point Size { get; }
    bool IsFullscreen { get; }
    Matrix ScreenToCanvas { get; }
    Matrix CanvasToScreen { get; }
    Canvas Canvas { get; }
    void SetRenderResolution(Point? optionalSize);
    void SetFullscreen(bool toggle);
}