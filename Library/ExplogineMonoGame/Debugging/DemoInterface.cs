using System;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Debugging;

internal class DemoInterface : IUpdateHook
{
    private readonly App _app;
    private float _totalTime;

    public DemoInterface(App app)
    {
        _app = app;
    }

    public void Draw(Painter painter, Depth depth)
    {
        var spriteSheet = Client.Assets.GetAsset<SpriteSheet>("demo-indicators");
        var consoleFont = Client.Assets.GetFont("engine/console-font", 24);
        var isPlaying = Client.Demo.IsPlaying;
        var isRecording = Client.Demo.IsRecording;

        if (isRecording || isPlaying)
        {
            var frame = 0;
            if (isRecording)
            {
                frame = 1;
            }

            if (MathF.Sin(_totalTime * 10) > 0)
            {
                var text = "Press ESC to stop";
                if (isRecording)
                {
                    text = "Press ^P to dump recording";
                }

                painter.DrawStringAtPosition(consoleFont, text,
                    new Vector2(
                        _app.Window.Size.X
                        - (int) consoleFont.MeasureString(text).X
                        , spriteSheet.GetSourceRectForFrame(0).Height),
                    new DrawSettings());

                spriteSheet.DrawFrameAtPosition(
                    painter,
                    frame,
                    new Vector2(_app.Window.Size.X - spriteSheet.GetSourceRectForFrame(0).Width, 0),
                    Scale2D.One,
                    new DrawSettings {Depth = depth});
            }
        }
    }

    public void Update(float dt)
    {
        _totalTime += dt;
        if (Client.Input.Keyboard.Modifiers.Control)
        {
            if (Client.Input.Keyboard.GetButton(Keys.P).WasPressed && !Client.Demo.IsPlaying)
            {
                Client.Demo.DumpRecording();
            }

            if (Client.Input.Keyboard.GetButton(Keys.D).WasPressed && !Client.Demo.IsPlaying)
            {
                Client.Demo.BeginPlayback();
            }
        }

        if (Client.Demo.IsPlaying)
        {
            if (Client.HumanInput.Keyboard.GetButton(Keys.Escape).WasPressed)
            {
                Client.Demo.Stop();
            }
        }
    }
}
