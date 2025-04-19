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
    public ClientCanvas(RealWindow window)
    {
        Window = window;
    }

    public Canvas Internal { get; private set; } = null!;

    public Matrix CanvasToScreen => Matrix.CreateScale(new Vector3(
                                        new Vector2(PointExtensions.CalculateScalarDifference(Window.Size,
                                            Window.RenderResolution)), 1))
                                    * Matrix.CreateTranslation(new Vector3(CalculateTopLeftCorner(), 0));

    public Matrix ScreenToCanvas => Matrix.Invert(CanvasToScreen);
    public Texture2D Texture => Internal.Texture;
    public Point Size => Internal.Size;

    public RealWindow Window { get; }

    public void ResizeCanvas(Point newRenderResolution)
    {
        if (Internal.Size == newRenderResolution)
        {
            return;
        }

        Internal.Dispose();
        Internal = new Canvas(newRenderResolution);
    }

    public void Setup()
    {
        Internal = new Canvas(1, 1);
    }

    public void Draw(Painter painter)
    {
        painter.BeginSpriteBatch(CanvasToScreen);
        painter.DrawAtPosition(Internal.Texture, Vector2.Zero);
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
