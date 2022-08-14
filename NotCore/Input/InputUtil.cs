using Microsoft.Xna.Framework.Input;

namespace NotCore.Input;

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

    public static bool CheckIsDown(ButtonState[] gamePadButtonStates, GamePadButton button)
    {
        return InputUtil.CheckIsDown(gamePadButtonStates, (int) button);
    }

    public static bool CheckIsDown(ButtonState[] mouseButtonStates, MouseButton mouseButton)
    {
        return InputUtil.CheckIsDown(mouseButtonStates, (int) mouseButton);
    }
}
