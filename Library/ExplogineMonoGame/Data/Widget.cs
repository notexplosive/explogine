using System;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class Widget : IDisposable
{
    public Widget(Vector2 position, Point size, Depth depth)
    {
        Position = position;
        Depth = depth;
        Canvas = new Canvas(size);
    }

    public Vector2 Position { get; }

    public Point Size
    {
        get => Canvas.Size;
        set => ResizeCanvas(value);
    }

    public Texture2D Texture => Canvas.Texture;
    public Canvas Canvas { get; private set; }

    public Matrix CanvasToScreen => Matrix.CreateTranslation(new Vector3(Position, 0));
    public Matrix ScreenToCanvas => Matrix.Invert(CanvasToScreen);
    public RectangleF Rectangle => new(Position, Size.ToVector2());
    public Depth Depth { get; set; }
    public HoverState IsHovered { get; } = new();

    public void Dispose()
    {
        Canvas.Dispose();
    }

    public void ResizeCanvas(Point newSize)
    {
        if (Canvas.Size == newSize)
        {
            return;
        }

        Canvas.Dispose();
        Canvas = new Canvas(newSize);
    }

    public void Draw(Painter painter)
    {
        painter.DrawAtPosition(Texture, Position, Scale2D.One, new DrawSettings {Depth = Depth});
    }

    public void UpdateHovered(HitTestStack hitTestStack)
    {
        hitTestStack.AddZone(Rectangle, Depth, IsHovered, true);
    }
}
