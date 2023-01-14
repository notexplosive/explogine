using System;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class Widget : IDisposable, IDrawHook
{
    public Widget(RectangleF rectangle, Depth depth) : this(rectangle.Location, rectangle.Size.ToPoint(), depth)
    {
    }

    public Widget(Vector2 position, Point size, Depth depth)
    {
        Position = position;
        Depth = depth;
        Canvas = new Canvas(size);
    }

    public Vector2 Position { get; set; }

    public Point Size
    {
        get => Canvas.Size;
        set => ResizeCanvas(value);
    }

    public Texture2D Texture => Canvas.Texture;
    public Canvas Canvas { get; private set; }
    public Matrix CanvasToScreen => Matrix.CreateTranslation(new Vector3(Position, 0));
    public Matrix ScreenToCanvas => Matrix.Invert(CanvasToScreen);

    public RectangleF Rectangle
    {
        get => new(Position, Size.ToVector2());
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
        painter.DrawAtPosition(Texture, Position, Scale2D.One, new DrawSettings {Depth = Depth});
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
        hitTestStack.AddZone(Rectangle, Depth, IsHovered, true);
    }

    public event Action? Resized;
}

public class WindowWidget : Widget, IWindow
{
    public WindowWidget(RectangleF rectangle, Depth depth) : base(rectangle, depth)
    {
    }

    public WindowWidget(Vector2 position, Point size, Depth depth) : base(position, size, depth)
    {
    }

    public Point RenderResolution => Size;
    public bool IsInFocus => true;
    public bool IsFullscreen => false;
    public void SetRenderResolution(Point? optionalSize)
    {
        Client.Debug.LogWarning("SetRenderResolution is not supported on Widgets");
    }

    public void SetFullscreen(bool toggle)
    {
        Client.Debug.LogWarning("SetFullscreen is not supported on Widgets");
    }
}
