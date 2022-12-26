using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using ExplogineCore.Data;
using ExplogineMonoGame.Gui;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Data;

public class TextInputWidget : Widget, IUpdateInput
{
    private readonly Alignment _alignment = Alignment.TopLeft;
    private readonly CharSequence _charSequence;
    private readonly ClickCounter _clickCounter = new();

    private readonly TextCursor _cursor = new();
    private readonly IFontGetter _font;
    private readonly ScrollableArea _scrollableArea;
    private int? _hoveredLetterIndex;
    private HorizontalDirection _hoveredSide;
    private bool _isDragging;
    private RepeatedAction? _mostRecentAction;
    private bool _selected;
    private readonly HoverState _isTextAreaHovered;

    public TextInputWidget(Vector2 position, Point size, IFontGetter font, Depth depth, string startingText)
        : base(position, size, depth)
    {
        _font = font;
        _scrollableArea = new ScrollableArea(Size, InnerRectangle, Depth.Front);
        _scrollableArea.EnableInput = new XyBool(false, true);

        _isTextAreaHovered = new HoverState();

        _cursor.MovedCursor += OnCursorMoved;

        // Content will need to be rebuilt every time these values change
        _charSequence = new CharSequence(font, startingText, TextAreaRectangle, _alignment);
        _charSequence.CacheUpdated += () => OnCursorMoved(CursorIndex);
    }

    public string Text => _charSequence.Cache.Text;
    public int CursorIndex => _cursor.Index;
    public RectangleF TextAreaRectangle => InnerRectangle.Inflated(-5, -5).ResizedOnEdge(RectEdge.Right, new Vector2(-_scrollableArea.ScrollBarWidth, 0));
    public RectangleF InnerRectangle => new(Vector2.Zero, Rectangle.Size);
    public int LastIndex => _charSequence.NumberOfChars;
    public int CurrentColumn => _charSequence.GetColumn(CursorIndex);
    public int CurrentLine => _charSequence.Cache.LineNumberAt(CursorIndex);

    private int? HoveredNodeIndex
    {
        get
        {
            if (!_hoveredLetterIndex.HasValue)
            {
                return null;
            }

            var offset = _hoveredSide == HorizontalDirection.Right ? 1 : 0;
            var result = _hoveredLetterIndex.Value + offset;

            if (result > _charSequence.NumberOfChars)
            {
                result = _charSequence.NumberOfChars;
            }

            return result;
        }
    }

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        UpdateHovered(hitTestStack);


        if (_selected)
        {
            var keyboard = input.Keyboard;

            EnterText(keyboard.GetEnteredCharacters());

            bool ControlIsDown(ModifierKeys modifierKeys)
            {
                return modifierKeys.ControlInclusive;
            }

            bool ControlIsNotDown(ModifierKeys modifierKeys)
            {
                return !modifierKeys.ControlInclusive;
            }

            bool ModifierAgnostic(ModifierKeys modifierKeys)
            {
                return true;
            }

            bool SelectionAgnostic()
            {
                return true;
            }

            bool HasSelection()
            {
                return _cursor.HasSelection;
            }

            bool DoesNotHaveSelection()
            {
                return !_cursor.HasSelection;
            }

            KeyBind(keyboard, Keys.Left, SelectionAgnostic, ControlIsDown, MoveWordLeft);
            KeyBind(keyboard, Keys.Left, SelectionAgnostic, ControlIsNotDown, MoveLeft);
            KeyBind(keyboard, Keys.Right, SelectionAgnostic, ControlIsDown, MoveWordRight);
            KeyBind(keyboard, Keys.Right, SelectionAgnostic, ControlIsNotDown, MoveRight);
            KeyBind(keyboard, Keys.Up, SelectionAgnostic, ModifierAgnostic, MoveUp);
            KeyBind(keyboard, Keys.Down, SelectionAgnostic, ModifierAgnostic, MoveDown);
            KeyBind(keyboard, Keys.Home, SelectionAgnostic, ModifierAgnostic, MoveToStartOfLine);
            KeyBind(keyboard, Keys.End, SelectionAgnostic, ModifierAgnostic, MoveToEndOfLine);
            KeyBind(keyboard, Keys.Back, DoesNotHaveSelection, ControlIsDown, BackspaceWholeWord);
            KeyBind(keyboard, Keys.Back, DoesNotHaveSelection, ControlIsNotDown, Backspace);
            KeyBind(keyboard, Keys.Back, HasSelection, ModifierAgnostic, ClearSelectedRange);
            KeyBind(keyboard, Keys.Delete, DoesNotHaveSelection, ControlIsDown, ReverseBackspaceWholeWord);
            KeyBind(keyboard, Keys.Delete, DoesNotHaveSelection, ControlIsNotDown, ReverseBackspace);
            KeyBind(keyboard, Keys.Delete, HasSelection, ModifierAgnostic, ClearSelectedRange);
            KeyBind(keyboard, Keys.A, SelectionAgnostic, ControlIsDown, SelectEverything);
        }

