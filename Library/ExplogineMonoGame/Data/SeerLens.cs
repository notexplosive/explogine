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

    /// <summary>
    ///     Deflates the ViewRect centered on a focus point such that the focus point is at the same relative position before
    ///     and after the deflation.
    /// </summary>
    /// <param name="viewBounds">Starting view bounds</param>
    /// <param name="zoomAmount">Amount to deflate the viewBounds by</param>
    /// <param name="focusPosition">Position to zoom towards in WorldSpace (aka: the same space as the ViewBounds rect)</param>
    /// <returns></returns>
    public static RectangleF GetZoomedInBounds(RectangleF viewBounds, float zoomAmount, Vector2 focusPosition)
    {
        var focusRelativeToViewBounds = focusPosition - viewBounds.Location;
        var relativeScalar = focusRelativeToViewBounds.StraightDivide(viewBounds.Width, viewBounds.Height);
        var zoomedInBounds = viewBounds.Inflated(-zoomAmount, -zoomAmount);

        // center zoomed in bounds on focus
        zoomedInBounds.Location = focusPosition - zoomedInBounds.Size / 2f;

        // offset zoomed in bounds so focus is in the same relative spot
        var focusRelativeToZoomedInBounds =
            relativeScalar.StraightMultiply(zoomedInBounds.Width, zoomedInBounds.Height);
        var newFocusPosition = focusRelativeToZoomedInBounds + zoomedInBounds.Location;
        var oldFocusPosition = focusRelativeToViewBounds + viewBounds.Location;
        zoomedInBounds.Offset(oldFocusPosition - newFocusPosition);

        return zoomedInBounds;
    }

    /// <summary>
    ///     Inflates the ViewRect centered on a focus point such that the focus point is at the same relative position before
    ///     and after the deflation.
    /// </summary>
    /// <param name="viewBounds">Starting view bounds</param>
    /// <param name="zoomAmount">Amount to deflate the viewBounds by</param>
    /// <param name="focusPosition">Position to zoom towards in WorldSpace (aka: the same space as the ViewBounds rect)</param>
    /// <returns></returns>
    public static RectangleF GetZoomedOutBounds(RectangleF viewBounds, float zoomAmount, Vector2 focusPosition)
    {
        var zoomedInBounds = SeerLens.GetZoomedInBounds(viewBounds, zoomAmount, focusPosition);
        var zoomedOutOffset = viewBounds.Center - zoomedInBounds.Center;
        var zoomedOutBounds = zoomedInBounds.Inflated(zoomAmount * 2, zoomAmount * 2);
        zoomedOutBounds.Offset(zoomedOutOffset * 2);
        return zoomedOutBounds;
    }
}
