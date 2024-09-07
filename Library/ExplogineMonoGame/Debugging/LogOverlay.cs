using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Logging;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Debugging;

internal class LogOverlay : ILogCapture, IUpdateInputHook, IUpdateHook
{
    private readonly IndirectFont _font = new("engine/console-font", 32);
    private readonly LinkedList<RenderedMessage> _linesBuffer = new();
    private readonly object _lock = new();
    private readonly float _maxTimer = 5;
    private readonly IRuntime _runtime;
    private float _timer;

    public LogOverlay(IRuntime runtime)
    {
        _runtime = runtime;
    }

    private float Opacity => Math.Clamp(_timer, 0f, 1f);
    private int TotalWidth => _runtime.Window.Size.X;
    private int MaxHeight => _runtime.Window.Size.Y;

    public void CaptureMessage(LogMessage message)
    {
        if (message.Type == LogMessageType.Verbose)
        {
            return;
        }

        lock (_lock)
        {
            var newMessage = new RenderedMessage(message, _font.MeasureString(message.Text, TotalWidth), _font);

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
    }

    public void Update(float dt)
    {
        if (_timer > 0)
        {
            _timer -= dt;

            if (_timer <= 0)
            {
                lock (_lock)
                {
                    _linesBuffer.Clear();
                }
            }
        }
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
    }

    public void Draw(Painter painter, Depth depth)
    {
        lock (_lock)
        {
            if (_linesBuffer.First != null)
            {
                painter.BeginSpriteBatch();
                var offset = (int) _linesBuffer.First.Value.Size.Y;
                var latestLogMessageRect =
                    new Rectangle(5, offset, TotalWidth - 10, MaxHeight - offset);

                foreach (var message in _linesBuffer)
                {
                    var color = Color.White;

                    painter.DrawFormattedStringWithinRectangle(
                        message.FormattedText,
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
        }
    }

    private class RenderedMessage
    {
        public RenderedMessage(LogMessage content, Vector2 size, IFontGetter font)
        {
            var messageColor = LogMessage.GetColorFromType(content.Type);
            Size = size;
            FormattedText = new FormattedText(font, content.Text, messageColor);
        }

        public Vector2 Size { get; }

        public FormattedText FormattedText { get; }
    }
}
