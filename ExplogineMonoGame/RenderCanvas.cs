using System;
using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame;

public class RenderCanvas
{
    private Canvas _internalCanvas = null!;

    public void OnWindowResized(Point newWindowSize)
    {
        if (_internalCanvas.Size == Client.Window.RenderResolution)
        {
            return;
        }

        _internalCanvas.Dispose();
        _internalCanvas = new Canvas(Client.Window.RenderResolution);
    }

    public void Setup()
    {
        _internalCanvas = new Canvas(1, 1);
    }

    public void DrawWithin(Action<Painter> drawAction)
    {
        Client.Graphics.PushCanvas(_internalCanvas);
        drawAction(Client.Graphics.Painter);
        Client.Graphics.PopCanvas();
    }

    public void Draw(Painter painter)
    {
        // this renders the whole canvas, does it need to be the same SamplerState as everything else?
        painter.BeginSpriteBatch(SamplerState.LinearWrap);
        painter.DrawAtPosition(_internalCanvas.Texture, Vector2.Zero);
        painter.EndSpriteBatch();
    }
}
