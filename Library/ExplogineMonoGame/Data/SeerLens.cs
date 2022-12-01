using System;
using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class SeerLens
{
    public static void Render(Painter painter, RectangleF viewBounds, Point outputDimensions, Action<Painter> drawContent)
    {
        var translation = 
            Matrix.Invert(Matrix.CreateTranslation(new Vector3(viewBounds.Location, 0)));
        var scale = 
            Matrix.CreateScale(new Vector3(outputDimensions.X / viewBounds.Width, outputDimensions.Y / viewBounds.Height, 1));
        painter.BeginSpriteBatch(SamplerState.LinearWrap, translation * scale);
        painter.Clear(Color.LightBlue);
        drawContent(painter);
        painter.EndSpriteBatch();
    }
}
