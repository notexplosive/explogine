using System;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class Widget : IDisposable, IDrawHook
{
    public Widget(RectangleF rectangle, Depth depth, Point? renderResolution = null) : this(rectangle.Location,
        rectangle.Size.ToPoint(), depth, renderResolution)
    {
    }

    public Widget(Vector2 position, Point size, Depth depth, Point? renderResolution = null)
    {
        Position = position;
        Depth = depth;
        Canvas = new Canvas(renderResolution ?? size);
        Size = size;
    }

    public Vector2 Position { get; set; }

    /// <summary>
    ///     Size of the widget in the world
    /// </summary>
    public Point Size { get; set; }

    public Texture2D Texture => Canvas.Texture;
    public Canvas Canvas { get; private set; }

    public Matrix CanvasToScreen =>
        ContentRectangle.CanvasToScreen(Size) * Matrix.CreateTranslation(new Vector3(Position, 0));

    public Matrix ScreenToCanvas => Matrix.Invert(CanvasToScreen);

    /// <summary>
    ///     The number of pixels the canvas is capable of rendering, not necessarily how big it is to be rendered
    /// </summary>
    public Point RenderResolution
    {
        get => Canvas.Size;
        set => ResizeCanvas(value);
    }

    /// <summary>
    ///     The rectangle of the widget as it is rendered in the world
    /// </summary>
    public RectangleF OutputRectangle
    {
        get => new(Position, Size.ToVector2());
        set
        {
            Position = value.Location;
            Size = value.Size.ToPoint();
        }
    }

    /// <summary>
    ///     The rectangle inside the widget, accounting for render resolution
    /// </summary>
    public RectangleF ContentRectangle
    {
        get => new(Vector2.Zero, RenderResolution.ToVector2());
        set
        {
            Position = value.Location;
            Size = value.Size.ToPoint();
        }
    }

    public Depth Depth { get; set; }
    public HoverState IsHovered { get; } = new();

    public void Dispose()
    {
        Canvas.Dispose();
    }

    public void Draw(Painter painter)
    {
        painter.DrawAsRectangle(Texture, OutputRectangle, new DrawSettings {Depth = Depth});
    }

    public void ResizeCanvas(Point newSize)
    {
        if (Canvas.Size == newSize)
        {
            return;
        }

        Canvas.Dispose();
        Canvas = new Canvas(newSize);
        Resized?.Invoke();
    }

    public void UpdateHovered(HitTestStack hitTestStack)
    {
        hitTestStack.AddZone(OutputRectangle, Depth, IsHovered, true);
    }

    public event Action? Resized;
}
