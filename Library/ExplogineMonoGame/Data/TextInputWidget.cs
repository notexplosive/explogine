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
    private readonly ClickCounter _clickCounter = new();
    private readonly bool _isSingleLine;
    private readonly HoverState _isTextAreaHovered;
    private readonly bool _showScrollbar;
    private int? _hoveredLetterIndex;
    private HorizontalDirection _hoveredSide;
    private bool _isDragging;
    private RepeatedAction? _mostRecentAction;

    public TextInputWidget(Vector2 position, Point size, IFontGetter font, Settings settings)
        : base(position, size, settings.Depth)
    {
        Font = font;
        _showScrollbar = settings.ShowScrollbar;
        _isSingleLine = settings.IsSingleLine;

        var shouldScrollY = ScrollableAxis == Axis.Y;
        ScrollableArea = new ScrollableArea(Size, InnerRectangle, Depth.Front)
        {
            EnableInput = new XyBool(!shouldScrollY, shouldScrollY)
        };

        _isTextAreaHovered = new HoverState();
        Cursor.MovedCursor += OnCursorMoved;
        // Content will need to be rebuilt every time these values change
        Content = new TextInputContent(font, settings.StartingText, TextAreaRectangle, Alignment,
            _isSingleLine);
        Content.CacheUpdated += () => OnCursorMoved(CursorIndex);
    }

    public Alignment Alignment { get; } = Alignment.TopLeft;
    private TextInputContent Content { get; }
    public TextCursor Cursor { get; } = new();
    public IFontGetter Font { get; }
    public ScrollableArea ScrollableArea { get; }
    public bool Selected { get; private set; }
    public string Text => Content.Text;
    public int CursorIndex => Cursor.Index;
    public int MarginSize => 5;

    public RectangleF TextAreaRectangle
    {
        get
        {
            var withMargin = InnerRectangle.Inflated(-MarginSize, -MarginSize);
            var scrollBarSide = _isSingleLine ? RectEdge.Bottom : RectEdge.Right;
            return withMargin.ResizedOnEdge(scrollBarSide,
                new Vector2(-ScrollableArea.ScrollBarWidth).JustAxis(ScrollableAxis));
        }
    }

    private Axis ScrollableAxis
    {
        get
        {
            if (_isSingleLine)
            {
                return Axis.X;
            }

            return Axis.Y;
        }
    }

    public RectangleF InnerRectangle => new(Vector2.Zero, Rectangle.Size);
    public int LastIndex => Content.NumberOfChars;
    public int CurrentColumn => Content.GetColumn(CursorIndex);
    public int CurrentLine => Content.LineNumberAt(CursorIndex);

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

            if (result > Content.NumberOfChars)
            {
                result = Content.NumberOfChars;
            }

            return result;
        }
    }

    public RectangleF ContainerRectangle => Content.ContainerRectangle;

    public RectangleF CursorRectangle
    {
        get
        {
            var index = CursorIndex;
            var cursorRect = Content.RectangleAtNode(index);
            var size = cursorRect.Size;
            size.X = 2f;
            cursorRect.Offset(-size.X / 2f, 0);
            return new RectangleF(cursorRect.TopLeft, size);
        }
    }

    private bool IsShowingScrollbar => ScrollableArea.CanScrollAlong(ScrollableAxis) && _showScrollbar;

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        UpdateHovered(hitTestStack);

        if (Selected)
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
                return Cursor.HasSelection;
            }

            bool DoesNotHaveSelection()
            {
                return !Cursor.HasSelection;
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

        var wrapperHitTestStack = hitTestStack.AddLayer(Matrix.Identity, Depth, Rectangle);
        var unscrolledHitTestStack = wrapperHitTestStack.AddLayer(ScreenToCanvas, Depth.Middle);
        var scrollingHitTestStack =
            wrapperHitTestStack.AddLayer(ScrollableArea.ScreenToCanvas * ScreenToCanvas, Depth.Middle + 1);

        if (IsShowingScrollbar)
        {
            ScrollableArea.UpdateInput(input, unscrolledHitTestStack);
        }

        scrollingHitTestStack.BeforeLayerResolved += () => { _hoveredLetterIndex = null; };

        unscrolledHitTestStack.AddZone(TextAreaRectangle, Depth.Middle, _isTextAreaHovered, true);

        var scrollDelta = input.Mouse.ScrollDelta();
        ScrollableArea.Move(new Vector2(0, -scrollDelta / 5f));

        scrollingHitTestStack.AddInfiniteZone(Depth.Back, () =>
        {
            if (Selected)
            {
                var lineRectangles = Content.LineRectangles();
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
                        var nodesOnLine = Content.GetNodesOnLine(lineNumber);
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
                Selected = true;

                if (HoveredNodeIndex.HasValue)
                {
                    Cursor.SetIndex(HoveredNodeIndex.Value, leaveAnchor);

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
                if (_isDragging && Selected && !leaveAnchor)
                {
                    if (HoveredNodeIndex.HasValue)
                    {
                        Cursor.SetIndex(HoveredNodeIndex.Value, true);
                    }
                }
            }

            for (var i = 0; i < Content.NumberOfChars; i++)
            {
                var index = i; // index will be captured so we need to set aside a variable
                var charRectangle = Content.RectangleAtNode(i);
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

    /// <summary>
    ///     Fired when you attempt a newline in single line mode
    /// </summary>
    public event Action? Submitted;

    private void OnCursorMoved(int nodeIndex)
    {
        ScrollableArea.InnerWorldBoundaries =
            RectangleF.Union(InnerRectangle, Content.UsedSpace);

        var nodeRect = Content.RectangleAtNode(nodeIndex);
        var viewBounds = ScrollableArea.ViewBounds;

        float Far(RectangleF rect)
        {
            if (ScrollableAxis == Axis.Y)
            {
                return rect.Bottom;
            }

            return rect.Right;
        }

        float Near(RectangleF rect)
        {
            if (ScrollableAxis == Axis.Y)
            {
                return rect.Top;
            }

            return rect.Left;
        }

        if (!viewBounds.Contains(nodeRect))
        {
            var distance = 0f;
            if (Far(nodeRect) > Far(viewBounds))
            {
                distance = Far(nodeRect) - Far(viewBounds);
            }

            if (Near(nodeRect) < Near(viewBounds))
            {
                distance = Near(nodeRect) - Near(viewBounds);
            }

            ScrollableArea.Move(new Vector2(distance).JustAxis(ScrollableAxis) +
                                new Vector2(distance).Normalized() * MarginSize);
        }

        ScrollableArea.ReConstrain();
    }

    private void SelectEverything(bool leaveAnchor)
    {
        SelectRange(0, Content.NumberOfChars);
    }

    private void SelectLineAtIndex(int index)
    {
        var left = Content.ScanUntil(index, HorizontalDirection.Left, IsManualNewlineAtIndex);
        var right = Content.ScanUntil(index, HorizontalDirection.Right, IsManualNewlineAtIndex);
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
            var left = Content.ScanUntil(index, HorizontalDirection.Left, IsNotWordBoundaryAtIndex);
            var right = Content.ScanUntil(index, HorizontalDirection.Right, IsNotWordBoundaryAtIndex);

            if (IsNotWordBoundaryAtIndex(left))
            {
                left++;
            }

            SelectRange(left, right);
        }
        else
        {
            var nudgeLeft = Content.IsValidIndex(index - 1) && IsWordBoundaryAtIndex(index - 1);
            SelectRange(GetWordBoundaryLeftOf(index + (nudgeLeft ? 1 : 0)), GetWordBoundaryRightOf(index));
        }
    }

    private void SelectRange(int left, int right)
    {
        Cursor.SetIndex(left, false);
        Cursor.SetIndex(right, true);
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
        if (CursorIndex > 0)
        {
            Cursor.SetIndex(CursorIndex - distance, false);
            Content.RemoveSeveralAt(CursorIndex, distance);
        }
    }

    private void ReverseBackspaceWholeWord(bool leaveAnchor)
    {
        var index = GetWordBoundaryRightOf(CursorIndex);

        var distance = index - CursorIndex;
        Content.RemoveSeveralAt(CursorIndex, distance);
    }

    public int GetWordBoundaryLeftOf(int index)
    {
        if (Content.IsValidIndex(index - 1) && IsWordBoundaryAtIndex(index - 1))
        {
            index = Content.ScanUntil(index, HorizontalDirection.Left, IsNotWordBoundaryAtIndex);
        }

        index = Content.ScanUntil(index, HorizontalDirection.Left, IsWordBoundaryAtIndex);

        if (Content.IsValidIndex(index + 1) && index != 0)
        {
            index++;
        }

        return index;
    }

    private void MoveWordLeft(bool leaveAnchor)
    {
        Cursor.SetIndex(GetWordBoundaryLeftOf(CursorIndex), leaveAnchor);
    }

    public int GetWordBoundaryRightOf(int index)
    {
        if (IsWordBoundaryAtIndex(index))
        {
            index = Content.ScanUntil(index, HorizontalDirection.Right, IsNotWordBoundaryAtIndex);
        }

        index = Content.ScanUntil(index, HorizontalDirection.Right, IsWordBoundaryAtIndex);

        return index;
    }

    private void MoveWordRight(bool leaveAnchor)
    {
        Cursor.SetIndex(GetWordBoundaryRightOf(CursorIndex), leaveAnchor);
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
        Cursor.SetIndex(Content.GetNodesOnLine(CurrentLine)[0], leaveAnchor);
    }

    public void MoveToEndOfLine(bool leaveAnchor)
    {
        Cursor.SetIndex(Content.GetNodesOnLine(CurrentLine)[^1], leaveAnchor);
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
        var currentLineIndices = Content.GetNodesOnLine(Content.LineNumberAt(CursorIndex));
        var targetLineIndices = Content.GetNodesOnLine(Content.LineNumberAt(CursorIndex) + delta);

        var currentX = 0f;
        foreach (var nodeIndex in currentLineIndices)
        {
            if (nodeIndex == CursorIndex)
            {
                break;
            }

            currentX += Content.RectangleAtNode(nodeIndex).Width;
        }

        var targetLineX = 0f;
        var targetColumn = 0;
        foreach (var nodeIndex in targetLineIndices)
        {
            var nextCharWidth = Content.RectangleAtNode(nodeIndex).Width;

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
            Cursor.SetIndex(targetLineIndices[^1], leaveAnchor);
        }
        else
        {
            Cursor.SetIndex(targetLineIndices[targetColumn], leaveAnchor);
        }
    }

    public void PrepareDraw(Painter painter, IGuiTheme theme)
    {
        Client.Graphics.PushCanvas(Canvas);
        theme.DrawTextInput(painter, this);

        if (IsShowingScrollbar)
        {
            painter.BeginSpriteBatch();
            ScrollableArea.DrawScrollbars(painter, theme);
            painter.EndSpriteBatch();
        }

        Client.Graphics.PopCanvas();
    }

    public void MoveRight(bool leaveAnchor)
    {
        if (CursorIndex < Content.NumberOfChars)
        {
            Cursor.SetIndex(CursorIndex + 1, leaveAnchor);
        }
    }

    public void MoveTo(int targetIndex, bool leaveAnchor)
    {
        if (targetIndex >= 0 && targetIndex < Content.NumberOfNodes)
        {
            Cursor.SetIndex(targetIndex, leaveAnchor);
        }
    }

    public void MoveLeft(bool leaveAnchor)
    {
        if (CursorIndex > 0)
        {
            Cursor.SetIndex(CursorIndex - 1, leaveAnchor);
        }
    }

    public void ReverseBackspace(bool leaveAnchor)
    {
        Content.RemoveAt(CursorIndex);
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
                        if (!_isSingleLine)
                        {
                            EnterCharacter('\n');
                        }
                        else
                        {
                            Submitted?.Invoke();
                        }
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
        if (Cursor.SelectedRangeSize > 0)
        {
            var size = Cursor.SelectedRangeSize;
            Cursor.SetIndex(Cursor.SelectedRangeStart, false);
            Content.RemoveSeveralAt(CursorIndex, size);
        }
    }

    public void EnterCharacter(char character)
    {
        ClearSelectedRange();
        Content.Insert(CursorIndex, character);
        Cursor.SetIndex(CursorIndex + 1, false);
    }

    public void Backspace(bool leaveAnchor)
    {
        if (CursorIndex > 0)
        {
            Cursor.SetIndex(CursorIndex - 1, false);
            Content.RemoveAt(CursorIndex);
        }
    }

    public int LineLength(int line)
    {
        return Content.CountNodesOnLine(line);
    }

    public void DrawDebugInfo(Painter painter)
    {
        var lineNumber = Content.LineNumberAt(CursorIndex);
        painter.DrawStringWithinRectangle(Client.Assets.GetFont("engine/console-font", 16),
            $"{CursorIndex}, line: {lineNumber}, anchor: {Cursor.SelectionAnchorIndex}, hovered: {_hoveredLetterIndex} {(_hoveredSide == HorizontalDirection.Left ? '<' : '>')}",
            TextAreaRectangle, Alignment.BottomRight, new DrawSettings {Color = Color.Black});

        for (var line = 0; line < Content.NumberOfLines; line++)
        {
            painter.DrawLineRectangle(Content.LineRectangleAtLine(line).Inflated(2, 2),
                new LineDrawSettings
                    {Color = Color.ForestGreen.WithMultipliedOpacity(0.5f), Thickness = 2});
        }

        for (var i = 0; i < Content.NumberOfNodes; i++)
        {
            var isLastChar = i == Content.NumberOfNodes - 1;
            var rectangle = Content.RectangleAtNode(i);
            var color = Color.Red;

            if (rectangle.Area == 0)
            {
                rectangle.Size = new Vector2(Font.GetFont().Height) / 2f;
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

        painter.DrawLineRectangle(Content.UsedSpace,
            new LineDrawSettings {Depth = Depth.Back, Thickness = 2, Color = Color.Orange});
    }

    public IEnumerable<RectangleF> GetSelectionRectangles()
    {
        RectangleF? pendingSelectionRect = null;
        for (var i = Cursor.SelectedRangeStart; i < Cursor.SelectedRangeEnd; i++)
        {
            var glyphRect = Content.GlyphAt(i).Rectangle.Inflated(2, 0);

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
                    yield return pendingSelectionRect.Value;
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
            yield return pendingSelectionRect.Value;
        }
    }

    public readonly record struct Settings(Depth Depth, bool IsSingleLine, bool ShowScrollbar, string StartingText);

    private delegate bool ScanFindDelegate(int index);

    private class TextInputContent
    {
        private readonly List<char> _nodes = new();

        public TextInputContent(IFontGetter font, string text, RectangleF containerRectangle, Alignment alignment,
            bool isSingleLine)
        {
            foreach (var character in text)
            {
                _nodes.Add(character);
            }

            Cache = new TextInputCache(_nodes.ToArray(), font, containerRectangle, alignment, isSingleLine);
        }

        private TextInputCache Cache { get; set; }
        public int NumberOfNodes => Cache.Text.Length + 1;
        public int NumberOfChars => Cache.Text.Length;
        public RectangleF UsedSpace => Cache.UsedSpace;
        public int NumberOfLines => Cache.NumberOfLines;
        public string Text => Cache.Text;
        public RectangleF ContainerRectangle => Cache.ContainerRectangle;

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

        public void RemoveSeveralAt(int cursorIndex, int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (cursorIndex != NumberOfNodes - 1)
                {
                    _nodes.RemoveAt(cursorIndex);
                }
            }

            Cache = Cache.Rebuild(_nodes.ToArray());
            CacheUpdated?.Invoke();
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

        public RectangleF RectangleAtNode(int index)
        {
            return Cache.RectangleAtNode(index);
        }

        public FormattedText.FormattedGlyph GlyphAt(int index)
        {
            return Cache.GlyphAt(index);
        }

        public int LineNumberAt(int cursorIndex)
        {
            return Cache.LineNumberAt(cursorIndex);
        }

        public RectangleF LineRectangleAtLine(int line)
        {
            return Cache.LineRectangleAtLine(line);
        }

        public RectangleF[] LineRectangles()
        {
            return Cache.LineRectangles();
        }
    }

    public class TextCursor
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

    private class TextInputCache
    {
        private readonly Alignment _alignment;
        private readonly IFontGetter _font;
        private readonly bool _isSingleLine;
        private readonly RectangleF[] _lineRects;
        private readonly CacheNode[] _nodes;

        public TextInputCache(char[] chars, IFontGetter font, RectangleF containerRectangle, Alignment alignment,
            bool isSingleLine)
        {
            _font = font;
            ContainerRectangle = containerRectangle;
            if (isSingleLine)
            {
                ContainerRectangle =
                    new RectangleF(containerRectangle.Location, new Vector2(float.MaxValue, containerRectangle.Height));
            }

            _alignment = alignment;
            _isSingleLine = isSingleLine;
            var numberOfNodes = chars.Length + 1;
            _nodes = new CacheNode[numberOfNodes];
            Text = BuildString(chars);
            BuildFormattedText();
            _lineRects = BuildLineRects();
        }

        public RectangleF ContainerRectangle { get; }

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

        private RectangleF[] BuildLineRects()
        {
            var result = new RectangleF[NumberOfLines];
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
                        result[currentLineNumber] = currentLineRectangle.Value;
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
                result[NumberOfLines - 1] = currentLineRectangle.Value;
            }

            return result;
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
            var nodeIndex = 0;

            foreach (var glyph in new FormattedText(_font, Text).GetGlyphs(ContainerRectangle, _alignment).ToArray())
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

        public TextInputCache Rebuild(char[] newNodes)
        {
            return new TextInputCache(newNodes, _font, ContainerRectangle, _alignment, _isSingleLine);
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
