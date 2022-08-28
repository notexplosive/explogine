namespace ExplogineMonoGame.Input;

public readonly struct InputFrameState
{
    public InputSnapshot Current { get; } = new();
    public InputSnapshot Previous { get; } = new();

    public InputFrameState(InputSnapshot current, InputSnapshot previous)
    {
        Current = current;
        Previous = previous;
        Mouse = new MouseFrameState(Current, Previous);
        Keyboard = new KeyboardFrameState(Current, Previous);
        GamePad = new GamePadFrameState(Current, Previous);
    }

    public GamePadFrameState GamePad { get; }
    public KeyboardFrameState Keyboard { get; }
    public MouseFrameState Mouse { get; }

    internal InputFrameState Next(InputSnapshot newSnapshot)
    {
        return new InputFrameState(newSnapshot, Current);
    }
}
