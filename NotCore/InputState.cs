using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NotCore;

public readonly struct InputState
{
    public InputState(NotKeyboardState keyboardState, NotMouseState mouseState)
    {
        Keyboard = keyboardState;
        Mouse = mouseState;
    }

    public static InputState ComputeHumanInputState(InputState previousInputState)
    {
        var rawGamepadState = GamePad.GetState(PlayerIndex.One);
        return new InputState(
            new NotKeyboardState(Microsoft.Xna.Framework.Input.Keyboard.GetState(), previousInputState.Keyboard),
            new NotMouseState(Microsoft.Xna.Framework.Input.Mouse.GetState(), previousInputState.Mouse)
        );
    }

    public NotMouseState Mouse { get; } = new(new MouseState(), null);
    public NotKeyboardState Keyboard { get; } = new(new KeyboardState(), null);
}
