using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Gui;
using ExplogineMonoGame.Input;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public class ScrollableArea : IUpdateInputHook
{
    private readonly Scrollbar _horizontalScrollbar;
    private readonly Scrollbar _verticalScrollbar;
    private Vector2 _viewPosition;

    public ScrollableArea(Point canvasSize, RectangleF innerWorldBoundaries, Depth scrollbarHitTestDepth)
    {
        CanvasSize = canvasSize;
        InnerWorldBoundaries = innerWorldBoundaries;

        _verticalScrollbar = new Scrollbar(this, Orientation.Vertical, scrollbarHitTestDepth, ScrollBarWidth);
        _horizontalScrollbar = new Scrollbar(this, Orientation.Horizontal, scrollbarHitTestDepth, ScrollBarWidth);
    }

    public XyBool EnabledAxes { get; set; } = XyBool.True;
    public Point CanvasSize { get; set; }
    public RectangleF InnerWorldBoundaries { get; set; }
    public RectangleF ViewBounds => new(_viewPosition, CanvasSize.ToVector2());
    public Matrix CanvasToScreen => ViewBounds.CanvasToScreen(CanvasSize);
    public Matrix ScreenToCanvas => ViewBounds.ScreenToCanvas(CanvasSize);
    public int ScrollBarWidth => 32;

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        if (EnabledAxes.X)
        {
            _horizontalScrollbar.UpdateInput(input, hitTestStack);
        }
        
        if (EnabledAxes.Y)
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

    public void DrawScrollbars(Painter painter, IGuiTheme theme)
    {
        if (EnabledAxes.X)
        {
            theme.DrawScrollbar(painter, _horizontalScrollbar);
        }

        if (EnabledAxes.Y)
        {
            theme.DrawScrollbar(painter, _verticalScrollbar);
        }
    }

    public void ReConstrain()
    {
        _viewPosition = ViewBounds.ConstrainedTo(InnerWorldBoundaries).TopLeft;
    }

    public bool CanScrollAlong(Axis scrollableAxis)
    {
        return ViewBounds.Size.GetAxis(scrollableAxis) <
            InnerWorldBoundaries.Size.GetAxis(scrollableAxis);
    }

    public void DoScrollWithMouseWheel(ConsumableInput input, float increment = 25)
    {
        var delta = -input.Mouse.ScrollDelta() / 120f * increment;
        if (delta != 0)
        {
            SetPosition(_viewPosition + new Vector2(0,delta));
        }
    }
}
