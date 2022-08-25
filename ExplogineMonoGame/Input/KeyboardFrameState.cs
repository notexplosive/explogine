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

    public ModifierKeys Modifiers
    {
        get
        {
            var control = Current.PressedKeys.Contains(Keys.LeftControl)
                          || Current.PressedKeys.Contains(Keys.RightControl);
            var alt = Current.PressedKeys.Contains(Keys.LeftAlt)
                      || Current.PressedKeys.Contains(Keys.RightAlt);
            var shift = Current.PressedKeys.Contains(Keys.LeftShift)
                        || Current.PressedKeys.Contains(Keys.RightShift);

            return new ModifierKeys(control, alt, shift);
        }
    }

    private InputSnapshot Previous { get; }
    private InputSnapshot Current { get; }

    public bool IsAnyKeyDown()
    {
        return Current.PressedKeys.Length > 0;
    }
}
