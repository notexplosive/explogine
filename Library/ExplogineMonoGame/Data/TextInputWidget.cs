using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using ExplogineCore.Data;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Data;

public class TextInputWidget : Widget, IUpdateInput
{
    private readonly Alignment _alignment = Alignment.TopLeft;
    private readonly IFontGetter _font;
    private int _cursorIndex;
    private int? _hoveredLetterIndex;
    private LeftRight _hoveredSide;
    private bool _selected;
    private readonly CharSequence _charSequence;

    public TextInputWidget(Vector2 position, Point size, IFontGetter font, Depth depth, string startingText) : base(position, size, depth)
    {
        _font = font;
        
        // Content will need to be rebuilt every time these values change
        _charSequence = new CharSequence(font, startingText, InnerRectangle, _alignment);
    }

    public RectangleF InnerRectangle => new RectangleF(Vector2.Zero, Rectangle.Size).Inflated(-5, -5);

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        UpdateHovered(hitTestStack);

        if (_selected)
        {
            EnterText(input.Keyboard.GetEnteredCharacters());

            if (input.Keyboard.GetButton(Keys.Left).WasPressed)
            {
                MoveLeft();
            }

            if (input.Keyboard.GetButton(Keys.Right).WasPressed)
            {
                MoveRight();
            }
        }