        var unscrolledHitTestStack = hitTestStack.AddLayer(ScreenToCanvas);
        var scrollingHitTestStack = hitTestStack.AddLayer(_scrollableArea.ScreenToCanvas * ScreenToCanvas);

        _scrollableArea.UpdateInput(input, unscrolledHitTestStack);

        scrollingHitTestStack.BeforeLayerResolved += () =>
        {
            _hoveredLetterIndex = null;
        };

        unscrolledHitTestStack.AddZone(TextAreaRectangle, Depth.Middle, _isTextAreaHovered, true);

        var scrollDelta = input.Mouse.ScrollDelta();
        _scrollableArea.Move(new Vector2(0, -scrollDelta / 5f));

        scrollingHitTestStack.AddInfiniteZone(Depth.Back, () =>
        {
            if (_selected)
            {
                var lineRectangles = _charSequence.Cache.LineRectangles();
                var targetIndex = 0;
                var mousePosition = input.Mouse.Position(scrollingHitTestStack.WorldMatrix);

                for (var lineNumber = 0; lineNumber < lineRectangles.Length; lineNumber++)
                {
                    var isFirstLine = lineNumber == 0;
                    var isLastLine = lineNumber == lineRectangles.Length - 1;

                    var lineRectangle = lineRectangles[lineNumber];

                    if (lineRectangle.Contains(mousePosition))
                    {
                        // We hit inside a line, let the HitTestZones figure it out
                        return;
                    }

                    var isBelowTop = mousePosition.Y >= lineRectangle.Top;
                    var isAboveBottom = mousePosition.Y < lineRectangle.Bottom;
                    var isWithin = isAboveBottom && isBelowTop;
                    var forceToStart = false;
                    var forceToEnd = false;

                    if (!isWithin)
                    {
                        if (isFirstLine)
                        {
                            isWithin = mousePosition.Y < lineRectangle.Top;
                            forceToStart = isWithin;
                        }

                        if (isLastLine)
                        {
                            isWithin = mousePosition.Y >= lineRectangle.Bottom;
                            forceToEnd = isWithin;
                        }
                    }

                    if (isWithin)
                    {
                        var nodesOnLine = _charSequence.GetNodesOnLine(lineNumber);
                        if (mousePosition.X > lineRectangle.Right || forceToEnd)
                        {
                            targetIndex = nodesOnLine[^1];
                        }

                        if (mousePosition.X < lineRectangle.Left || forceToStart)
                        {
                            targetIndex = nodesOnLine[0];
                        }
                    }
                }

                _hoveredLetterIndex = targetIndex;
                _hoveredSide = HorizontalDirection.Left;
            }
        });

        if (input.Mouse.GetButton(MouseButton.Left).WasReleased)
        {
            _isDragging = false;
        }

