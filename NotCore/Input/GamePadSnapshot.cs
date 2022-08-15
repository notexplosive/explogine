using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NotCore.Input;

public readonly struct GamePadSnapshot
{
    public GamePadSnapshot(string[] data)
    {
        GamePadLeftTrigger = float.Parse(data[0]);
        GamePadRightTrigger = float.Parse(data[1]);
        LeftThumbstick = new Vector2
        {
            X = float.Parse(data[2]),
            Y = float.Parse(data[3])
        };
        RightThumbstick = new Vector2
        {
            X = float.Parse(data[4]),
            Y = float.Parse(data[5])
        };
        GamePadButtonStates =
            InputSerialization.IntToStates(int.Parse(data[6]), InputSerialization.NumberOfGamepadButtons);
    }

    public GamePadSnapshot(GamePadState gamePadState)
    {
        var gamePadButtons = InputSerialization.AllGamePadButtons;
        GamePadButtonStates = new ButtonState[gamePadButtons.Length];
        foreach (var value in gamePadButtons)
        {
            GamePadButtonStates[(int) value] = gamePadState.ButtonLookup(value);
        }

        GamePadLeftTrigger = gamePadState.Triggers.Left;
        GamePadRightTrigger = gamePadState.Triggers.Right;

        LeftThumbstick = gamePadState.ThumbSticks.Left;
        RightThumbstick = gamePadState.ThumbSticks.Right;
    }

    public Vector2 RightThumbstick { get; } = Vector2.Zero;
    public Vector2 LeftThumbstick { get; } = Vector2.Zero;
    public float GamePadRightTrigger { get; } = 0f;
    public float GamePadLeftTrigger { get; } = 0f;
    public ButtonState[] GamePadButtonStates { get; } = Array.Empty<ButtonState>();
}
