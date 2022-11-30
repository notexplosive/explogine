using System;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Data;

public static class MouseCursorExtensions
{
    public static MouseCursor GetCursorForEdge(RectEdge edge)
    {
        switch (edge)
        {
            case RectEdge.Bottom:
            case RectEdge.Top:
                return MouseCursor.SizeNS;
            case RectEdge.Left:
            case RectEdge.Right:
                return MouseCursor.SizeWE;
            case RectEdge.TopLeft:
            case RectEdge.BottomRight:
                return MouseCursor.SizeNWSE;
            case RectEdge.TopRight:
            case RectEdge.BottomLeft:
                return MouseCursor.SizeNESW;
            default:
                return MouseCursor.Arrow;
        }
    }
}
