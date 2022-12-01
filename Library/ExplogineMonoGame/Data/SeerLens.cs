using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class SeerLens
{
    // this should go on RectangleF
    public static Matrix RenderMatrix(RectangleF viewBounds, Point outputDimensions)
    {
        var translation =
            Matrix.Invert(Matrix.CreateTranslation(new Vector3(viewBounds.Location, 0)));
        return translation * SeerLens.RenderMatrixScalar(viewBounds, outputDimensions);
    }

    // this should go on RectangleF
    public static Matrix RenderMatrixScalar(RectangleF viewBounds, Point outputDimensions)
    {
        return Matrix.CreateScale(new Vector3(outputDimensions.X / viewBounds.Width,
            outputDimensions.Y / viewBounds.Height, 1));
    }

    // This should go on Painter
    public static void Begin(Painter painter, RectangleF viewBounds, Point outputDimensions)
    {
        painter.BeginSpriteBatch(SamplerState.LinearWrap, SeerLens.RenderMatrix(viewBounds, outputDimensions));
        painter.Clear(Color.LightBlue);
    }

    public static void End(Painter painter)
    {
        painter.EndSpriteBatch();
    }
}
