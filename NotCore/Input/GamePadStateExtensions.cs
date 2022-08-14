using System;
using Microsoft.Xna.Framework.Input;

namespace NotCore.Input;

public static class GamePadStateExtensions
{
    public static ButtonState ButtonLookup(this GamePadState state, GamePadButton buttonName)
    {
        switch (buttonName)
        {
            case GamePadButton.A:
                return state.Buttons.A;
            case GamePadButton.B:
                return state.Buttons.B;
            case GamePadButton.X:
                return state.Buttons.X;
            case GamePadButton.Y:
                return state.Buttons.Y;
            case GamePadButton.DPadUp:
                return state.DPad.Up;
            case GamePadButton.DPadDown:
                return state.DPad.Down;
            case GamePadButton.DPadLeft:
                return state.DPad.Left;
            case GamePadButton.DPadRight:
                return state.DPad.Right;
            case GamePadButton.Start:
                return state.Buttons.Start;
            case GamePadButton.Back:
                return state.Buttons.Back;
            case GamePadButton.BigButton:
                return state.Buttons.BigButton;
            case GamePadButton.LeftShoulder:
                return state.Buttons.LeftShoulder;
            case GamePadButton.RightShoulder:
                return state.Buttons.RightShoulder;
            case GamePadButton.LeftStick:
                return state.Buttons.LeftStick;
            case GamePadButton.RightStick:
                return state.Buttons.RightStick;
            default:
                throw new Exception($"Unknown button {buttonName}");
        }
    }
}
