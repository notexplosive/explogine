using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NotCore.Input;

public readonly struct MouseFrameState
{
    public MouseFrameState(InputSnapshot current, InputSnapshot previous)
    {
        Current = current;
        Previous = previous;
    }

    private InputSnapshot Previous { get; }
    private InputSnapshot Current { get; }
    public Vector2 Position => Current.MousePosition;

    public bool IsButtonDown(MouseButton mouseButton)
    {
        return InputUtil.CheckIsDown(Current.MouseButtonStates, mouseButton);
    }
    
    public bool IsButtonUp(MouseButton mouseButton)
    {
        return !IsButtonDown(mouseButton);
    }

    public bool WasButtonPressed(MouseButton mouseButton)
    {
        return IsButtonDown(mouseButton) && !InputUtil.CheckIsDown(Previous.MouseButtonStates, mouseButton);
    }
    
    public bool WasButtonReleased(MouseButton mouseButton)
    {
        return IsButtonUp(mouseButton) && InputUtil.CheckIsDown(Previous.MouseButtonStates, mouseButton);
    }

    public bool IsAnyButtonDown()
    {
        foreach (var state in Current.MouseButtonStates)
        {
            if (state == ButtonState.Pressed)
            {
                return true;
            }
        }

        return false;
    }
}
