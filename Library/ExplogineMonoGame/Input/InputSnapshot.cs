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
                        GamePadSnapshotOne = gamePadSnapshot;
                        break;
                    case 1:
                        GamePadSnapshotTwo = gamePadSnapshot;
                        break;
                    case 2:
                        GamePadSnapshotThree = gamePadSnapshot;
                        break;
                    case 3:
                        GamePadSnapshotFour = gamePadSnapshot;
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

    public InputSnapshot(KeyboardState keyboardState, MouseState mouseState, TextEnteredBuffer buffer,
        GamePadState gamePadStateP1,
        GamePadState gamePadStateP2, GamePadState gamePadStateP3, GamePadState gamePadStateP4)
    {
        PressedKeys = new Keys[keyboardState.GetPressedKeyCount()];
        for (var i = 0; i < keyboardState.GetPressedKeyCount(); i++)
        {
            PressedKeys.Set(i, keyboardState.GetPressedKeys()[i]);
        }

        MousePosition = mouseState.Position.ToVector2();
        TextEntered = buffer;
        ScrollValue = mouseState.ScrollWheelValue;
        MouseButtonStates = new ButtonState[InputSerialization.NumberOfMouseButtons];
        MouseButtonStates.Set(0, mouseState.LeftButton);
        MouseButtonStates.Set(1, mouseState.RightButton);
        MouseButtonStates.Set(2, mouseState.MiddleButton);

        GamePadSnapshotOne = new GamePadSnapshot(gamePadStateP1);
        GamePadSnapshotTwo = new GamePadSnapshot(gamePadStateP2);
        GamePadSnapshotThree = new GamePadSnapshot(gamePadStateP3);
        GamePadSnapshotFour = new GamePadSnapshot(gamePadStateP4);
    }

    public TextEnteredBuffer TextEntered { get; } = new();
    public GamePadSnapshot GamePadSnapshotOne { get; } = new();
    public GamePadSnapshot GamePadSnapshotTwo { get; } = new();
    public GamePadSnapshot GamePadSnapshotThree { get; } = new();
    public GamePadSnapshot GamePadSnapshotFour { get; } = new();
    public NotNullArray<ButtonState> MouseButtonStates { get; } = new();
    public NotNullArray<Keys> PressedKeys { get; } = new();
    public Vector2 MousePosition { get; } = Vector2.Zero;
    public int ScrollValue { get; } = 0;

    public IEnumerable<GamePadSnapshot> GamePadSnapshots()
    {
        yield return GamePadSnapshotOne;
        yield return GamePadSnapshotTwo;
        yield return GamePadSnapshotThree;
        yield return GamePadSnapshotFour;
    }

    public GamePadSnapshot GamePadSnapshotOfPlayer(PlayerIndex playerIndex)
    {
        switch (playerIndex)
        {
            case PlayerIndex.One:
                return GamePadSnapshotOne;
            case PlayerIndex.Two:
                return GamePadSnapshotTwo;
            case PlayerIndex.Three:
                return GamePadSnapshotThree;
            case PlayerIndex.Four:
                return GamePadSnapshotFour;
            default:
                throw new Exception("PlayerIndex out of range");
        }
    }

    /// <summary>
    ///     Obtains the latest Human Input State (ie: the actual input of the real physical mouse, keyboard, controller, etc.
    ///     If the Window is not in focus we return an almost-empty InputState, that only holds the current mouse position.
    /// </summary>
    public static InputSnapshot Human =>
        Client.IsInFocus
            ? new InputSnapshot(
                Keyboard.GetState(),
                Mouse.GetState(),
                Client.Window.TextEnteredBuffer,
                GamePad.GetState(PlayerIndex.One),
                GamePad.GetState(PlayerIndex.Two),
                GamePad.GetState(PlayerIndex.Three),
                GamePad.GetState(PlayerIndex.Four)
            )
            : new InputSnapshot(
                new KeyboardState(),
                InputSnapshot.AlmostEmptyMouseState,
                new TextEnteredBuffer(),
                new GamePadState(),
                new GamePadState(),
                new GamePadState(),
                new GamePadState()
            );

    private static MouseState AlmostEmptyMouseState =>
        new(
            Mouse.GetState().X,
            Mouse.GetState().Y,
            0,
            ButtonState.Released,
            ButtonState.Released,
            ButtonState.Released,
            ButtonState.Released,
            ButtonState.Released);

    public static InputSnapshot Empty =>
        new(new KeyboardState(),
            new MouseState(),
            new TextEnteredBuffer(),
            new GamePadState(),
            new GamePadState(),
            new GamePadState(),
            new GamePadState());

    public override string ToString()
    {
        return Serialize();
    }

    public string Serialize()
    {
        return InputSerialization.AsString(this);
    }
}