        var innerHitTestStack = hitTestStack.AddLayer(ScreenToCanvas);
        if (IsHovered)
        {
            if (input.Mouse.GetButton(MouseButton.Left).WasPressed)
            {
                _selected = true;
                if (_hoveredLetterIndex.HasValue)
                {
                    var offset = _hoveredSide == LeftRight.Right ? 1 : 0;
                    _cursorIndex = _hoveredLetterIndex.Value + offset;
                    if (_cursorIndex > _charSequence.NumberOfChars)
                    {
                        _cursorIndex = _charSequence.NumberOfChars;
                    }
                }
            }

            innerHitTestStack.BeforeLayerResolved += () => { _hoveredLetterIndex = null; };

            for (var i = 0; i < _charSequence.NumberOfChars; i++)
            {
                var index = i; // index will be captured so we need to set aside a variable
                var charRectangle = _charSequence.RectangleAtCharIndex(i);
                var halfWidth = charRectangle.Size.X / 2;
                var leftRectangle = new RectangleF(charRectangle.Location,
                    new Vector2(halfWidth, charRectangle.Size.Y));

                var rightRectangle = new RectangleF(charRectangle.Location,
                        new Vector2(halfWidth, charRectangle.Size.Y))
                    .Moved(new Vector2(halfWidth, 0));

                innerHitTestStack.AddZone(leftRectangle, Depth.Middle,
                    () =>
                    {
                        _hoveredLetterIndex = index;
                        _hoveredSide = LeftRight.Left;
                        Client.Window.SetCursor(MouseCursor.IBeam);
                    });

                innerHitTestStack.AddZone(rightRectangle, Depth.Middle,
                    () =>
                    {
                        _hoveredLetterIndex = index;
                        _hoveredSide = LeftRight.Right;
                        Client.Window.SetCursor(MouseCursor.IBeam);
                    });
            }
        }
    }

    public void PrepareDraw(Painter painter)
    {
        Client.Graphics.PushCanvas(Canvas);
        painter.BeginSpriteBatch();
        var depth = Depth.Middle;

        painter.Clear(_selected ? Color.LightBlue : Color.White);

        if (_selected)
        {
            var cursorRect = _charSequence.RectangleAtCharIndex(_cursorIndex);
            var size = cursorRect.Size;
            size.X = 2f;
            cursorRect.Offset(-size.X / 2f, 0);
            var cursorRectangle = new RectangleF(cursorRect.TopLeft, size);
            painter.DrawRectangle(cursorRectangle, new DrawSettings {Depth = depth - 1, Color = Color.Black});
        }

        painter.DrawStringWithinRectangle(_font, _charSequence.ContentString(), InnerRectangle, _alignment,
            new DrawSettings {Color = Color.Black, Depth = depth});

        if (_hoveredLetterIndex.HasValue)
        {
            var charRectangle = _charSequence.RectangleAtCharIndex(_hoveredLetterIndex.Value);
            var halfWidth = charRectangle.Size.X / 2;
            var leftRectangle = new RectangleF(charRectangle.Location,
                new Vector2(halfWidth, charRectangle.Size.Y));

            var rightRectangle = new RectangleF(charRectangle.Location,
                    new Vector2(halfWidth, charRectangle.Size.Y))
                .Moved(new Vector2(halfWidth, 0));

            if (Client.Debug.IsActive)
            {
                painter.DrawRectangle(_hoveredSide == LeftRight.Left ? leftRectangle : rightRectangle,
                    new DrawSettings
                    {
                        Depth = depth + 1,
                        Color = _hoveredSide == LeftRight.Left
                            ? Color.Red.WithMultipliedOpacity(0.2f)
                            : Color.Blue.WithMultipliedOpacity(0.2f)
                    });
            }
        }

        painter.EndSpriteBatch();
        Client.Graphics.PopCanvas();
    }

    private void MoveRight()
    {
        if (_cursorIndex < _charSequence.NumberOfChars)
        {
            _cursorIndex++;
        }
    }

    private void MoveLeft()
    {
        if (_cursorIndex > 0)
        {
            _cursorIndex--;
        }
    }

    private void ReverseBackspace()
    {
        _charSequence.RemoveAt(_cursorIndex);
    }

    private void EnterText(char[] enteredCharacters)
    {
        foreach (var character in enteredCharacters)
        {
            if (!char.IsControl(character))
            {
                EnterCharacter(character);
            }
            else if (character == '\b')
            {
                Backspace();
            }
            else if (character == '\r')
            {
                EnterCharacter('\n');
            }
            else if (character == 127)
            {
                ReverseBackspace();
            }
            else
            {
                if (char.IsControl(character))
                {
                    Client.Debug.LogWarning($"Unsafe control character, ignored {(int) character}");
                }
            }
        }
    }

    private void EnterCharacter(char character)
    {
        _charSequence.Insert(_cursorIndex, character);
        _cursorIndex++;
    }

    private void Backspace()
    {
        if (_cursorIndex > 0)
        {
            _cursorIndex--;
            _charSequence.RemoveAt(_cursorIndex);
        }
    }

    private class CharSequence
    {
        private readonly IFontGetter _font;
        private readonly RectangleF _innerRectangle;
        private readonly Alignment _alignment;
        private readonly List<char> _content = new();

        public CharSequence(IFontGetter font, string text, RectangleF innerRectangle, Alignment alignment)
        {
            _font = font;
            _innerRectangle = innerRectangle;
            _alignment = alignment;
            foreach (var character in text)
            {
                _content.Add(character);
            }
            _content.Add(char.MinValue);
        }

        public int NumberOfNodes => _content.Count;
        public int NumberOfChars => _content.Count - 1;

        [Pure]
        public string ContentString()
        {
            // todo: cache this!!
            var stringBuilder = new StringBuilder();
            foreach (var item in _content)
            {
                if (item != '\0')
                {
                    stringBuilder.Append(item);
                }
            }

            return stringBuilder.ToString();
        }

        [Pure]
        public char CharacterAt(int index)
        {
            return ContentString()[index];
        }

        [Pure]
        public RectangleF RectangleAtCharIndex(int targetIndex)
        {
            // todo: cache this!!
            var contentString = ContentString();
            // If the current buffer is empty, we act like we have just one character so we format it in the right spot
            var formattedText = new FormattedText(_font, contentString.Length > 0 ? contentString : " ");

            var currentRect = new RectangleF(Vector2.Zero, new Vector2(0, _font.GetFont().Height));

            var glyphIndex = 0;
            foreach (var glyph in formattedText.GetGlyphs(_innerRectangle, _alignment))
            {
                currentRect = new RectangleF(glyph.Position, glyph.Data.Size);
                if (glyphIndex == targetIndex)
                {
                    return currentRect;
                }

                glyphIndex++;
            }

            if (glyphIndex == targetIndex)
            {
                // We're at the very end of the string, we want a zero-width rect at the end of the string 
                return new RectangleF(new Vector2(currentRect.X + currentRect.Width, currentRect.Y),
                    new Vector2(0, _font.GetFont().Height));
            }

            throw new IndexOutOfRangeException();
        }

        public void RemoveAt(int cursorIndex)
        {
            if (cursorIndex != NumberOfNodes - 1)
            {
                _content.RemoveAt(cursorIndex);
            }
            else
            {
                Client.Debug.LogWarning("Attempted to remove null terminator");
            }
        }

        public void Insert(int cursorIndex, char character)
        {
            _content.Insert(cursorIndex, character);
        }
    }
}
