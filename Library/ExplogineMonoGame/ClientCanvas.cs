using System;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame;

/// <summary>
/// Formerly known as RenderCanvas but that name sucks
/// </summary>
public class ClientCanvas
{
    private Canvas _canvas = null!;

    public Matrix CanvasToScreen => Matrix.CreateScale(new Vector3(
                                        new Vector2(PointExtensions.CalculateScalarDifference(Client.Window.Size,
                                            Client.Window.RenderResolution)), 1))
                                    * Matrix.CreateTranslation(new Vector3(CalculateTopLeftCorner(), 0));

    public Matrix ScreenToCanvas => Matrix.Invert(Client.Canvas.CanvasToScreen);
    public Texture2D Texture => _canvas.Texture;
    public Point Size => _canvas.Size;

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
            PointExtensions.IsEnclosingSizeTooWide(Client.Window.Size, Client.Window.RenderResolution);

        var scalar =
            PointExtensions.CalculateScalarDifference(Client.Window.Size, Client.Window.RenderResolution);
        var canvasSize = Client.Window.RenderResolution.ToVector2() * scalar;
        var result = (Client.Window.Size.ToVector2() - canvasSize) / 2;

        return windowIsTooWide ? new Vector2(result.X, 0) : new Vector2(0, result.Y);
    }
}
