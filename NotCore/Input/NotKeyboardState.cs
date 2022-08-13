using Microsoft.Xna.Framework.Input;

namespace NotCore;

public class NotKeyboardState
{
    private readonly ButtonState[] _buttonState;
    private readonly bool[] _wasPressedThisFrame;
    private readonly bool[] _wasReleasedThisFrame;

    public NotKeyboardState(KeyboardState newKeyboardState, NotKeyboardState? previousState)
    {
        _buttonState = NotKeyboardState.CreateKeyboardStateArray<ButtonState>();
        _wasPressedThisFrame = NotKeyboardState.CreateKeyboardStateArray<bool>();
        _wasReleasedThisFrame = NotKeyboardState.CreateKeyboardStateArray<bool>();

        foreach (var pressedKey in newKeyboardState.GetPressedKeys())
        {
            _buttonState[(int) pressedKey] = ButtonState.Pressed;
        }

        if (previousState != null)
        {
            for (var key = 0; key < _buttonState.Length; key++)
            {
                var isPressed = _buttonState[key] == ButtonState.Pressed;
                var isReleased = _buttonState[key] != ButtonState.Pressed;

                var wasPressed = previousState._buttonState[key] == ButtonState.Pressed;
                var wasReleased = previousState._buttonState[key] != ButtonState.Pressed;

                if (wasReleased && isPressed)
                {
                    _wasPressedThisFrame[key] = true;
                }

                if (wasPressed && isReleased)
                {
                    _wasReleasedThisFrame[key] = true;
                }
            }
        }
    }

    public bool WasPressed(Keys key)
    {
        return _wasPressedThisFrame[(int) key];
    }

    public bool WasReleased(Keys key)
    {
        return _wasReleasedThisFrame[(int) key];
    }

    public bool IsDown(Keys key)
    {
        return _buttonState[(int) key] == ButtonState.Pressed;
    }

    private static T[] CreateKeyboardStateArray<T>()
    {
        var keyboardKeyCount = 255;
        return new T[keyboardKeyCount];
    }
}
