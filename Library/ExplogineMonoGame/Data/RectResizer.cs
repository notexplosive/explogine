﻿using System;
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
        Depth depth, int grabHandleThickness = 50)
    {
        var leftButton = input.Mouse.GetButton(MouseButton.Left);
        var mouseDown = leftButton.IsDown;
        var mousePressed = leftButton.WasPressed;

        hitTestStack.BeforeLayerResolved += () => { _edgeHovered = RectEdge.None; };

        foreach (var edge in Enum.GetValues<RectEdge>())
        {
            if (edge != RectEdge.None)
            {
                hitTestStack.AddZone(startingRect.GetEdgeRect(edge, grabHandleThickness), depth, () =>
                {
                    _edgeHovered = edge;
                    if (!mouseDown)
                    {
                        Client.Window.SetCursor(MouseCursorExtensions.GetCursorForEdge(edge));
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

        var delta = input.Mouse.CanvasDelta();
        _edgeDrag.AddDelta(delta);

        if (_edgeDrag.IsDragging)
        {
            Client.Window.SetCursor(MouseCursorExtensions.GetCursorForEdge(_edgeGrabbed));
            var newRect = _edgeDrag.StartingValue.ResizedOnEdge(_edgeGrabbed, _edgeDrag.TotalDelta);
            return newRect;
        }

        return startingRect;
    }
}
