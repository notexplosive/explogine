using System;
using ExplogineCore.Data;
using ExplogineMonoGame.HitTesting;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Data;

public class ResizableRect
{
    private RectEdge _edgeGrabbed;
    private RectEdge _edgeHovered;
    private readonly Drag<RectangleF> _edgeDrag = new();
    
    public RectangleF UpdateInput(InputFrameState input, HitTestStack hitTestStack, RectangleF startingRect, Depth depth)
    {
        var leftButton = input.Mouse.GetButton(MouseButton.Left);
        var mouseDown = leftButton.IsDown;
        var mousePressed = leftButton.WasPressed;

        hitTestStack.AddFallThrough(() =>
        {
            _edgeHovered = RectEdge.None;
        });
        
        foreach (var edge in Enum.GetValues<RectEdge>())
        {
            if (edge != RectEdge.None)
            {
                hitTestStack.Add(startingRect.GetEdgeRect(edge, 50), depth, () =>
                {
                    _edgeHovered = edge;
                    Client.Window.SetCursor(MouseCursorExtensions.GetCursorForEdge(edge));
                });
            }
        }

        if (_edgeHovered != RectEdge.None && mousePressed)
        {
            _edgeDrag.Start(startingRect);
            _edgeGrabbed = _edgeHovered;
        }

        if (!mouseDown)
        {
            _edgeDrag.End();
            _edgeGrabbed = RectEdge.None;
        }
        
        var delta = input.Mouse.CanvasDelta();
        _edgeDrag.AddDelta(delta);
        
        if (_edgeDrag.IsDragging)
        {
            var newRect = _edgeDrag.StartingValue.ResizedOnEdge(_edgeGrabbed, _edgeDrag.TotalDelta);
            Client.Debug.Log(newRect.Width);
            return newRect;
        }

        return startingRect;
    }
}
