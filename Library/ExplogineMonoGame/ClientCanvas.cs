using System;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame;

/// <summary>
///     Formerly known as RenderCanvas but that name sucks
/// </summary>
public class ClientCanvas
{
    private Canvas _canvas = null!;

    public ClientCanvas(PlatformAgnosticWindow window)
    {
        Window = window;
    }

    public Matrix CanvasToScreen => Matrix.CreateScale(new Vector3(
                                        new Vector2(PointExtensions.CalculateScalarDifference(Window.Size,
                                            Window.RenderResolution)), 1))
                                    * Matrix.CreateTranslation(new Vector3(CalculateTopLeftCorner(), 0));

    public Matrix ScreenToCanvas => Matrix.Invert(CanvasToScreen);
    public Texture2D Texture => _canvas.Texture;
    public Point Size => _canvas.Size;

    public PlatformAgnosticWindow Window { get; }

    public void ResizeCanvas(Point newWindowSize)
    {
        if (_canvas.Size == newWindowSize)
        {
            return;
        }

        _canvas.Dispose();
        _canvas = new Canvas(newWindowSize);
    }

    public void Setup()
    {
        _canvas = new Canvas(1, 1);
    }

    public void DrawWithin(Action<Painter> drawAction)
    {
        Client.Graphics.PushCanvas(_canvas);
        Client.Graphics.Painter.Clear(Color.Black);
        drawAction(Client.Graphics.Painter);
        Client.Graphics.PopCanvas();
    }

    public void Draw(Painter painter)
    {
        painter.BeginSpriteBatch(CanvasToScreen);
        painter.DrawAtPosition(_canvas.Texture, Vector2.Zero);
        painter.EndSpriteBatch();
    }

    public Vector2 CalculateTopLeftCorner()
    {
        var windowIsTooWide =
            PointExtensions.IsEnclosingSizeTooWide(Window.Size, Window.RenderResolution);

        var scalar =
            PointExtensions.CalculateScalarDifference(Window.Size, Window.RenderResolution);
        var canvasSize = Window.RenderResolution.ToVector2() * scalar;
        var result = (Window.Size.ToVector2() - canvasSize) / 2;

        return windowIsTooWide ? new Vector2(result.X, 0) : new Vector2(0, result.Y);
    }
}
