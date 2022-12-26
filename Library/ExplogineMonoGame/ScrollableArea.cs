using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Gui;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public class ScrollableArea : IUpdateInput
{
    private readonly Scrollbar _horizontalScrollbar;
    private readonly Scrollbar _verticalScrollbar;
    private Vector2 _viewPosition;

    public ScrollableArea(Point canvasSize, RectangleF innerWorldBoundaries, Depth scrollbarHitTestDepth)
    {
        CanvasSize = canvasSize;
        InnerWorldBoundaries = innerWorldBoundaries;

        _verticalScrollbar = new Scrollbar(this, Orientation.Vertical, scrollbarHitTestDepth);
        _horizontalScrollbar = new Scrollbar(this, Orientation.Horizontal, scrollbarHitTestDepth);
    }

    public XyBool EnableInput { get; set; } = XyBool.True;
    public Point CanvasSize { get; set; }
    public RectangleF InnerWorldBoundaries { get; set; }
    public RectangleF ViewBounds => new(_viewPosition, CanvasSize.ToVector2());
    public Matrix CanvasToScreen => ViewBounds.CanvasToScreen(CanvasSize);
    public Matrix ScreenToCanvas => ViewBounds.ScreenToCanvas(CanvasSize);
    public int ScrollBarWidth => 32;

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        if (EnableInput.X)
        {
            _horizontalScrollbar.UpdateInput(input, hitTestStack);
        }
        
        if (EnableInput.Y)
        {
            _verticalScrollbar.UpdateInput(input, hitTestStack);
        }

    }

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
        if (EnableInput.X)
        {
            theme.DrawScrollbar(painter, _horizontalScrollbar);
        }

        if (EnableInput.Y)
        {
            theme.DrawScrollbar(painter, _verticalScrollbar);
        }
    }

    public void ReConstrain()
    {
        _viewPosition = ViewBounds.ConstrainedTo(InnerWorldBoundaries).TopLeft;
    }
}
