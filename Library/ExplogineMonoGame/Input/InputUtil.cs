using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Input;

internal static class InputUtil
{
    private static bool CheckIsDown(ButtonState[]? buttonStates, int index)
    {
        if (buttonStates == null)
        {
            return false;
        }

        return buttonStates[index] == ButtonState.Pressed;
    }

    public static bool CheckIsDown(Keys[]? pressedKeys, Keys key)
    {
        return pressedKeys != null && pressedKeys.Contains(key);
    }

    public static bool CheckIsDown(Buttons[]? pressedButtons, Buttons button)
    {
        return pressedButtons != null && pressedButtons.Contains(button);
    }

    public static bool CheckIsDown(ButtonState[]? mouseButtonStates, MouseButton mouseButton)
    {
        return mouseButtonStates != null && InputUtil.CheckIsDown(mouseButtonStates, (int) mouseButton);
    }
}
