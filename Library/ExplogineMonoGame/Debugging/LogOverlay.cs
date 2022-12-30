using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Gui;
using ExplogineMonoGame.Input;
using ExplogineMonoGame.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Debugging;

internal class LogOverlay : ILogCapture, IUpdateInput
{
    private readonly IndirectFont _font = new("engine/console-font", 32);
    private readonly LinkedList<RenderedMessage> _linesBuffer = new();
    private readonly float _maxTimer = 5;
    private readonly SimpleGuiTheme _theme;
    private float _timer;

    public LogOverlay()
    {
        _theme = new SimpleGuiTheme(Color.White, Color.White, Color.Transparent, _font);
    }

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
        _timer = _maxTimer;
    }

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
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
        painter.BeginSpriteBatch();
        var latestLogMessageRect = new Rectangle(5, 0, TotalWidth - 10, MaxHeight);

        foreach (var message in _linesBuffer)
        {
            var color = Color.White;
            var messageColor = LogMessage.GetColorFromType(message.Content.Type).WithMultipliedOpacity(Opacity);

            painter.DrawFormattedStringWithinRectangle(
                new FormattedText(_font, message.Content.Text, messageColor),
                latestLogMessageRect,
                Alignment.TopLeft,
                new DrawSettings {Color = color.WithMultipliedOpacity(Opacity), Depth = depth});

            latestLogMessageRect.Location += new Point(0, (int) message.Size.Y);
        }

        var overlayHeight = (float) latestLogMessageRect.Location.Y;

        painter.DrawRectangle(new RectangleF(0, 0, TotalWidth, overlayHeight),
            new DrawSettings {Color = Color.Black.WithMultipliedOpacity(0.5f * Opacity), Depth = 100});
        painter.EndSpriteBatch();
    }

    private readonly record struct RenderedMessage(LogMessage Content, Vector2 Size);
}
