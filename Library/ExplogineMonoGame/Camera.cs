using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public class Camera
{
    public Camera(RectangleF viewBounds)
    {
        ViewBounds = viewBounds;
    }

    public Camera(Vector2 viewableSize) : this(new RectangleF(Vector2.Zero, viewableSize))
    {
    }

    public float Angle { get; set; }

    private RectangleF ViewBounds
    {
        get => new(TopLeftPosition, ViewableSize);
        set
        {
            TopLeftPosition = value.TopLeft;
            ViewableSize = value.Size;
        }
    }

    public Matrix CanvasToScreen => ViewBounds.CanvasToScreen(Client.Window.RenderResolution, Angle);
    public Matrix ScreenToCanvas => Matrix.Invert(CanvasToScreen);
    public Vector2 TopLeftPosition { get; set; }
    public Vector2 ViewableSize { get; set; }
    public Vector2 CenterPosition => TopLeftPosition + ViewableSize / 2;

    public void ZoomInTowards(int amount, Vector2 mousePos)
    {
        var newBounds = ViewBounds.GetZoomedInBounds(amount, mousePos);
        if (newBounds.Width > amount * 2 && newBounds.Height > amount * 2)
        {
            TopLeftPosition = newBounds.Location;
            ViewableSize = newBounds.Size;
        }
    }

    public void ZoomOutFrom(int amount, Vector2 mousePos)
    {
        var newBounds = ViewBounds.GetZoomedOutBounds(amount, mousePos);
        TopLeftPosition = newBounds.Location;
        ViewableSize = newBounds.Size;
    }
}
