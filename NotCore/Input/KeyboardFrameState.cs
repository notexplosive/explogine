using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace NotCore.Input;

public readonly struct KeyboardFrameState
{
    public KeyboardFrameState(InputSnapshot current, InputSnapshot previous)
    {
        Current = current;
        Previous = previous;
    }

    public bool IsKeyDown(Keys key)
    {
        return Current.PressedKeys.Contains(key);
    }
    
    public bool WasKeyPressed(Keys key)
    {
        return IsKeyDown(key) && !Previous.PressedKeys.Contains(key);
    }
    
    public bool WasKeyReleased(Keys key)
    {
        return IsKeyUp(key) && Previous.PressedKeys.Contains(key);
    }

    public bool IsKeyUp(Keys key)
    {
        return !IsKeyDown(key);
    }

    private InputSnapshot Previous { get; }
    private InputSnapshot Current { get; }

    public bool IsAnyKeyDown()
    {
        return Current.PressedKeys.Length > 0;
    }
}
