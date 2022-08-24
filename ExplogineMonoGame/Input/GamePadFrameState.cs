using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Input;

public readonly struct GamePadFrameState
{
    public GamePadFrameState(InputSnapshot current, InputSnapshot previous)
    {
        Current = current;
        Previous = previous;
    }

    public ButtonFrameState GetButton(GamePadButton button, PlayerIndex playerIndex)
    {
        var isDown = InputUtil.CheckIsDown(Current.GamePadSnapshotOfPlayer(playerIndex).GamePadButtonStates, button);
        var wasDown = InputUtil.CheckIsDown(Previous.GamePadSnapshotOfPlayer(playerIndex).GamePadButtonStates, button);
        return new ButtonFrameState(isDown, wasDown);
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
