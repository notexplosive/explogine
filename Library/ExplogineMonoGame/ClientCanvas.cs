using System;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

/// <summary>
/// Formerly known as RenderCanvas but that name sucks
/// </summary>
public class ClientCanvas
{
    public Canvas Canvas { get; private set; } = null!;

    public Matrix CanvasToScreen => Matrix.CreateScale(new Vector3(
                                        new Vector2(PointExtensions.CalculateScalarDifference(Client.Window.Size,
                                            Client.Window.RenderResolution)), 1))
                                    * Matrix.CreateTranslation(new Vector3(CalculateTopLeftCorner(), 0));

    public Matrix ScreenToCanvas => Matrix.Invert(Client.ClientCanvas.CanvasToScreen);

    public void ResizeCanvas(Point newWindowSize)
    {
        if (Canvas.Size == Client.Window.RenderResolution)
        {
            return;
        }

        Canvas.Dispose();
        Canvas = new Canvas(Client.Window.RenderResolution);
    }

    public void Setup()
    {
        Canvas = new Canvas(1, 1);
    }

    public void DrawWithin(Action<Painter> drawAction)
    {
        Client.Graphics.PushCanvas(Canvas);
        Client.Graphics.Painter.Clear(Color.Black);
        drawAction(Client.Graphics.Painter);
        Client.Graphics.PopCanvas();
    }

    public void Draw(Painter painter)
    {
        painter.BeginSpriteBatch(CanvasToScreen);
        painter.DrawAtPosition(Canvas.Texture, Vector2.Zero);
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
