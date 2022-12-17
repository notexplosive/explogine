using ExplogineMonoGame.Data;
using ExTween;
using ExTweenMonoGame;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public class Camera
{
    public Camera(RectangleF viewBounds)
    {
        ViewBounds = viewBounds;
        TweenableViewBounds = new TweenableRectangleF(() => ViewBounds, val => ViewBounds = val);
        TweenableCenterPosition = new TweenableVector2(() => CenterPosition, val => CenterPosition = val);
        TweenableAngle = new TweenableFloat(() => Angle, val => Angle = val);
    }

    public Camera(Vector2 viewableSize) : this(new RectangleF(Vector2.Zero, viewableSize))
    {
    }

    public float Angle { get; set; }

    public RectangleF ViewBounds
    {
        get => new(TopLeftPosition, Size);
        set
        {
            TopLeftPosition = value.TopLeft;
            Size = value.Size;
        }
    }

    /// <summary>
    /// Typically used for Drawing
    /// </summary>
    public Matrix CanvasToScreen => ViewBounds.CanvasToScreen(Client.Window.RenderResolution, Angle);
    /// <summary>
    /// Typically used for Mouse Position -> World Position
    /// </summary>
    public Matrix ScreenToCanvas => Matrix.Invert(CanvasToScreen);
    public Vector2 TopLeftPosition { get; set; }
    public Vector2 Size { get; private set; }

    public Vector2 CenterPosition
    {
        get => TopLeftPosition + Size / 2;
        set => TopLeftPosition = value - Size / 2;
    }

    public TweenableRectangleF TweenableViewBounds { get; }
    public TweenableVector2 TweenableCenterPosition { get; }
    public TweenableFloat TweenableAngle { get; }

    public void ZoomInTowards(int amount, Vector2 focus)
    {
        var newBounds = ViewBounds.GetZoomedInBounds(amount, focus);
        if (newBounds.Width > amount * 2 && newBounds.Height > amount * 2)
        {
            TopLeftPosition = newBounds.Location;
            Size = newBounds.Size;
        }
    }

    public void ZoomOutFrom(int amount, Vector2 focus)
    {
        var newBounds = ViewBounds.GetZoomedOutBounds(amount, focus);
        TopLeftPosition = newBounds.Location;
        Size = newBounds.Size;
    }
}
