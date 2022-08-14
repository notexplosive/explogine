using Microsoft.Xna.Framework.Input;

namespace NotCore.Input;

public readonly struct GamePadFrameState
{
    public GamePadFrameState(InputSnapshot current, InputSnapshot previous)
    {
        Current = current;
        Previous = previous;
    }

    public bool IsButtonDown(GamePadButton button)
    {
        return InputUtil.CheckIsDown(Current.GamePadButtonStates, button);
    }

    public bool IsButtonUp(GamePadButton button)
    {
        return !IsButtonDown(button);
    }

    public bool WasButtonPressed(GamePadButton button)
    {
        return IsButtonDown(button) && !InputUtil.CheckIsDown(Previous.GamePadButtonStates, button);
    }
    
    public bool WasButtonReleased(GamePadButton button)
    {
        return IsButtonUp(button) && InputUtil.CheckIsDown(Previous.GamePadButtonStates, button);
    } 
    
    private InputSnapshot Previous { get; }
    private InputSnapshot Current { get; }

    public bool IsAnyButtonDown()
    {
        foreach (var state in Current.GamePadButtonStates)
        {
            if (state == ButtonState.Pressed)
            {
                return true;
            }
        }

        return false;
    }
}
