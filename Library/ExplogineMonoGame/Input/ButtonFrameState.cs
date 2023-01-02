namespace ExplogineMonoGame.Input;

public readonly struct ButtonFrameState
{
    public bool IsDown { get; }
    public bool IsUp => !IsDown;
    public bool WasPressed { get; }
    public bool WasReleased { get; }

    internal ButtonFrameState(bool isDown, bool wasDown)
    {
        IsDown = isDown;
        WasPressed = isDown && !wasDown;
        WasReleased = !isDown && wasDown;
    }

    public static ButtonFrameState Empty => new(false, false);
}
