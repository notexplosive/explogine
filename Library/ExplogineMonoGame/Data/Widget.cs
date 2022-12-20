using System;
using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class Widget : IDisposable
{
    public Widget(Vector2 position, Point size)
    {
        Position = position;
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
        painter.DrawAtPosition(Texture, Position);
    }
}
