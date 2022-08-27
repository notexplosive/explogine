using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Logging;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Debugging;

internal class LogOverlay : ILogCapture
{
    private readonly LinkedList<RenderedMessage> _linesBuffer = new();
    private float _timer;
    private Font Font => Client.Assets.GetFont("engine/console-font", 32);

    private float Opacity => Math.Clamp(_timer, 0f, 1f);

    private int TotalWidth => Client.Window.Size.X;
    private int MaxHeight => Client.Window.Size.Y;

    public void CaptureMessage(string text)
    {
        var newMessage = new RenderedMessage(text, Font.MeasureString(text, TotalWidth));

        float usedHeight = 0;
        foreach (var line in _linesBuffer)
        {
            usedHeight += line.Size.Y;
        }

        void RemoveEntriesUntilFit(float pendingTotalHeight)
        {
            if (pendingTotalHeight > MaxHeight)
            {
                var first = _linesBuffer.First;

                if (first != _linesBuffer.Last && first != null)
                {
                    var firstValue = first.Value;
                    var removedHeight = firstValue.Size.Y;
                    _linesBuffer.RemoveFirst();
                    pendingTotalHeight -= removedHeight;
                    RemoveEntriesUntilFit(pendingTotalHeight);
                }
            }
        }

        RemoveEntriesUntilFit(usedHeight + newMessage.Size.Y);

        _linesBuffer.AddLast(newMessage);
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
        var textRect = new Rectangle(5, 0, TotalWidth - 10, MaxHeight);

        foreach (var message in _linesBuffer)
        {
            var color = Color.White;

            painter.DrawStringWithinRectangle(Font, message.Text, textRect,
                new DrawSettings {Color = color.WithMultipliedOpacity(Opacity), Depth = depth});

            textRect.Location += new Point(0, (int) message.Size.Y);
        }

        painter.DrawAsRectangle(Client.Assets.GetTexture("white-pixel"),
            new Rectangle(0, 0, TotalWidth, textRect.Location.Y),
            new DrawSettings {Color = Color.Black.WithMultipliedOpacity(0.5f * Opacity), Depth = 100});
    }

    private readonly record struct RenderedMessage(string Text, Vector2 Size);
}
