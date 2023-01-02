using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Input;

internal static class GamePadStateExtensions
{
    public static Buttons[] PressedButtons(this GamePadState buttons)
    {
        var result = new List<Buttons>();

        foreach (var button in Enum.GetValues<Buttons>())
        {
            if (buttons.IsButtonDown(button))
            {
                result.Add(button);
            }
        }

        return result.ToArray();
    }
}
