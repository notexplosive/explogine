using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Input;

public readonly struct InputSnapshot
{
    public InputSnapshot(string serializedString)
    {
        MouseButtonStates = Array.Empty<ButtonState>();
        PressedKeys = Array.Empty<Keys>();
        TextEntered = new TextEnteredBuffer(Array.Empty<char>());

        var split = serializedString.Split('|');
        var playerIndex = 0;

        foreach (var segment in split)
        {
            if (segment.StartsWith("K"))
            {
                var pressedKeys = new List<Keys>();
                var data = segment.Split(":")[1];

                if (!string.IsNullOrEmpty(data))
                {
                    foreach (var keyCode in data.Split(","))
                    {
                        pressedKeys.Add(Enum.Parse<Keys>(keyCode));
                    }

                    PressedKeys = pressedKeys.ToArray();
                }
            }
            else if (segment.StartsWith("M"))
            {
                var data = segment.Split(":")[1].Split(',');
                var mousePosition = new Vector2
                {
                    X = float.Parse(data[0]),
                    Y = float.Parse(data[1])
                };
                MousePosition = mousePosition;
                ScrollValue = int.Parse(data[2]);
                MouseButtonStates =
                    InputSerialization.IntToStates(int.Parse(data[3]), InputSerialization.NumberOfMouseButtons);
            }
            else if (segment.StartsWith("G"))
            {
                var data = segment.Split(":")[1].Split(',');
                var gamePadSnapshot = new GamePadSnapshot(data);

                switch (playerIndex)
                {
                    case 0:
                        GamePadSnapshot1 = gamePadSnapshot;
                        break;
                    case 1:
                        GamePadSnapshot2 = gamePadSnapshot;
                        break;
                    case 2:
                        GamePadSnapshot3 = gamePadSnapshot;
                        break;
                    case 3:
                        GamePadSnapshot4 = gamePadSnapshot;
                        break;
                    default:
                        throw new Exception("Serialized input came in with more than 4 players");
                }

                playerIndex++;
            }

            else if (segment.StartsWith("E"))
            {
                var data = segment.Split(":")[1].Split(',');
                var charList = new List<char>();

                foreach (var item in data)
                {
                    if (item.Length > 0)
                    {
                        charList.Add((char) int.Parse(item));
                    }
                }

                TextEntered = new TextEnteredBuffer(charList.ToArray());
            }
        }
    }

    public InputSnapshot()
    {
    }

    public InputSnapshot(Keys[] pressedKeys, Vector2 mousePosition, ButtonState[] buttonStates, int scrollValue,
        TextEnteredBuffer buffer,
        GamePadSnapshot gamePadStateP1, GamePadSnapshot gamePadStateP2, GamePadSnapshot gamePadStateP3,
        GamePadSnapshot gamePadStateP4)
    {
        PressedKeys = new Keys[pressedKeys.Length];
        for (var i = 0; i < pressedKeys.Length; i++)
        {
            PressedKeys[i] = pressedKeys[i];
        }

        MousePosition = mousePosition;
        TextEntered = buffer;
        ScrollValue = scrollValue;
        MouseButtonStates = new ButtonState[InputSerialization.NumberOfMouseButtons];
        MouseButtonStates[0] = buttonStates[0];
        MouseButtonStates[1] = buttonStates[1];
        MouseButtonStates[2] = buttonStates[2];

        GamePadSnapshot1 = gamePadStateP1;
        GamePadSnapshot2 = gamePadStateP2;
        GamePadSnapshot3 = gamePadStateP3;
        GamePadSnapshot4 = gamePadStateP4;
    }

    public TextEnteredBuffer TextEntered { get; } = new();
    public GamePadSnapshot GamePadSnapshot1 { get; } = new();
    public GamePadSnapshot GamePadSnapshot2 { get; } = new();
    public GamePadSnapshot GamePadSnapshot3 { get; } = new();
    public GamePadSnapshot GamePadSnapshot4 { get; } = new();
    public ButtonState[]? MouseButtonStates { get; } = null;
    public Keys[]? PressedKeys { get; } = null;
    public Vector2 MousePosition { get; } = Vector2.Zero;
    public int ScrollValue { get; } = 0;

    public GamePadSnapshot GamePadSnapshotOfPlayer(PlayerIndex playerIndex)
    {
        switch (playerIndex)
        {
            case PlayerIndex.One:
                return GamePadSnapshot1;
            case PlayerIndex.Two:
                return GamePadSnapshot2;
            case PlayerIndex.Three:
                return GamePadSnapshot3;
            case PlayerIndex.Four:
                return GamePadSnapshot4;
            default:
                throw new Exception("PlayerIndex out of range");
        }
    }

    /// <summary>
    ///     Obtains the latest Human Input State (ie: the actual input of the real physical mouse, keyboard, controller, etc.
    ///     If the Window is not in focus we return an almost-empty InputState.
    /// </summary>
    public static InputSnapshot Human
    {
        get
        {
            if (!Client.IsInFocus)
            {
                return new InputSnapshot(
                    Array.Empty<Keys>(),
                    InputSnapshot.AlmostEmptyMouseState.Position.ToVector2(),
                    new[]
                    {
                        ButtonState.Released,
                        ButtonState.Released,
                        ButtonState.Released
                    },
                    InputSnapshot.AlmostEmptyMouseState.ScrollWheelValue,
                    new TextEnteredBuffer(),
                    new GamePadSnapshot(),
                    new GamePadSnapshot(),
                    new GamePadSnapshot(),
                    new GamePadSnapshot()
                );
            }

            var p1 = GamePad.GetState(PlayerIndex.One);
            var p2 = GamePad.GetState(PlayerIndex.Two);
            var p3 = GamePad.GetState(PlayerIndex.Three);
            var p4 = GamePad.GetState(PlayerIndex.Four);

            var mouseState = Mouse.GetState();
            return new InputSnapshot(
                Keyboard.GetState().GetPressedKeys(),
                mouseState.Position.ToVector2(),
                new[]
                {
                    // order matters!!
                    mouseState.LeftButton,
                    mouseState.RightButton,
                    mouseState.MiddleButton
                },
                mouseState.ScrollWheelValue,
                Client.Window.TextEnteredBuffer,
                new GamePadSnapshot(p1.PressedButtons(), p1.ThumbSticks, p1.Triggers),
                new GamePadSnapshot(p2.PressedButtons(), p2.ThumbSticks, p2.Triggers),
                new GamePadSnapshot(p3.PressedButtons(), p3.ThumbSticks, p3.Triggers),
                new GamePadSnapshot(p4.PressedButtons(), p4.ThumbSticks, p4.Triggers)
            );

        }
    }

    /// <summary>
    ///     Empty MouseState except it holds the current mouse and scroll position
    ///     We need to detect mouse because otherwise we default to (0,0)
    ///     We need to detect scroll position because the scroll value will go from a potentially very high number to 0
    ///     Scrolling is not received unless the window is hovered so this is probably harmless.
    /// </summary>
    private static MouseState AlmostEmptyMouseState =>
        new(
            Mouse.GetState().X,
            Mouse.GetState().Y,
            Mouse.GetState().ScrollWheelValue,
            ButtonState.Released,
            ButtonState.Released,
            ButtonState.Released,
            ButtonState.Released,
            ButtonState.Released);

    public static InputSnapshot Empty =>
        new(Array.Empty<Keys>(),
            InputSnapshot.AlmostEmptyMouseState.Position.ToVector2(),
            new[]
            {
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released
            },
            InputSnapshot.AlmostEmptyMouseState.ScrollWheelValue,
            new TextEnteredBuffer(),
            new GamePadSnapshot(),
            new GamePadSnapshot(),
            new GamePadSnapshot(),
            new GamePadSnapshot());

    public override string ToString()
    {
        return Serialize();
    }

    public string Serialize()
    {
        return InputSerialization.AsString(this);
    }

    public bool IsDown(Keys key)
    {
        return InputUtil.CheckIsDown(PressedKeys, key);
    }
}
