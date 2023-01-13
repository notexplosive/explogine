using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame;

/// <summary>
/// Represents either a window (could be the "Real" OS window, a Phone Screen, or a Virtual Window)
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
    void SetCursor(MouseCursor cursor);
    void SetRenderResolution(Point? optionalSize);
    void SetFullscreen(bool toggle);
}