        if (_isTextAreaHovered || _isDragging)
        {
            if (_hoveredLetterIndex != null)
            {
                Client.Window.SetCursor(MouseCursor.IBeam);
            }

            var leaveAnchor = input.Keyboard.Modifiers.ShiftInclusive;

            if (input.Mouse.GetButton(MouseButton.Left).WasPressed)
            {
                _isDragging = true;
                _selected = true;

                if (HoveredNodeIndex.HasValue)
                {
                    _cursor.SetIndex(HoveredNodeIndex.Value, leaveAnchor);

                    _clickCounter.Increment(input.Mouse.Position());
                    if (_clickCounter.NumberOfClicks > 1)
                    {
                        if (_clickCounter.NumberOfClicks == 2)
                        {
                            SelectWordFromIndex(HoveredNodeIndex.Value);
                        }
                        else if (_clickCounter.NumberOfClicks == 3)
                        {
                            SelectLineAtIndex(HoveredNodeIndex.Value);
                        }

                        _isDragging = false;
                    }
                }
            }
            else
            {
                if (_isDragging && _selected && !leaveAnchor)
                {
                    if (HoveredNodeIndex.HasValue)
                    {
                        _cursor.SetIndex(HoveredNodeIndex.Value, true);
                    }
                }
            }

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

                scrollingHitTestStack.AddZone(leftRectangle, Depth.Middle,
                    () =>
                    {
                        _hoveredLetterIndex = index;
                        _hoveredSide = HorizontalDirection.Left;
                    });

                scrollingHitTestStack.AddZone(rightRectangle, Depth.Middle,
                    () =>
                    {
                        _hoveredLetterIndex = index;
                        _hoveredSide = HorizontalDirection.Right;
                    });
            }
        }
    }

    private void OnCursorMoved(int nodeIndex)
    {
        _scrollableArea.InnerWorldBoundaries =
            RectangleF.Union(InnerRectangle, _charSequence.Cache.UsedSpace);

        var nodeRect = _charSequence.Cache.RectangleAtNode(nodeIndex);
        var viewBounds = _scrollableArea.ViewBounds;
        if (!viewBounds.Contains(nodeRect))
        {
            var verticalDistance = 0f;
            if (nodeRect.Bottom > viewBounds.Bottom)
            {
                verticalDistance = nodeRect.Bottom - viewBounds.Bottom;
            }

            if (nodeRect.Top < viewBounds.Top)
            {
                verticalDistance = nodeRect.Top - viewBounds.Top;
            }

            _scrollableArea.Move(new Vector2(0, verticalDistance));
        }

        _scrollableArea.ReConstrain();
    }

    private void SelectEverything(bool leaveAnchor)
    {
        SelectRange(0, _charSequence.NumberOfChars);
    }

    private void SelectLineAtIndex(int index)
    {
        var left = _charSequence.ScanUntil(index, HorizontalDirection.Left, IsManualNewlineAtIndex);
        var right = _charSequence.ScanUntil(index, HorizontalDirection.Right, IsManualNewlineAtIndex);
        SelectRange(left, right);
    }

    private bool IsManualNewlineAtIndex(int index)
    {
        return Text[index] == '\n';
    }

    private void SelectWordFromIndex(int index)
    {
        if (IsWordBoundaryAtIndex(index))
        {
            var left = _charSequence.ScanUntil(index, HorizontalDirection.Left, IsNotWordBoundaryAtIndex);
            var right = _charSequence.ScanUntil(index, HorizontalDirection.Right, IsNotWordBoundaryAtIndex);

            if (IsNotWordBoundaryAtIndex(left))
            {
                left++;
            }

            SelectRange(left, right);
        }
        else
        {
            var nudgeLeft = _charSequence.IsValidIndex(index - 1) && IsWordBoundaryAtIndex(index - 1);
            SelectRange(GetWordBoundaryLeftOf(index + (nudgeLeft ? 1 : 0)), GetWordBoundaryRightOf(index));
        }
    }

    private void SelectRange(int left, int right)
    {
        _cursor.SetIndex(left, false);
        _cursor.SetIndex(right, true);
    }

    private void KeyBind(KeyboardFrameState keyboard, Keys button, Func<bool> checkSelectionCriteria,
        Func<ModifierKeys, bool> checkModifierCriteria,
        Action<bool> action)
    {
        var hasSelectionCriteria = checkSelectionCriteria.Invoke();
        var hasRequiredModifier = checkModifierCriteria.Invoke(keyboard.Modifiers);
        var isDown = keyboard.GetButton(button).IsDown && hasRequiredModifier && hasSelectionCriteria;
        var wasPressed = keyboard.GetButton(button).WasPressed && hasRequiredModifier && hasSelectionCriteria;
        if (wasPressed)
        {
            var arg = keyboard.Modifiers.ShiftInclusive;
            action(arg);
            _mostRecentAction = new RepeatedAction(action, arg);
        }

        if (_mostRecentAction?.Action == action)
        {
            if (isDown)
            {
                _mostRecentAction.Poll();
            }
            else
            {
                _mostRecentAction = null;
            }
        }
    }

    private void BackspaceWholeWord(bool leaveAnchor)
    {
        var index = GetWordBoundaryLeftOf(CursorIndex);

        var distance = CursorIndex - index;
        for (var i = 0; i < distance; i++)
        {
            Backspace(leaveAnchor);
        }
    }

    private void ReverseBackspaceWholeWord(bool leaveAnchor)
    {
        var index = GetWordBoundaryRightOf(CursorIndex);

        var distance = index - CursorIndex;
        for (var i = 0; i < distance; i++)
        {
            ReverseBackspace(leaveAnchor);
        }
    }

    public int GetWordBoundaryLeftOf(int index)
    {
        if (_charSequence.IsValidIndex(index - 1) && IsWordBoundaryAtIndex(index - 1))
        {
            index = _charSequence.ScanUntil(index, HorizontalDirection.Left, IsNotWordBoundaryAtIndex);
        }

        index = _charSequence.ScanUntil(index, HorizontalDirection.Left, IsWordBoundaryAtIndex);

        if (_charSequence.IsValidIndex(index + 1) && index != 0)
        {
            index++;
        }

        return index;
    }

    public void MoveWordLeft(bool leaveAnchor)
    {
        _cursor.SetIndex(GetWordBoundaryLeftOf(CursorIndex), leaveAnchor);
    }

    public int GetWordBoundaryRightOf(int index)
    {
        if (IsWordBoundaryAtIndex(index))
        {
            index = _charSequence.ScanUntil(index, HorizontalDirection.Right, IsNotWordBoundaryAtIndex);
        }

        index = _charSequence.ScanUntil(index, HorizontalDirection.Right, IsWordBoundaryAtIndex);

        return index;
    }

    public void MoveWordRight(bool leaveAnchor)
    {
        _cursor.SetIndex(GetWordBoundaryRightOf(CursorIndex), leaveAnchor);
    }

    private bool IsNotWordBoundaryAtIndex(int nodeIndex)
    {
        return !IsWordBoundaryAtIndex(nodeIndex);
    }

    private bool IsWordBoundaryAtIndex(int nodeIndex)
    {
        return nodeIndex == LastIndex || char.IsWhiteSpace(Text[nodeIndex]);
    }

    public void MoveToStartOfLine(bool leaveAnchor)
    {
        _cursor.SetIndex(_charSequence.GetNodesOnLine(CurrentLine)[0], leaveAnchor);
    }

    public void MoveToEndOfLine(bool leaveAnchor)
    {
        _cursor.SetIndex(_charSequence.GetNodesOnLine(CurrentLine)[^1], leaveAnchor);
    }

    public void MoveUp(bool leaveAnchor)
    {
        MoveVertically(-1, leaveAnchor);
    }
    
    public void MoveDown(bool leaveAnchor)
    {
        MoveVertically(1, leaveAnchor);
    }

    private void MoveVertically(int delta, bool leaveAnchor)
    {
        var currentLineIndices = _charSequence.GetNodesOnLine(_charSequence.Cache.LineNumberAt(CursorIndex));
        var targetLineIndices = _charSequence.GetNodesOnLine(_charSequence.Cache.LineNumberAt(CursorIndex) + delta);

        var currentX = 0f;
        foreach (var nodeIndex in currentLineIndices)
        {
            if (nodeIndex == CursorIndex)
            {
                break;
            }
            currentX += _charSequence.Cache.RectangleAtNode(nodeIndex).Width;
        }

        var targetLineX = 0f;
        var targetColumn = 0;
        foreach (var nodeIndex in targetLineIndices)
        {
            var nextCharWidth = _charSequence.Cache.RectangleAtNode(nodeIndex).Width;

            var distanceToCurrent = Math.Abs(targetLineX - currentX);
            var distanceToNext = Math.Abs(targetLineX + nextCharWidth - currentX);
            if (distanceToCurrent < distanceToNext)
            {
                break;
            }
            
            targetColumn++;
            targetLineX += nextCharWidth;
        }

        if (targetLineIndices.Length == 0)
        {
            return;
        }

        if (targetLineIndices.Length <= targetColumn)
        {
            _cursor.SetIndex(targetLineIndices[^1], leaveAnchor);
        }
        else
        {
            _cursor.SetIndex(targetLineIndices[targetColumn], leaveAnchor);
        }
    }

    public void PrepareDraw(Painter painter)
    {
        Client.Graphics.PushCanvas(Canvas);
        painter.BeginSpriteBatch(_scrollableArea.CanvasToScreen);
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

            var selectionRects = new List<RectangleF>();
            RectangleF? pendingSelectionRect = null;
            for (var i = _cursor.SelectedRangeStart; i < _cursor.SelectedRangeEnd; i++)
            {
                var glyphRect = _charSequence.Cache.GlyphAt(i).Rectangle.Inflated(2, 0);

                if (pendingSelectionRect == null)
                {
                    pendingSelectionRect = glyphRect;
                }
                else
                {
                    var newPendingRect = RectangleF.Union(pendingSelectionRect.Value, glyphRect);

                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (glyphRect.Height != newPendingRect.Height)
                    {
                        selectionRects.Add(pendingSelectionRect.Value);
                        pendingSelectionRect = glyphRect;
                    }
                    else
                    {
                        pendingSelectionRect = newPendingRect;
                    }
                }
            }

            if (pendingSelectionRect.HasValue)
            {
                selectionRects.Add(pendingSelectionRect.Value);
            }

            var random = new NoiseBasedRng(123);
            var selectionColor = Color.Blue;

            foreach (var selectionRect in selectionRects)
            {
                if (Client.Debug.IsActive)
                {
                    selectionColor = random.NextColor();
                }

                painter.DrawRectangle(selectionRect,
                    new DrawSettings {Depth = depth + 1, Color = selectionColor.WithMultipliedOpacity(0.5f)});
            }
        }

        painter.DrawStringWithinRectangle(_font, Text, TextAreaRectangle, _alignment,
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
                $"{CursorIndex}, line: {lineNumber}, anchor: {_cursor.SelectionAnchorIndex}, hovered: {_hoveredLetterIndex} {(_hoveredSide == HorizontalDirection.Left ? '<' : '>')}",
                TextAreaRectangle, Alignment.BottomRight, new DrawSettings {Color = Color.Black});

            for (var line = 0; line < _charSequence.Cache.NumberOfLines; line++)
            {
                painter.DrawLineRectangle(_charSequence.Cache.LineRectangleAtLine(line).Inflated(2, 2),
                    new LineDrawSettings
                        {Color = Color.ForestGreen.WithMultipliedOpacity(0.5f), Thickness = 2});
            }

            for (var i = 0; i < _charSequence.NumberOfNodes; i++)
            {
                var isLastChar = i == _charSequence.NumberOfNodes - 1;
                var rectangle = _charSequence.Cache.RectangleAtNode(i);
                var color = Color.Red;

                if (rectangle.Area == 0)
                {
                    rectangle.Size = new Vector2(_font.GetFont().Height) / 2f;
                    rectangle.Inflate(-1, -1);
                    color = isLastChar ? Color.Green : Color.Blue;
                }

                if (CursorIndex == i)
                {
                    painter.DrawRectangle(rectangle,
                        new DrawSettings
                            {Color = (isLastChar ? Color.Green : Color.Yellow).WithMultipliedOpacity(0.5f)});
                }

                painter.DrawLineRectangle(rectangle.Inflated(-1, -1), new LineDrawSettings {Color = color});
            }

            painter.DrawLineRectangle(_charSequence.Cache.UsedSpace,
                new LineDrawSettings {Depth = Depth.Back, Thickness = 2, Color = Color.Orange});
        }

        painter.EndSpriteBatch();

        painter.BeginSpriteBatch();
        var theme = new SimpleGuiTheme(Color.LightBlue, Color.DarkBlue, Color.SlateBlue,
            Client.Assets.GetFont("engine/logo-font", 32));
        _scrollableArea.DrawScrollbars(painter, theme);
        painter.EndSpriteBatch();
        Client.Graphics.PopCanvas();
    }

    public void MoveRight(bool leaveAnchor)
    {
        if (CursorIndex < _charSequence.NumberOfChars)
        {
            _cursor.SetIndex(CursorIndex + 1, leaveAnchor);
        }
    }

    public void MoveTo(int targetIndex, bool leaveAnchor)
    {
        if (targetIndex >= 0 && targetIndex < _charSequence.NumberOfNodes)
        {
            _cursor.SetIndex(targetIndex, leaveAnchor);
        }
    }

    public void MoveLeft(bool leaveAnchor)
    {
        if (CursorIndex > 0)
        {
            _cursor.SetIndex(CursorIndex - 1, leaveAnchor);
        }
    }

    public void ReverseBackspace(bool leaveAnchor)
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

    private void ClearSelectedRange(bool leaveAnchor = true)
    {
        if (_cursor.SelectedRangeSize > 0)
        {
            var size = _cursor.SelectedRangeSize;
            var end = _cursor.SelectedRangeEnd;
            _cursor.SetIndex(end, false);
            for (var i = 0; i < size; i++)
            {
                Backspace(false);
            }
        }
    }

    public void EnterCharacter(char character)
    {
        ClearSelectedRange();
        _charSequence.Insert(CursorIndex, character);
        _cursor.SetIndex(CursorIndex + 1, false);
    }

    public void Backspace(bool leaveAnchor)
    {
        if (CursorIndex > 0)
        {
            _cursor.SetIndex(CursorIndex - 1, false);
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
        public event Action? CacheUpdated;

        public void RemoveAt(int cursorIndex)
        {
            if (cursorIndex != NumberOfNodes - 1)
            {
                _nodes.RemoveAt(cursorIndex);
                Cache = Cache.Rebuild(_nodes.ToArray());
                CacheUpdated?.Invoke();
            }
        }

        public void Insert(int cursorIndex, char character)
        {
            _nodes.Insert(cursorIndex, character);
            Cache = Cache.Rebuild(_nodes.ToArray());
            CacheUpdated?.Invoke();
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

    private class TextCursor
    {
        public int Index { get; private set; }
        public int SelectionAnchorIndex { get; private set; }

        public bool HasSelection => SelectedRangeSize > 0;
        public int SelectedRangeSize => Math.Abs(Index - SelectionAnchorIndex);
        public int SelectedRangeStart => Math.Min(Index, SelectionAnchorIndex);
        public int SelectedRangeEnd => Math.Max(Index, SelectionAnchorIndex);

        public void SetIndex(int index, bool leaveAnchor)
        {
            Index = index;

            if (!leaveAnchor)
            {
                SelectionAnchorIndex = index;
            }

            MovedCursor?.Invoke(index);
        }

        public event Action<int>? MovedCursor;
    }

    private class Cache
    {
        private readonly Alignment _alignment;
        private readonly RectangleF _containerRectangle;
        private readonly IFontGetter _font;
        private readonly RectangleF[] _lineRects;
        private readonly CacheNode[] _nodes;
        private readonly char[] _originalChars;

        public Cache(char[] chars, IFontGetter font, RectangleF containerRectangle, Alignment alignment)
        {
            _originalChars = chars;
            _font = font;
            _containerRectangle = containerRectangle;
            _alignment = alignment;
            var numberOfNodes = chars.Length + 1;
            _nodes = new CacheNode[numberOfNodes];
            Text = BuildString(chars);
            BuildFormattedText();

            _lineRects = new RectangleF[NumberOfLines];
            var currentLineNumber = 0;
            RectangleF? currentLineRectangle = null;
            foreach (var node in _nodes)
            {
                if (node.OriginalGlyph.Data is FormattedText.WhiteSpaceGlyphData
                    {
                        WhiteSpaceType: WhiteSpaceType.NullTerminator
                    })
                {
                    break;
                }

                if (currentLineRectangle == null)
                {
                    currentLineRectangle = node.Rectangle;
                }
                else
                {
                    if (node.LineNumber != currentLineNumber)
                    {
                        _lineRects[currentLineNumber] = currentLineRectangle.Value;
                        currentLineNumber++;
                        currentLineRectangle = node.Rectangle;
                    }
                    else
                    {
                        currentLineRectangle = RectangleF.Union(currentLineRectangle.Value, node.Rectangle);
                    }
                }
            }

            if (currentLineRectangle.HasValue)
            {
                _lineRects[NumberOfLines - 1] = currentLineRectangle.Value;
            }
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

        public int NumberOfLines => _nodes[^1].LineNumber + 1;

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
            var nodeIndex = 0;

            foreach (var glyph in new FormattedText(_font, Text).GetGlyphs(_containerRectangle, _alignment).ToArray())
            {
                var character = '\0';
                if (glyph.Data is FormattedText.WhiteSpaceGlyphData whiteSpaceGlyphData)
                {
                    character = whiteSpaceGlyphData.WhiteSpaceType switch
                    {
                        WhiteSpaceType.Newline => '\n',
                        _ => ' '
                    };
                }

                if (glyph.Data is FormattedText.CharGlyphData charGlyphData)
                {
                    character = charGlyphData.Text;
                }

                _nodes[nodeIndex] = new CacheNode(new RectangleF(glyph.Position, glyph.Data.Size), glyph.LineNumber,
                    character, glyph);
                nodeIndex++;
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

        public FormattedText.FormattedGlyph GlyphAt(int nodeIndex)
        {
            return _nodes[nodeIndex].OriginalGlyph;
        }

        public RectangleF LineRectangleAtLine(int lineNumber)
        {
            return _lineRects[lineNumber];
        }

        public RectangleF[] LineRectangles()
        {
            return _lineRects;
        }

        private readonly record struct CacheNode(RectangleF Rectangle, int LineNumber, char Char,
            FormattedText.FormattedGlyph OriginalGlyph);
    }

    private class RepeatedAction
    {
        private readonly bool _arg;
        private DateTime _timeStarted;

        public RepeatedAction(Action<bool> action, bool arg)
        {
            // arg could be a template type, but for now all we need is bool
            _timeStarted = DateTime.Now;
            _arg = arg;
            Action = action;
        }

        public Action<bool> Action { get; }

        public void Poll()
        {
            var timeSinceStart = DateTime.Now - _timeStarted;
            var staticFriction = 0.5f;
            var tick = 0.05f;

            if (timeSinceStart.TotalSeconds - staticFriction > tick)
            {
                Action(_arg);
                _timeStarted = _timeStarted.AddSeconds(tick);
            }
        }
    }

    private class ClickCounter
    {
        private Vector2 _mousePosition;
        private DateTime _timeOfLastClick = DateTime.UnixEpoch;
        public int NumberOfClicks { get; private set; }

        public void Increment(Vector2 mousePosition)
        {
            var interval = 0.2f;
            var timeSinceLastClick = DateTime.Now - _timeOfLastClick;
            if (timeSinceLastClick.TotalSeconds < interval && mousePosition == _mousePosition)
            {
                NumberOfClicks++;
            }
            else
            {
                NumberOfClicks = 1;
            }

            _mousePosition = mousePosition;
            _timeOfLastClick = DateTime.Now;
        }
    }
}
