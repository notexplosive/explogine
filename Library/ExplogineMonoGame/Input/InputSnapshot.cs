﻿using System;
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

    public InputSnapshot(Keys[] pressedKeys, Vector2 mousePosition, ButtonState[] buttonStates, int scrollValue, TextEnteredBuffer buffer,
        GamePadState gamePadStateP1,
        GamePadState gamePadStateP2, GamePadState gamePadStateP3, GamePadState gamePadStateP4)
    {
        PressedKeys = new Keys[pressedKeys.Length];
        for (var i = 0; i < pressedKeys.Length; i++)
        {
            PressedKeys.Set(i, pressedKeys[i]);
        }

        MousePosition = mousePosition;
        TextEntered = buffer;
        ScrollValue = scrollValue;
        MouseButtonStates = new ButtonState[InputSerialization.NumberOfMouseButtons];
        MouseButtonStates.Set(0, buttonStates[0]);
        MouseButtonStates.Set(1, buttonStates[1]);
        MouseButtonStates.Set(2, buttonStates[2]);

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
    ///     If the Window is not in focus we return an almost-empty InputState.
    /// </summary>
    public static InputSnapshot Human
    {
        get
        {
            if (Client.IsInFocus)
            {
                var mouseState = Mouse.GetState();
                return new InputSnapshot(
                    Keyboard.GetState().GetPressedKeys(),
                    mouseState.Position.ToVector2(),
                    new ButtonState[]
                    {
                        // order matters!!
                        mouseState.LeftButton,
                        mouseState.RightButton,
                        mouseState.MiddleButton,
                    },
                    mouseState.ScrollWheelValue,
                    Client.Window.TextEnteredBuffer,
                    GamePad.GetState(PlayerIndex.One),
                    GamePad.GetState(PlayerIndex.Two),
                    GamePad.GetState(PlayerIndex.Three),
                    GamePad.GetState(PlayerIndex.Four)
                );
            }
            else
            {
                return new InputSnapshot(
                    Array.Empty<Keys>(),
                    InputSnapshot.AlmostEmptyMouseState.Position.ToVector2(),
                    new ButtonState[]
                    {
                        ButtonState.Released,
                        ButtonState.Released,
                        ButtonState.Released
                    },
                    InputSnapshot.AlmostEmptyMouseState.ScrollWheelValue,
                    new TextEnteredBuffer(),
                    new GamePadState(),
                    new GamePadState(),
                    new GamePadState(),
                    new GamePadState()
                );
            }
        }
    }

    /// <summary>
    /// Empty MouseState except it holds the current mouse and scroll position
    /// We need to detect mouse because otherwise we default to (0,0)
    /// We need to detect scroll position because the scroll value will go from a potentially very high number to 0
    /// Scrolling is not received unless the window is hovered so this is probably harmless. 
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
            AlmostEmptyMouseState.Position.ToVector2(), 
            new ButtonState[3]
            {
                ButtonState.Released,
                ButtonState.Released,
                ButtonState.Released
            },
            AlmostEmptyMouseState.ScrollWheelValue,
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
