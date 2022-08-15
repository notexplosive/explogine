using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NotCore.Input;

public readonly struct GamePadFrameState
{
    public GamePadFrameState(InputSnapshot current, InputSnapshot previous)
    {
        Current = current;
        Previous = previous;
    }

    public bool IsButtonDown(GamePadButton button, PlayerIndex playerIndex)
    {
        return InputUtil.CheckIsDown(Current.GamePadSnapshotOfPlayer(playerIndex).GamePadButtonStates, button);
    }

    public bool IsButtonUp(GamePadButton button, PlayerIndex playerIndex)
    {
        return !IsButtonDown(button, playerIndex);
    }

    public bool WasButtonPressed(GamePadButton button, PlayerIndex playerIndex)
    {
        return IsButtonDown(button, playerIndex) && !InputUtil.CheckIsDown(Previous.GamePadSnapshotOfPlayer(playerIndex).GamePadButtonStates, button);
    }

    public bool WasButtonReleased(GamePadButton button, PlayerIndex playerIndex)
    {
        return IsButtonUp(button, playerIndex) && InputUtil.CheckIsDown(Previous.GamePadSnapshotOfPlayer(playerIndex).GamePadButtonStates, button);
    }

    private InputSnapshot Previous { get; }
    private InputSnapshot Current { get; }

    public bool IsAnyButtonDown(PlayerIndex playerIndex)
    {
        foreach (var state in Current.GamePadSnapshotOfPlayer(playerIndex).GamePadButtonStates)
        {
            if (state == ButtonState.Pressed)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsAnyButtonDownOnAnyGamePad()
    {
        return IsAnyButtonDown(PlayerIndex.One) || IsAnyButtonDown(PlayerIndex.Two) ||
               IsAnyButtonDown(PlayerIndex.Three) || IsAnyButtonDown(PlayerIndex.Four);
    }
}
