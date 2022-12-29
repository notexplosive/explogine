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
    private TextInputWidget _textInputWidget = null!;
    private readonly SimpleGuiTheme _theme;
    private bool _isTyping;
    private float _timer;

    public LogOverlay()
    {
        Client.FinishedLoading.Add(() =>
        {
            _textInputWidget = new TextInputWidget(Vector2.Zero, new Point(200, _font.FontSize + 1), _font,
                new TextInputWidget.Settings
                {
                    Depth = 50,
                    Selector = new AlwaysSelected(),
                    IsSingleLine = true,
                    ShowScrollbar = false,
                }
            );
        });

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
        if (input.Keyboard.GetButton(Keys.OemTilde).WasPressed && input.Keyboard.Modifiers.None)
        {
            _isTyping = !_isTyping;
        }

        if (_isTyping)
        {
            _textInputWidget.UpdateInput(input, hitTestStack);
        }
    }

    public void Update(float dt)
    {
        if (_isTyping)
        {
            _timer = _maxTimer;
        }

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
        PrepareDraw(painter);

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

        if (_isTyping)
        {
            // +5 so text doesn't get clipped off the bottom
            var typeAreaHeight = (int) _font.GetFont().Height + 5;
            _textInputWidget.Position = new Vector2(latestLogMessageRect.X, overlayHeight);
            _textInputWidget.Size = new Point(latestLogMessageRect.Width, typeAreaHeight);
            overlayHeight += typeAreaHeight;

            _textInputWidget.Draw(painter);
        }

        painter.DrawRectangle(new RectangleF(0, 0, TotalWidth, overlayHeight),
            new DrawSettings {Color = Color.DarkBlue.WithMultipliedOpacity(0.5f * Opacity), Depth = 100});
        painter.EndSpriteBatch();
    }

    public void PrepareDraw(Painter painter)
    {
        if (_isTyping)
        {
            _textInputWidget.PrepareDraw(painter, _theme);
        }
    }

    private readonly record struct RenderedMessage(LogMessage Content, Vector2 Size);
}
