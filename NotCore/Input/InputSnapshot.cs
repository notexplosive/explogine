using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NotCore.Input;

public readonly struct InputSnapshot
{
    public InputSnapshot(string serializedString)
    {
        GamePadButtonStates = Array.Empty<ButtonState>();
        MouseButtonStates = Array.Empty<ButtonState>();
        PressedKeys = Array.Empty<Keys>();
        
        var split = serializedString.Split('|');

        foreach (var segment in split)
        {
            if (segment.StartsWith("K"))
            {
                var pressedKeys = new List<Keys>();
                var data = segment.Split(":")[1];
                foreach (var keyCode in data.Split(","))
                {
                    pressedKeys.Add(Enum.Parse<Keys>(keyCode));
                }
                PressedKeys = pressedKeys.ToArray();
            }

            if (segment.StartsWith("M"))
            {
                var data = segment.Split(":")[1].Split(',');
                var mousePosition = new Vector2
                {
                    X = float.Parse(data[0]),
                    Y = float.Parse(data[1])
                };
                MousePosition = mousePosition;
                MouseButtonStates = InputSerialization.IntToStates(int.Parse(data[2]), InputSerialization.NumberOfMouseButtons);
            }

            if (segment.StartsWith("G"))
            {
                var data = segment.Split(":")[1].Split(',');
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
                GamePadButtonStates = InputSerialization.IntToStates(int.Parse(data[6]), InputSerialization.NumberOfGamepadButtons);
            }
        }
    }
    
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
        MouseButtonStates = new ButtonState[InputSerialization.NumberOfMouseButtons];
        MouseButtonStates[0] = mouseState.LeftButton;
        MouseButtonStates[1] = mouseState.RightButton;
        MouseButtonStates[2] = mouseState.MiddleButton;

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
    public ButtonState[] GamePadButtonStates { get; }
    public ButtonState[] MouseButtonStates { get; }
    public Keys[] PressedKeys { get; }
    public Vector2 MousePosition { get; } = Vector2.Zero;

    public static InputSnapshot Human =>
        new(Keyboard.GetState(), Mouse.GetState(), GamePad.GetState(PlayerIndex.One));

    public static InputSnapshot Empty =>
        new(new KeyboardState(), new MouseState(), new GamePadState());

    public override string ToString()
    {
        return InputSerialization.AsString(this);
    }
}

