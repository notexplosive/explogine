using System;
using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class SeerLens
{
    public SeerLens(RectangleF rectangleF)
    {
        Rectangle = rectangleF;
    }

    public RectangleF Rectangle { get; set; }

    public void Render(Painter painter, Canvas canvas, Action<Painter> drawContent)
    {
        Client.Graphics.PushCanvas(canvas);
        var translation = 
            Matrix.Invert(Matrix.CreateTranslation(new Vector3(Rectangle.Location, 0)));
        var scale = 
            Matrix.CreateScale(new Vector3(canvas.Size.X / Rectangle.Width, canvas.Size.Y / Rectangle.Height, 1));
        painter.BeginSpriteBatch(SamplerState.LinearWrap, translation * scale);
        painter.Clear(Color.LightBlue);
        drawContent(painter);
        painter.EndSpriteBatch();
        Client.Graphics.PopCanvas();
    }
}
