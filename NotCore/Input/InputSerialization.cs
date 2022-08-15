using System;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace NotCore.Input;

public static class InputSerialization
{
    public static readonly GamePadButton[] AllGamePadButtons = Enum.GetValues<GamePadButton>();
    public static readonly uint NumberOfGamepadButtons = (uint) InputSerialization.AllGamePadButtons.Length;
    public static readonly uint NumberOfMouseButtons = (uint) Enum.GetValues<MouseButton>().Length;

    public static int StatesToInt(ButtonState[] states)
    {
        if (states.Length > 32)
        {
            throw new Exception("This array is too long to compress");
        }

        var result = 0;
        for (var i = 0; i < states.Length; i++)
        {
            var mask = 1 << i;
            if (states[i] == ButtonState.Pressed)
            {
                result |= mask;
            }
        }

        return result;
    }

    public static ButtonState[] IntToStates(int compressed, uint size)
    {
        var result = new ButtonState[size];

        for (var i = 0; i < result.Length; i++)
        {
            var shiftedByIndex = 1 << i;
            if ((compressed & shiftedByIndex) != 0)
            {
                result[i] = ButtonState.Pressed;
            }
        }

        return result;
    }

    public static string AsString(InputSnapshot input)
    {
        var mouse =
            $"{input.MousePosition.X},{input.MousePosition.Y},{InputSerialization.StatesToInt(input.MouseButtonStates)}";
        var gamepad =
            $"{input.GamePadLeftTrigger},{input.GamePadRightTrigger},{input.LeftThumbstick.X},{input.LeftThumbstick.Y},{input.RightThumbstick.X},{input.RightThumbstick.Y},{InputSerialization.StatesToInt(input.GamePadButtonStates)}";

        var keyboardBuilder = new StringBuilder();
        for (var i = 0; i < input.PressedKeys.Length; i++)
        {
            var key = input.PressedKeys[i];
            keyboardBuilder.Append((int) key);

            if (i < input.PressedKeys.Length - 1)
            {
                keyboardBuilder.Append(',');
            }
        }

        return
            $"M:{mouse}|K:{keyboardBuilder}|G:{gamepad}";
    }
}
