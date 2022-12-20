using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Gui;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public class ScrollableArea : IUpdateInput
{
    private Vector2 _viewPosition;
    private Scrollbar _verticalScrollbar;
    private Scrollbar _horizontalScrollbar;

    public ScrollableArea(Point canvasSize, RectangleF innerWorldBoundaries)
    {
        CanvasSize = canvasSize;
        InnerWorldBoundaries = innerWorldBoundaries;
        
        _verticalScrollbar = new Scrollbar(this, Orientation.Vertical, Depth.Middle);
        _horizontalScrollbar = new Scrollbar(this, Orientation.Horizontal, Depth.Middle);
    }

    public Point CanvasSize { get; set; }
    public RectangleF InnerWorldBoundaries { get; set; }
    public RectangleF ViewBounds => new(_viewPosition, CanvasSize.ToVector2());
    public Matrix CanvasToScreen => ViewBounds.CanvasToScreen(CanvasSize);
    public Matrix ScreenToCanvas => ViewBounds.ScreenToCanvas(CanvasSize);

    public void Move(Vector2 offset)
    {
        _viewPosition = ViewBounds.Moved(offset).ConstrainedTo(InnerWorldBoundaries).TopLeft;
    }

    public void SetPosition(Vector2 position)
    {
        _viewPosition = (ViewBounds with {Location = position}).ConstrainedTo(InnerWorldBoundaries).TopLeft;
    }

    public void DrawScrollbars(Painter painter, SimpleGuiTheme theme)
    {
        theme.DrawScrollbar(painter, _verticalScrollbar);
        theme.DrawScrollbar(painter, _horizontalScrollbar);
    }

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        _verticalScrollbar.UpdateInput(input, hitTestStack);
        _horizontalScrollbar.UpdateInput(input, hitTestStack);
    }
}
