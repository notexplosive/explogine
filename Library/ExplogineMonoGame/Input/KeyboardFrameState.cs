using System;
using System.Collections;
using System.Collections.Generic;
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

    public char[] GetEnteredCharacters()
    {
        return Current.TextEntered.Characters;
    }
    
    public ButtonFrameState GetButton(Keys key)
    {
        var isDown = Current.IsDown(key);
        var wasDown = Previous.IsDown(key);
        return new ButtonFrameState(isDown, wasDown);
    }

    public ModifierKeys Modifiers
    {
        get
        {
            var control = Current.IsDown(Keys.LeftControl)
                          || Current.IsDown(Keys.RightControl);
            var alt = Current.IsDown(Keys.LeftAlt)
                      || Current.IsDown(Keys.RightAlt);
            var shift = Current.IsDown(Keys.LeftShift)
                        || Current.IsDown(Keys.RightShift);

            return new ModifierKeys(control, alt, shift);
        }
    }

    private InputSnapshot Previous { get; }
    private InputSnapshot Current { get; }

    public bool IsAnyKeyDown()
    {
        return Current.PressedKeys is {Length: > 0};
    }

    public IEnumerable<(ButtonFrameState, Keys)> EachKey()
    {
        // This might be slow, technically O(n^2)
        foreach (var key in Enum.GetValues<Keys>())
        {
            yield return (GetButton(key), key);
        }
    }
}
