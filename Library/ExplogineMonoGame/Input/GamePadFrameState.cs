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

    public ButtonFrameState GetButton(Buttons button, PlayerIndex playerIndex)
    {
        var isDown = InputUtil.CheckIsDown(Current.GamePadSnapshotOfPlayer(playerIndex).PressedButtons, button);
        var wasDown = InputUtil.CheckIsDown(Previous.GamePadSnapshotOfPlayer(playerIndex).PressedButtons, button);
        return new ButtonFrameState(isDown, wasDown);
    }

    private InputSnapshot Previous { get; }
    private InputSnapshot Current { get; }

    public bool IsAnyButtonDown(PlayerIndex playerIndex)
    {
        return Current.GamePadSnapshotOfPlayer(playerIndex).PressedButtons.Length > 0;
    }

    private bool ThumbstickHit(Vector2 previousPosition, Vector2 currentPosition, Vector2 target, float tolerance)
    {
        var wasAtTarget = ApproximatelyEqual(previousPosition, target, tolerance);
        var isAtTarget = ApproximatelyEqual(currentPosition, target, tolerance);

        return isAtTarget && !wasAtTarget;
    }

    private const float Tolerance = 0.05f;
    
    public bool LeftThumbstickAt(PlayerIndex playerIndex, Vector2 target, float tolerance = GamePadFrameState.Tolerance)
    {
        return ApproximatelyEqual(Current.GamePadSnapshotOfPlayer(playerIndex).LeftThumbstick, target, tolerance);
    }
    
    public bool RightThumbstickAt(PlayerIndex playerIndex, Vector2 target, float tolerance = GamePadFrameState.Tolerance)
    {
        return ApproximatelyEqual(Current.GamePadSnapshotOfPlayer(playerIndex).RightThumbstick, target, tolerance);
    }

    public bool LeftThumbstickHit(PlayerIndex playerIndex, Vector2 target, float tolerance = GamePadFrameState.Tolerance)
    {
        return ThumbstickHit(
            Previous.GamePadSnapshotOfPlayer(playerIndex).LeftThumbstick,
            Current.GamePadSnapshotOfPlayer(playerIndex).LeftThumbstick,
            target,
            tolerance
        );
    }

    public bool RightThumbstickHit(PlayerIndex playerIndex, Vector2 target, float tolerance = 0.05f)
    {
        return ThumbstickHit(
            Previous.GamePadSnapshotOfPlayer(playerIndex).LeftThumbstick,
            Current.GamePadSnapshotOfPlayer(playerIndex).LeftThumbstick,
            target,
            tolerance
        );
    }

    private bool ApproximatelyEqual(Vector2 a, Vector2 b, float tolerance)
    {
        return (a - b).Length() < tolerance;
    }

    public Vector2 LeftThumbstickPosition(PlayerIndex playerIndex)
    {
        return Current.GamePadSnapshotOfPlayer(playerIndex).LeftThumbstick;
    }

    public Vector2 RightThumbstickPosition(PlayerIndex playerIndex)
    {
        return Current.GamePadSnapshotOfPlayer(playerIndex).RightThumbstick;
    }

    public float GetLeftTrigger(PlayerIndex playerIndex)
    {
        return Current.GamePadSnapshotOfPlayer(playerIndex).GamePadLeftTrigger;
    }

    public float GetRightTrigger(PlayerIndex playerIndex)
    {
        return Current.GamePadSnapshotOfPlayer(playerIndex).GamePadRightTrigger;
    }

    public bool IsAnyButtonDownOnAnyGamePad()
    {
        return IsAnyButtonDown(PlayerIndex.One) || IsAnyButtonDown(PlayerIndex.Two) ||
               IsAnyButtonDown(PlayerIndex.Three) || IsAnyButtonDown(PlayerIndex.Four);
    }
}
