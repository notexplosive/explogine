namespace ExplogineMonoGame.Input;

public readonly struct AllDeviceFrameState
{
    public AllDeviceFrameState(MouseFrameState mouse, KeyboardFrameState keyboard, GamePadFrameState gamePad)
    {
        Mouse = mouse;
        Keyboard = keyboard;
        GamePad = gamePad;
    }

    public GamePadFrameState GamePad { get; }
    public KeyboardFrameState Keyboard { get; }
    public MouseFrameState Mouse { get; }
}
