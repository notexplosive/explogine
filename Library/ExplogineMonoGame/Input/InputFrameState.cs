namespace ExplogineMonoGame.Input;

public readonly struct InputFrameState
{
    private InputSnapshot Current { get; } = new();
    private InputSnapshot Previous { get; } = new();

    public InputFrameState(InputSnapshot current, InputSnapshot previous)
    {
        Current = current;
        Previous = previous;

        AllDevices = new AllDeviceFrameState(
            new MouseFrameState(Current, Previous),
            new KeyboardFrameState(Current, Previous),
            new GamePadFrameState(Current, Previous)
        );
    }

    public GamePadFrameState GamePad => AllDevices.GamePad;
    public KeyboardFrameState Keyboard => AllDevices.Keyboard;
    public MouseFrameState Mouse => AllDevices.Mouse;
    public AllDeviceFrameState AllDevices { get; }

    internal InputFrameState Next(InputSnapshot newSnapshot)
    {
        return new InputFrameState(newSnapshot, Current);
    }
}