using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NotCore.Input;

[Serializable]
public readonly struct InputSnapshot
{
    public InputSnapshot()
    {
        PressedKeys = Array.Empty<Keys>();
        GamePadButtonStates = Array.Empty<ButtonState>();
        MouseButtonStates = Array.Empty<ButtonState>();
    }

    public InputSnapshot(KeyboardState keyboardState, MouseState mouseState, GamePadState gamePadState)
    {
        PressedKeys = new Keys[keyboardState.GetPressedKeyCount()];
        for (var i = 0; i < keyboardState.GetPressedKeyCount(); i++)
        {
            PressedKeys[i] = keyboardState.GetPressedKeys()[i];
        }

        MousePosition = mouseState.Position.ToVector2();
        MouseButtonStates = new[]
        {
            mouseState.LeftButton,
            mouseState.RightButton,
            mouseState.MiddleButton
        };

        var gamePadButtons = Enum.GetValues<GamePadButton>();
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
    public ButtonState[] GamePadButtonStates { get; }
    public ButtonState[] MouseButtonStates { get; }
    public Keys[] PressedKeys { get; }
    public Vector2 MousePosition { get; } = Vector2.Zero;

    public static InputSnapshot Human =>
        new(Keyboard.GetState(), Mouse.GetState(), GamePad.GetState(PlayerIndex.One));

    public static InputSnapshot Empty =>
        new(new KeyboardState(), new MouseState(), new GamePadState());
}
