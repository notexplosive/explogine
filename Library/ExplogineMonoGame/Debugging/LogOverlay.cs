using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Logging;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Debugging;

internal class LogOverlay : ILogCapture
{
    private readonly LazyInitializedFont _font = new("engine/console-font", 32);
    private readonly LinkedList<RenderedMessage> _linesBuffer = new();
    private float _timer;

    private float Opacity => Math.Clamp(_timer, 0f, 1f);

    private int TotalWidth => Client.Window.Size.X;
    private int MaxHeight => Client.Window.Size.Y;

    public void CaptureMessage(LogMessage message)
    {
        var newMessage = new RenderedMessage(message, _font.MeasureString(message.Text, TotalWidth));

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
            var messageColor = LogMessage.GetColorFromType(message.Content.Type).WithMultipliedOpacity(Opacity);

            painter.DrawFormattedStringWithinRectangle(
                new FormattedText(_font, message.Content.Text, messageColor),
                textRect,
                Alignment.TopLeft,
                new DrawSettings {Color = color.WithMultipliedOpacity(Opacity), Depth = depth});

            textRect.Location += new Point(0, (int) message.Size.Y);
        }

        painter.DrawAsRectangle(Client.Assets.GetTexture("white-pixel"),
            new Rectangle(0, 0, TotalWidth, textRect.Location.Y),
            new DrawSettings {Color = Color.Black.WithMultipliedOpacity(0.5f * Opacity), Depth = 100});
    }

    private readonly record struct RenderedMessage(LogMessage Content, Vector2 Size);
}
