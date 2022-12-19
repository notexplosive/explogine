using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public class ScrollableArea
{
    private Vector2 _viewPosition;

    public ScrollableArea(Point canvasSize, RectangleF innerWorldBoundaries)
    {
        CanvasSize = canvasSize;
        InnerWorldBoundaries = innerWorldBoundaries;
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
        _viewPosition = (ViewBounds with {Location = Vector2.Zero}).ConstrainedTo(InnerWorldBoundaries).TopLeft;
    }
}
