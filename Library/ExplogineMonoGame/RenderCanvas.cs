using System;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public class RenderCanvas
{
    public Canvas Canvas { get; private set; } = null!;

    public Matrix CanvasToScreenScalar => Matrix.CreateScale(new Vector3(
        new Vector2(PointExtensions.CalculateScalarDifference(Client.Window.Size,
            Client.Window.RenderResolution)), 1));

    public Matrix ScreenToCanvasScalar => Matrix.Invert(CanvasToScreenScalar);
    
    public Matrix CanvasToScreen => CanvasToScreenScalar
                                    * Matrix.CreateTranslation(new Vector3(CalculateTopLeftCorner(), 0));

    public Matrix ScreenToCanvas => Matrix.Invert(Client.RenderCanvas.CanvasToScreen);

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
        drawAction(Client.Graphics.Painter);
        Client.Graphics.PopCanvas();
    }

    public void Draw(Painter painter)
    {
        // this renders the whole canvas, does it need to be the same SamplerState as everything else?
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
        var canvasWidth =
            Client.Window.RenderResolution.X * scalar;
        var canvasHeight =
            Client.Window.RenderResolution.Y * scalar;

        var result = new Vector2(
            Client.Window.Size.X / 2f - canvasWidth / 2,
            Client.Window.Size.Y / 2f - canvasHeight / 2
        );

        if (windowIsTooWide)
        {
            return new Vector2(result.X, 0);
        }

        return new Vector2(0, result.Y);
    }
}
