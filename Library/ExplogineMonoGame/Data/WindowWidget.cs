using ExplogineCore.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public class WindowWidget : Widget, IWindow
{
    public WindowWidget(RectangleF rectangle, Depth depth, Point? renderResolution = null) : base(rectangle, depth, renderResolution)
    {
    }

    public WindowWidget(Vector2 position, Point size, Depth depth, Point? renderResolution = null) : base(position, size, depth, renderResolution)
    {
    }

    public bool IsInFocus => true;
    public bool IsFullscreen => false;
    public void SetRenderResolution(Point? optionalSize)
    {
        RenderResolution = optionalSize ?? Size;
    }

    public void SetFullscreen(bool toggle)
    {
        Client.Debug.LogWarning("SetFullscreen is not supported on Widgets");
    }
}
