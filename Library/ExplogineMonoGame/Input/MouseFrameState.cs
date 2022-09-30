using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Input;

public readonly struct MouseFrameState
{
    public MouseFrameState(InputSnapshot current, InputSnapshot previous)
    {
        Current = current;
        Previous = previous;
    }

    private InputSnapshot Previous { get; }
    private InputSnapshot Current { get; }

    public Vector2 Position(Matrix transform)
    {
        return Vector2.Transform(Current.MousePosition, transform);
    }

    public Vector2 Delta(Matrix transform)
    {
        return Vector2.Transform(Current.MousePosition, transform) -
               Vector2.Transform(Previous.MousePosition, transform);
    }

    public ButtonFrameState GetButton(MouseButton mouseButton)
    {
        var isDown = InputUtil.CheckIsDown(Current.MouseButtonStates, mouseButton);
        var wasDown = InputUtil.CheckIsDown(Previous.MouseButtonStates, mouseButton);

        return new ButtonFrameState(isDown, wasDown);
    }

    public bool IsAnyButtonDown()
    {
        return Current.MouseButtonStates.Any(state => state == ButtonState.Pressed);
    }

    public bool WasAnyButtonPressedOrReleased()
    {
        foreach (var (buttonState, button) in EachButton())
        {
            if (buttonState.WasPressed || buttonState.WasReleased)
            {
                return true;
            }
        }

        return false;
    }

    public int ScrollDelta()
    {
        return Current.ScrollValue - Previous.ScrollValue;
    }

    public IEnumerable<(ButtonFrameState, MouseButton)> EachButton()
    {
        foreach (var mouseButton in Enum.GetValues<MouseButton>())
        {
            yield return (GetButton(mouseButton), mouseButton);
        }
    }
}
