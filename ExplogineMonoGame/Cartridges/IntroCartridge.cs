using ExplogineMonoGame.Data;
using ExTween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Cartridges;

public class IntroCartridge : ICartridge
{
    private Font? _logoFont;
    private SequenceTween _tween = new();

    public void OnCartridgeStarted()
    {
        Client.Debug.Log("Intro loaded");
        _logoFont = Client.Assets.GetFont("engine/console-font", 25);
    }

    public void Update(float dt)
    {
    }

    public void Draw(Painter painter)
    {
        painter.BeginSpriteBatch(SamplerState.LinearClamp);
        painter.Clear(Color.Navy);

        var centerOfScreen = (Client.Window.RenderResolution.ToVector2() / 2).ToPoint();

        var text = "NotExplosive.net";
        painter.FillRectangle(new Rectangle(centerOfScreen, _logoFont!.MeasureString(text).ToPoint()), Color.Orange,
            200);
        painter.DrawStringAtPosition(_logoFont!, text, centerOfScreen, new DrawSettings {Origin = DrawOrigin.Center});

        painter.EndSpriteBatch();
    }

    public bool ShouldLoadNextCartridge()
    {
        return false;
    }
}
