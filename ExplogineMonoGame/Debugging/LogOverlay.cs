using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Logging;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Debugging;

internal class LogOverlay : ILogCapture
{
    private readonly LinkedList<string> _linesBuffer = new();
    private float _timer;

    private float Opacity => Math.Clamp(_timer, 0f, 1f);

    public void CaptureMessage(string message)
    {
        _linesBuffer.AddLast(message);
        _timer = 5;
    }

    public void Update(float dt)
    {
        if (_timer > 0)
        {
            _timer -= dt;

            if (_timer <= 0)
            {
                _linesBuffer.Clear();
            }
        }
    }

    public void Draw(Painter painter, Depth depth)
    {
        var font = Client.Assets.GetFont("engine/console-font", 32);
        var maxWidth = Client.Graphics.WindowSize.X;
        var textRect = new Rectangle(5, 0, maxWidth - 10, Client.Graphics.WindowSize.Y);

        foreach (var message in _linesBuffer)
        {
            painter.DrawStringWithinRectangle(font, message, textRect,
                new DrawSettings {Color = Color.White.WithMultipliedOpacity(Opacity), Depth = depth});

            textRect.Location += new Point(0, (int) font.MeasureString(message, maxWidth).Y);
        }

        painter.DrawAsRectangle(Client.Assets.GetTexture("white-pixel"),
            new Rectangle(0, 0, Client.Graphics.WindowSize.X, textRect.Location.Y),
            new DrawSettings {Color = Color.Black.WithMultipliedOpacity(0.5f * Opacity), Depth = 100});
    }
}
