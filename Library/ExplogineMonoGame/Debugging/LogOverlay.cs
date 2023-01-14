using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Gui;
using ExplogineMonoGame.Input;
using ExplogineMonoGame.Logging;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Debugging;

internal class LogOverlay : ILogCapture, IUpdateInputHook, IUpdateHook
{
    private readonly IApp _app;
    private readonly IndirectFont _font = new("engine/console-font", 32);
    private readonly LinkedList<RenderedMessage> _linesBuffer = new();
    private readonly float _maxTimer = 5;
    private float _timer;

    public LogOverlay(IApp app)
    {
        _app = app;
    }

    private float Opacity => Math.Clamp(_timer, 0f, 1f);
    private int TotalWidth => _app.Window.Size.X;
    private int MaxHeight => _app.Window.Size.Y;

    public void CaptureMessage(LogMessage message)
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

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
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

    private class RenderedMessage
    {
        public Vector2 Size { get; }

        public RenderedMessage(LogMessage content, Vector2 size, IFontGetter font)
        {
            var messageColor = LogMessage.GetColorFromType(content.Type);
            Size = size;
            FormattedText = new FormattedText(font, content.Text, messageColor);
        }
        
        public FormattedText FormattedText { get; }
    }
}
