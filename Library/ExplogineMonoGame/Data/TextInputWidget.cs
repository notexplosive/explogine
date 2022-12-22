using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using ExplogineCore.Data;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Data;

public class TextInputWidget : Widget, IUpdateInput
{
    private readonly Alignment _alignment = Alignment.TopLeft;
    private readonly CharSequence _charSequence;
    private readonly IFontGetter _font;
    private int? _hoveredLetterIndex;
    private LeftRight _hoveredSide;
    private bool _selected;

    public TextInputWidget(Vector2 position, Point size, IFontGetter font, Depth depth, string startingText) : base(
        position, size, depth)
    {
        _font = font;

        // Content will need to be rebuilt every time these values change
        _charSequence = new CharSequence(font, startingText, InnerRectangle, _alignment);
    }

    public string Text => _charSequence.Cache.Text;

    public int CursorIndex { get; private set; }

    public RectangleF InnerRectangle => new RectangleF(Vector2.Zero, Rectangle.Size).Inflated(-5, -5);
    public int LastIndex => _charSequence.NumberOfChars;

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

            if (input.Keyboard.GetButton(Keys.Home).WasPressed)
            {
                MoveToStartOfLine();
            }

            if (input.Keyboard.GetButton(Keys.End).WasPressed)
            {
                MoveToEndOfLine();
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
                    CursorIndex = _hoveredLetterIndex.Value + offset;
                    if (CursorIndex > _charSequence.NumberOfChars)
                    {
                        CursorIndex = _charSequence.NumberOfChars;
                    }
                }
            }

            innerHitTestStack.BeforeLayerResolved += () => { _hoveredLetterIndex = null; };

            for (var i = 0; i < _charSequence.NumberOfChars; i++)
            {
                var index = i; // index will be captured so we need to set aside a variable
                var charRectangle = _charSequence.Cache.RectangleAtNode(i);
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

    public void MoveToStartOfLine()
    {
        var currentLine = _charSequence.Cache.LineNumberAt(CursorIndex);
        var newIndex = _charSequence.ScanUntil(CursorIndex, LeftRight.Left,
            currentIndex => currentLine != _charSequence.Cache.LineNumberAt(currentIndex));

        if (_charSequence.Cache.LineNumberAt(newIndex) != currentLine)
        {
            newIndex++;
        }
        
        CursorIndex = newIndex;
    }
    
    public void MoveToEndOfLine()
    {
        var currentLine = _charSequence.Cache.LineNumberAt(CursorIndex);
        var newIndex = _charSequence.ScanUntil(CursorIndex, LeftRight.Right,
            currentIndex => currentLine != _charSequence.Cache.LineNumberAt(currentIndex));

        if (newIndex != 0)
        {
            newIndex--;
        }

        CursorIndex = newIndex;
    }

    public void PrepareDraw(Painter painter)
    {
        Client.Graphics.PushCanvas(Canvas);
        painter.BeginSpriteBatch();
        var depth = Depth.Middle;

        painter.Clear(_selected ? Color.LightBlue : Color.White);

        if (_selected)
        {
            var cursorRect = _charSequence.Cache.RectangleAtNode(CursorIndex);
            var size = cursorRect.Size;
            size.X = 2f;
            cursorRect.Offset(-size.X / 2f, 0);
            var cursorRectangle = new RectangleF(cursorRect.TopLeft, size);
            painter.DrawRectangle(cursorRectangle, new DrawSettings {Depth = depth - 1, Color = Color.Black});
        }

        painter.DrawStringWithinRectangle(_font, _charSequence.Cache.Text, InnerRectangle, _alignment,
            new DrawSettings {Color = Color.Black, Depth = depth});

        if (_hoveredLetterIndex.HasValue)
        {
            var charRectangle = _charSequence.Cache.RectangleAtNode(_hoveredLetterIndex.Value);
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

        if (Client.Debug.IsActive)
        {
            var lineNumber = _charSequence.Cache.LineNumberAt(CursorIndex);
            painter.DrawStringWithinRectangle(Client.Assets.GetFont("engine/console-font", 16),
                $"{CursorIndex}, line: {lineNumber}",
                InnerRectangle, Alignment.BottomRight, new DrawSettings {Color = Color.Black});

            for (var i = 0; i < _charSequence.NumberOfNodes; i++)
            {
                var isLastChar = i == _charSequence.NumberOfNodes - 1;
                var rectangle = _charSequence.Cache.RectangleAtNode(i);
                var color = Color.Red;

                if (rectangle.Area == 0)
                {
                    rectangle.Size = new Vector2(_font.GetFont().Height) / 2f;
                    color = isLastChar ? Color.Green : Color.Blue;
                }

                if (CursorIndex == i)
                {
                    painter.DrawRectangle(rectangle,
                        new DrawSettings
                            {Color = (isLastChar ? Color.Green : Color.Yellow).WithMultipliedOpacity(0.5f)});
                }

                painter.DrawLineRectangle(rectangle, new LineDrawSettings {Color = color});
            }

            painter.DrawLineRectangle(_charSequence.Cache.UsedSpace,
                new LineDrawSettings {Depth = Depth.Back, Thickness = 2, Color = Color.Orange});
        }

        painter.EndSpriteBatch();
        Client.Graphics.PopCanvas();
    }

    public void MoveRight()
    {
        if (CursorIndex < _charSequence.NumberOfChars)
        {
            CursorIndex++;
        }
    }
    
    
    public void MoveTo(int targetIndex)
    {
        if (targetIndex >= 0 && targetIndex < _charSequence.NumberOfNodes)
        {
            CursorIndex = targetIndex;
        }
    }

    public void MoveLeft()
    {
        if (CursorIndex > 0)
        {
            CursorIndex--;
        }
    }

    public void ReverseBackspace()
    {
        _charSequence.RemoveAt(CursorIndex);
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

    public void EnterCharacter(char character)
    {
        _charSequence.Insert(CursorIndex, character);
        CursorIndex++;
    }

    public void Backspace()
    {
        if (CursorIndex > 0)
        {
            CursorIndex--;
            _charSequence.RemoveAt(CursorIndex);
        }
    }

    private delegate bool ScanFindDelegate(int index);

    private class CharSequence
    {
        private readonly List<char> _nodes = new();

        public CharSequence(IFontGetter font, string text, RectangleF containerRectangle, Alignment alignment)
        {
            foreach (var character in text)
            {
                _nodes.Add(character);
            }

            _nodes.Add('\0');
            Cache = new Cache(_nodes.ToArray(), font, containerRectangle, alignment);
        }

        public Cache Cache { get; private set; }
        public int NumberOfNodes => Cache.Text.Length + 1;
        public int NumberOfChars => Cache.Text.Length;

        public void RemoveAt(int cursorIndex)
        {
            if (cursorIndex != NumberOfNodes - 1)
            {
                _nodes.RemoveAt(cursorIndex);
                Cache = Cache.Rebuild(_nodes.ToArray());
            }
            else
            {
                Client.Debug.LogWarning("Attempted to remove null terminator");
            }
        }

        public void Insert(int cursorIndex, char character)
        {
            _nodes.Insert(cursorIndex, character);
            Cache = Cache.Rebuild(_nodes.ToArray());
        }

        public int ScanUntil(int startIndex, LeftRight direction, ScanFindDelegate found)
        {
            var step = direction == LeftRight.Left ? -1 : 1;
            var index = startIndex;

            while (true)
            {
                index += step;

                if (index < 0)
                {
                    return 0;
                }

                if (index >= NumberOfNodes)
                {
                    return NumberOfNodes;
                }
                
                if (found(index))
                {
                    break;
                }
            }

            return index;
        }
    }

    private class Cache
    {
        private readonly Alignment _alignment;
        private readonly RectangleF _containerRectangle;
        private readonly IFontGetter _font;
        private readonly char[] _nodes;
        private readonly RectangleF[] _rectangles;
        private FormattedText? _formattedTextOrNull;
        private FormattedText.FormattedGlyph[]? _glyphs;
        private string? _textOrNull;

        public Cache(char[] nodes, IFontGetter font, RectangleF containerRectangle, Alignment alignment)
        {
            _font = font;
            _containerRectangle = containerRectangle;
            _alignment = alignment;
            _nodes = nodes;
            _rectangles = new RectangleF[_nodes.Length];
            _formattedTextOrNull = null;
            _textOrNull = null;
        }

        public string Text
        {
            get
            {
                if (_textOrNull == null)
                {
                    _textOrNull = BuildString();
                }

                return _textOrNull;
            }
        }

        public RectangleF UsedSpace
        {
            get
            {
                if (!_formattedTextOrNull.HasValue)
                {
                    _formattedTextOrNull = BuildFormattedText();
                }

                var result = _rectangles[0];
                foreach (var rectangle in _rectangles)
                {
                    result = RectangleF.Union(result, rectangle);
                }

                return result;
            }
        }

        private string BuildString()
        {
            var stringBuilder = new StringBuilder();
            // note: length - 1 because we skip the null terminator
            for (var i = 0; i < _nodes.Length - 1; i++)
            {
                stringBuilder.Append(_nodes[i]);
            }

            return stringBuilder.ToString();
        }

        public int LineNumberAt(int nodeIndex)
        {
            PrepareFormattedTextCache();

            if (_glyphs == null)
            {
                throw new Exception($"{nameof(Cache._glyphs)} array was not generated");
            }

            FormattedText.FormattedGlyph glyph;
            if (nodeIndex >= _nodes.Length - 1)
            {
                glyph = _glyphs[^1];
                return glyph.LineNumber;
            }

            glyph = _glyphs[nodeIndex];

            if (glyph.Data is FormattedText.WhiteSpaceGlyphData {IsManualNewLine: true})
            {
                return glyph.LineNumber - 1;
            }

            return glyph.LineNumber;
        }

        private FormattedText BuildFormattedText()
        {
            // If the current buffer is empty, we act like we have just one character so we format it in the right spot
            var result = new FormattedText(_font, Text.Length > 0 ? Text : " ");
            var glyphRect = RectangleF.Empty;
            var currentRect = new RectangleF(Vector2.Zero, new Vector2(0, _font.GetFont().Height));
            var previousGlyphRect = currentRect;
            var glyphIndex = 0;
            var wasPreviousNewLine = false;
            _glyphs = result.GetGlyphs(_containerRectangle, _alignment).ToArray();
            foreach (var glyph in _glyphs)
            {
                glyphRect = new RectangleF(glyph.Position, glyph.Data.Size);
                var isNewLine = glyph.Data is FormattedText.WhiteSpaceGlyphData {IsManualNewLine: true};
                if (isNewLine && !wasPreviousNewLine)
                {
                    currentRect = new RectangleF(currentRect.Location + currentRect.Size.JustX(), glyphRect.Size);
                }
                else if (isNewLine && wasPreviousNewLine)
                {
                    currentRect = previousGlyphRect;
                }
                else
                {
                    currentRect = glyphRect;
                }

                _rectangles[glyphIndex++] = currentRect;
                wasPreviousNewLine = isNewLine;
                previousGlyphRect = glyphRect;
            }

            if (wasPreviousNewLine)
            {
                currentRect = glyphRect;
            }

            if (_rectangles.Length > 1)
            {
                // We're at the very end of the string, we want a zero-width rect at the end of the string 
                _rectangles[glyphIndex] = new RectangleF(new Vector2(currentRect.X + currentRect.Width, currentRect.Y),
                    new Vector2(0, _font.GetFont().Height));
            }

            return result;
        }

        public Cache Rebuild(char[] newNodes)
        {
            return new Cache(newNodes, _font, _containerRectangle, _alignment);
        }

        [Pure]
        public RectangleF RectangleAtNode(int targetIndex)
        {
            PrepareFormattedTextCache();
            return _rectangles[targetIndex];
        }

        private void PrepareFormattedTextCache()
        {
            if (!_formattedTextOrNull.HasValue)
            {
                _formattedTextOrNull = BuildFormattedText();
            }
        }
    }

    public int CurrentLine()
    {
        return _charSequence.Cache.LineNumberAt(CursorIndex);
    }
}
