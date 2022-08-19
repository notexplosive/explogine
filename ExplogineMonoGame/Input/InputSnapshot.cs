using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Input;

public readonly struct InputSnapshot
{
    public InputSnapshot(string serializedString)
    {
        MouseButtonStates = Array.Empty<ButtonState>();
        PressedKeys = Array.Empty<Keys>();

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
                MouseButtonStates =
                    InputSerialization.IntToStates(int.Parse(data[2]), InputSerialization.NumberOfMouseButtons);
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
        }
    }

    public InputSnapshot()
    {
    }

    public InputSnapshot(KeyboardState keyboardState, MouseState mouseState, GamePadState gamePadStateP1,
        GamePadState gamePadStateP2, GamePadState gamePadStateP3, GamePadState gamePadStateP4)
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

        GamePadSnapshotOne = new GamePadSnapshot(gamePadStateP1);
        GamePadSnapshotTwo = new GamePadSnapshot(gamePadStateP2);
        GamePadSnapshotThree = new GamePadSnapshot(gamePadStateP3);
        GamePadSnapshotFour = new GamePadSnapshot(gamePadStateP4);
    }

    public GamePadSnapshot GamePadSnapshotOne { get; } = new();
    public GamePadSnapshot GamePadSnapshotTwo { get; } = new();
    public GamePadSnapshot GamePadSnapshotThree { get; } = new();
    public GamePadSnapshot GamePadSnapshotFour { get; } = new();
    public ButtonState[] MouseButtonStates { get; } = Array.Empty<ButtonState>();
    public Keys[] PressedKeys { get; } = Array.Empty<Keys>();
    public Vector2 MousePosition { get; } = Vector2.Zero;

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

    public static InputSnapshot Human =>
        new(Keyboard.GetState(), Mouse.GetState(),
            GamePad.GetState(PlayerIndex.One), GamePad.GetState(PlayerIndex.Two),
            GamePad.GetState(PlayerIndex.Three), GamePad.GetState(PlayerIndex.Four)
        );

    public static InputSnapshot Empty =>
        new(new KeyboardState(), new MouseState(), new GamePadState(), new GamePadState(), new GamePadState(),
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
