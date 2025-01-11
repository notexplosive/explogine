using System;
using System.Collections.Generic;
using ExplogineCore;
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
            var nativeControl = Current.IsDown(Keys.LeftControl)
                                || Current.IsDown(Keys.RightControl);
            var alt = Current.IsDown(Keys.LeftAlt)
                      || Current.IsDown(Keys.RightAlt);
            var shift = Current.IsDown(Keys.LeftShift)
                        || Current.IsDown(Keys.RightShift);
            var nativeCommand = Current.IsDown(Keys.LeftWindows)
                                || Current.IsDown(Keys.RightWindows);

            var effectiveControl = nativeControl;

            if (PlatformApi.OperatingSystem() == SupportedOperatingSystem.MacOs)
            {
                // If we're on macOS, CTRL and Command both do the same thing
                effectiveControl = nativeCommand || nativeControl;
            }

            return new ModifierKeys(effectiveControl, alt, shift);
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
