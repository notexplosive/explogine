using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Input;

public readonly struct KeyboardFrameState
{
    public KeyboardFrameState(InputSnapshot current, InputSnapshot previous)
    {
        Current = current;
        Previous = previous;
    }

    public ButtonFrameState GetButton(Keys key)
    {
        var isDown = Current.PressedKeys.Contains(key);
        var wasDown = Previous.PressedKeys.Contains(key);
        return new ButtonFrameState(isDown, wasDown);
    }

    private InputSnapshot Previous { get; }
    private InputSnapshot Current { get; }

    public bool IsAnyKeyDown()
    {
        return Current.PressedKeys.Length > 0;
    }
}
