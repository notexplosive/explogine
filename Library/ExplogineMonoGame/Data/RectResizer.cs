using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public class RectResizer
{
    private readonly Drag<RectangleF> _edgeDrag = new();
    private RectEdge _edgeGrabbed;
    private RectEdge _edgeHovered;

    public bool HasGrabbed => _edgeGrabbed != RectEdge.None;
    public event Action? Initiated;
    public event Action? Finished;

    public RectangleF GetResizedRect(ConsumableInput input, HitTestStack hitTestStack, RectangleF startingRect,
        Depth depth, Matrix screenToCanvas, int grabHandleThickness = 50, Point minimumSize = default)
    {
        var leftButton = input.Mouse.GetButton(MouseButton.Left);
        var mouseDown = leftButton.IsDown;
        var mousePressed = leftButton.WasPressed;

        hitTestStack.BeforeLayerResolved += () => { _edgeHovered = RectEdge.None; };

        foreach (var edge in Enum.GetValues<RectEdge>())
        {
            if (edge != RectEdge.None)
            {
                hitTestStack.AddZone(startingRect.GetRectangleFromEdge(edge, grabHandleThickness), depth, () =>
                {
                    _edgeHovered = edge;
                    if (!mouseDown)
                    {
                        Client.Cursor.Set(MouseCursorExtensions.GetCursorForEdge(edge));
                    }
                });
            }
        }

        if (_edgeHovered != RectEdge.None && mousePressed)
        {
            Initiated?.Invoke();
            _edgeDrag.Start(startingRect);
            _edgeGrabbed = _edgeHovered;
        }

        if (!mouseDown)
        {
            var wasDragging = _edgeDrag.IsDragging;
            _edgeDrag.End();
            _edgeGrabbed = RectEdge.None;

            if (wasDragging)
            {
                Finished?.Invoke();
            }
        }

        // I think we can just use hitTestStack.WorldMatrix here??
        var delta = input.Mouse.Delta(screenToCanvas);
        _edgeDrag.AddDelta(delta);

        if (_edgeDrag.IsDragging)
        {
            Client.Cursor.Set(MouseCursorExtensions.GetCursorForEdge(_edgeGrabbed));
            var sizeDelta = _edgeDrag.TotalDelta;
            var overflow = startingRect.Size + sizeDelta - minimumSize.ToVector2();
            if (overflow.X < 0)
            {
                sizeDelta.X -= overflow.X;
            }

            if (overflow.Y < 0)
            {
                sizeDelta.Y -= overflow.Y;
            }

            var newRect = _edgeDrag.StartingValue.ResizedOnEdge(_edgeGrabbed, sizeDelta);
            return newRect;
        }

        return startingRect;
    }
}
