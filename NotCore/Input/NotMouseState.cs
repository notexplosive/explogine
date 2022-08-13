using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NotCore;

public class NotMouseState
{
    private readonly bool[] _isDown;
    private readonly int _scrollDelta;
    private readonly int _scrollWheelValue;
    private readonly bool[] _wasPressed;
    private readonly bool[] _wasReleased;

    internal NotMouseState(MouseState rawCurrentState, NotMouseState? previousState)
    {
        Position = rawCurrentState.Position.ToVector2();
        var mouseButtons = Enum.GetValues(typeof(MouseButton));
        var numberOfMouseButtons = mouseButtons.Length;
        _isDown = new bool[numberOfMouseButtons];

        if (rawCurrentState.LeftButton == ButtonState.Pressed)
        {
            _isDown[(int) MouseButton.Left] = true;
        }

        if (rawCurrentState.RightButton == ButtonState.Pressed)
        {
            _isDown[(int) MouseButton.Right] = true;
        }

        if (rawCurrentState.MiddleButton == ButtonState.Pressed)
        {
            _isDown[(int) MouseButton.Middle] = true;
        }

        _wasPressed = new bool[numberOfMouseButtons];
        _wasReleased = new bool[numberOfMouseButtons];
        _scrollWheelValue = rawCurrentState.ScrollWheelValue;

        if (previousState != null)
        {
            foreach (int button in mouseButtons)
            {
                if (!previousState._isDown[button] && _isDown[button])
                {
                    _wasPressed[button] = true;
                }

                if (previousState._isDown[button] && !_isDown[button])
                {
                    _wasReleased[button] = true;
                }
            }

            _scrollDelta = (_scrollWheelValue - previousState._scrollWheelValue) / 120;
        }
    }

    public Vector2 Position { get; } = Vector2.Zero;

    public bool IsDown(MouseButton button)
    {
        return _isDown[(int) button];
    }

    public bool WasPressed(MouseButton button)
    {
        return _wasPressed[(int) button];
    }

    public bool WasReleased(MouseButton button)
    {
        return _wasReleased[(int) button];
    }

    public int ScrollDelta()
    {
        return _scrollDelta;
    }
}

public enum MouseButton
{
    Left,
    Right,
    Middle
}
