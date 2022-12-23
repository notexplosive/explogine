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
    private HorizontalDirection _hoveredSide;
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
    public int CurrentColumn => _charSequence.GetColumn(CursorIndex);

    public int CurrentLine => _charSequence.Cache.LineNumberAt(CursorIndex);

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        UpdateHovered(hitTestStack);

        if (_selected)
        {
            EnterText(input.Keyboard.GetEnteredCharacters());

            if (input.Keyboard.GetButton(Keys.Left).WasPressed)
            {
                if (input.Keyboard.Modifiers.ControlInclusive)
                {
                    MoveWordLeft();
                }
                else
                {
                    MoveLeft();
                }
            }

            if (input.Keyboard.GetButton(Keys.Right).WasPressed)
            {
                if (input.Keyboard.Modifiers.ControlInclusive)
                {
                    MoveWordRight();
                }
                else
                {
                    MoveRight();
                }
            }

            if (input.Keyboard.GetButton(Keys.Up).WasPressed)
            {
                MoveUp();
            }

            if (input.Keyboard.GetButton(Keys.Down).WasPressed)
            {
                MoveDown();
            }

            if (input.Keyboard.GetButton(Keys.Home).WasPressed)
            {
                MoveToStartOfLine();
            }

            if (input.Keyboard.GetButton(Keys.End).WasPressed)
            {
                MoveToEndOfLine();
            }

            if (input.Keyboard.GetButton(Keys.Back).WasPressed)
            {
                if (input.Keyboard.Modifiers.ControlInclusive)
                {
                    BackspaceWholeWord();
                }
                else
                {
                    Backspace();
                }
            }
            
            if (input.Keyboard.GetButton(Keys.Delete).WasPressed)
            {
                if (input.Keyboard.Modifiers.ControlInclusive)
                {
                    ReverseBackspaceWholeWord();
                }
                else
                {
                    ReverseBackspace();
                }
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
                    var offset = _hoveredSide == HorizontalDirection.Right ? 1 : 0;
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
                        _hoveredSide = HorizontalDirection.Left;
                        Client.Window.SetCursor(MouseCursor.IBeam);
                    });

                innerHitTestStack.AddZone(rightRectangle, Depth.Middle,
                    () =>
                    {
                        _hoveredLetterIndex = index;
                        _hoveredSide = HorizontalDirection.Right;
                        Client.Window.SetCursor(MouseCursor.IBeam);
                    });
            }
        }
    }

    private void BackspaceWholeWord()
    {
        var index = CursorIndex;
        if (_charSequence.IsValidIndex(index - 1) && IsWordBoundary(index - 1))
        {
            index = _charSequence.ScanUntil(index, HorizontalDirection.Left, IsNotWordBoundary);
        }
        else
        {
            index = _charSequence.ScanUntil(CursorIndex, HorizontalDirection.Left, IsWordBoundary);
        }

        if (_charSequence.IsValidIndex(index + 1) && index != 0)
        {
            index++;
        }
        
        var distance = CursorIndex - index;
        for (int i = 0; i < distance; i++)
        {
            Backspace();
        }
    }

    private void ReverseBackspaceWholeWord()
    {
        var index = CursorIndex;
        if (IsWordBoundary(index))
        {
            index = _charSequence.ScanUntil(index, HorizontalDirection.Right, IsNotWordBoundary);
        }
        else
        {
            index = _charSequence.ScanUntil(index, HorizontalDirection.Right, IsWordBoundary);
        }

        var distance = index - CursorIndex;
        for (int i = 0; i < distance; i++)
        {
            ReverseBackspace();
        }
    }

    public int GetWordBoundaryLeftOf(int index)
    {
        if (_charSequence.IsValidIndex(index - 1) && IsWordBoundary(index - 1))
        {
            index = _charSequence.ScanUntil(index, HorizontalDirection.Left, IsNotWordBoundary);
        }

        index = _charSequence.ScanUntil(index, HorizontalDirection.Left, IsWordBoundary);

        if (_charSequence.IsValidIndex(index + 1) && index != 0)
        {
            index++;
        }

        return index;
    }

    public void MoveWordLeft()
    {
        CursorIndex = GetWordBoundaryLeftOf(CursorIndex);
    }

    public int GetWordBoundaryRightOf(int index)
    {
        if (IsWordBoundary(index))
        {
            index = _charSequence.ScanUntil(index, HorizontalDirection.Right, IsNotWordBoundary);
        }

        index = _charSequence.ScanUntil(index, HorizontalDirection.Right, IsWordBoundary);

        return index;
    }

    public void MoveWordRight()
    {
        CursorIndex = GetWordBoundaryRightOf(CursorIndex);
    }

    private bool IsNotWordBoundary(int nodeIndex)
    {
        return !IsWordBoundary(nodeIndex);
    }

    private bool IsWordBoundary(int nodeIndex)
    {
        return nodeIndex == LastIndex || char.IsWhiteSpace(_charSequence.Cache.Text[nodeIndex]);
    }

    public void MoveToStartOfLine()
    {
        CursorIndex = _charSequence.GetNodesOnLine(CurrentLine)[0];
    }

    public void MoveToEndOfLine()
    {
        CursorIndex = _charSequence.GetNodesOnLine(CurrentLine)[^1];
    }

    public void MoveUp()
    {
        var targetLineIndices = _charSequence.GetNodesOnLine(_charSequence.Cache.LineNumberAt(CursorIndex) - 1);
        var currentColumn = _charSequence.GetColumn(CursorIndex);

        if (targetLineIndices.Length == 0)
        {
            return;
        }

        if (targetLineIndices.Length <= currentColumn)
        {
            CursorIndex = targetLineIndices[^1];
        }
        else
        {
            CursorIndex = targetLineIndices[currentColumn];
        }
    }

    public void MoveDown()
    {
        var targetLineIndices = _charSequence.GetNodesOnLine(_charSequence.Cache.LineNumberAt(CursorIndex) + 1);
        var currentColumn = _charSequence.GetColumn(CursorIndex);

        if (targetLineIndices.Length == 0)
        {
            return;
        }

        if (targetLineIndices.Length <= currentColumn)
        {
            CursorIndex = targetLineIndices[^1];
        }
        else
        {
            CursorIndex = targetLineIndices[currentColumn];
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
                painter.DrawRectangle(_hoveredSide == HorizontalDirection.Left ? leftRectangle : rightRectangle,
                    new DrawSettings
                    {
                        Depth = depth + 1,
                        Color = _hoveredSide == HorizontalDirection.Left
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
            switch (char.IsControl(character))
            {
                case false:
                    EnterCharacter(character);
                    break;
                default:
                {
                    if (character == '\b')
                    {
                    }
                    else if (character == '\r')
                    {
                        EnterCharacter('\n');
                    }
                    else
                    {
                        if (char.IsControl(character) && character != 127)
                        {
                            Client.Debug.LogWarning($"Unsafe control character, ignored {(int) character}");
                        }
                    }

                    break;
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

    public int LineLength(int line)
    {
        return _charSequence.CountNodesOnLine(line);
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
        }

        public void Insert(int cursorIndex, char character)
        {
            _nodes.Insert(cursorIndex, character);
            Cache = Cache.Rebuild(_nodes.ToArray());
        }

        public int ScanUntil(int startIndex, HorizontalDirection direction, ScanFindDelegate found)
        {
            var step = direction == HorizontalDirection.Left ? -1 : 1;
            var index = startIndex;

            while (true)
            {
                index += step;

                if (index < 0)
                {
                    return 0;
                }

                if (index >= NumberOfChars)
                {
                    return NumberOfChars;
                }

                if (found(index))
                {
                    break;
                }
            }

            return index;
        }

        public int CountNodesOnLine(int currentLine)
        {
            var count = 0;
            for (var i = 0; i < NumberOfNodes; i++)
            {
                if (Cache.LineNumberAt(i) == currentLine)
                {
                    count++;
                }
            }

            return count;
        }

        public int[] GetNodesOnLine(int targetLine)
        {
            var result = new int[CountNodesOnLine(targetLine)];
            var j = 0;
            for (var i = 0; i < NumberOfNodes; i++)
            {
                if (Cache.LineNumberAt(i) == targetLine)
                {
                    result[j++] = i;
                }
            }

            return result.ToArray();
        }

        public int GetColumn(int nodeIndex)
        {
            var currentLine = Cache.LineNumberAt(nodeIndex);

            var col = 0;
            for (var i = 0; i < NumberOfNodes; i++)
            {
                if (Cache.LineNumberAt(i) == currentLine)
                {
                    if (i == nodeIndex)
                    {
                        return col;
                    }

                    col++;
                }
            }

            throw new Exception("could not find column");
        }

        public bool IsValidIndex(int nodeIndex)
        {
            return nodeIndex >= 0 && nodeIndex < NumberOfNodes;
        }
    }

    private class Cache
    {
        private readonly Alignment _alignment;
        private readonly RectangleF _containerRectangle;
        private readonly IFontGetter _font;
        private readonly CacheNode[] _nodes;
        private readonly char[] _originalChars;

        public Cache(char[] chars, IFontGetter font, RectangleF containerRectangle, Alignment alignment)
        {
            _originalChars = chars;
            _font = font;
            _containerRectangle = containerRectangle;
            _alignment = alignment;
            _nodes = new CacheNode[chars.Length + 1];
            Text = BuildString(chars);
            BuildFormattedText();
        }

        public string Text { get; }

        public RectangleF UsedSpace
        {
            get
            {
                var result = _nodes[0].Rectangle;
                foreach (var node in _nodes)
                {
                    result = RectangleF.Union(result, node.Rectangle);
                }

                return result;
            }
        }

        private string BuildString(char[] chars)
        {
            var stringBuilder = new StringBuilder();
            // We do length - 1 because we want to skip the null terminator
            foreach (var character in chars)
            {
                stringBuilder.Append(character);
            }

            return stringBuilder.ToString();
        }

        public int LineNumberAt(int nodeIndex)
        {
            return _nodes[nodeIndex].LineNumber;
        }

        private void BuildFormattedText()
        {
            // If the current buffer is empty, we act like we have just one character so we format it in the right spot
            var formattedText = new FormattedText(_font, Text.Length > 0 ? Text : " ");
            var glyphRect = RectangleF.Empty;
            var currentRect = new RectangleF(Vector2.Zero, new Vector2(0, _font.GetFont().Height));
            var previousGlyphRect = currentRect;
            var nodeIndex = 0;
            var wasPreviousNewLine = false;
            var glyphs = formattedText.GetGlyphs(_containerRectangle, _alignment).ToArray();
            var lineNumber = 0;

            foreach (var glyph in glyphs)
            {
                lineNumber = glyph.LineNumber;
                if (glyph.Data is FormattedText.WhiteSpaceGlyphData {IsManualNewLine: true})
                {
                    lineNumber--;
                }

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

                var character = '\0';
                if (nodeIndex < _originalChars.Length)
                {
                    character = _originalChars[nodeIndex];
                }

                _nodes[nodeIndex] = new CacheNode(currentRect, lineNumber, character, glyph);

                nodeIndex++;
                wasPreviousNewLine = isNewLine;
                previousGlyphRect = glyphRect;
            }

            if (wasPreviousNewLine)
            {
                currentRect = glyphRect;
            }

            if (_nodes.Length > 1)
            {
                var rectangle = new RectangleF(new Vector2(currentRect.X + currentRect.Width, currentRect.Y),
                    new Vector2(0, _font.GetFont().Height));

                // We want a zero-width rect at the end of the string 
                _nodes[nodeIndex] = new CacheNode(rectangle, lineNumber, '\0', new FormattedText.FormattedGlyph());
            }
        }

        public Cache Rebuild(char[] newNodes)
        {
            return new Cache(newNodes, _font, _containerRectangle, _alignment);
        }

        [Pure]
        public RectangleF RectangleAtNode(int nodeIndex)
        {
            return _nodes[nodeIndex].Rectangle;
        }

        private readonly record struct CacheNode(RectangleF Rectangle, int LineNumber, char Char,
            FormattedText.FormattedGlyph OriginalGlyph);
    }
}
