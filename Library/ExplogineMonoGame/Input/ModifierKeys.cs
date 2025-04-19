namespace ExplogineMonoGame.Input;

public readonly struct ModifierKeys
{
    public ModifierKeys(bool control, bool alt, bool shift)
    {
        ControlInclusive = control;
        AltInclusive = alt;
        ShiftInclusive = shift;
    }

    /// <summary>
    ///     True if left or right control are pressed
    /// </summary>
    public bool ControlInclusive { get; }

    /// <summary>
    ///     True if left and right alt are pressed
    /// </summary>
    public bool AltInclusive { get; }

    /// <summary>
    ///     True if left or right shift are pressed
    /// </summary>
    public bool ShiftInclusive { get; }

    /// <summary>
    ///     True if Control is the only modifier pressed
    /// </summary>
    public bool Control => ControlInclusive && !AltInclusive && !ShiftInclusive;

    /// <summary>
    ///     True if Alt is the only modifier pressed
    /// </summary>
    public bool Alt => !ControlInclusive && AltInclusive && !ShiftInclusive;

    /// <summary>
    ///     True if Shift is the only modifier pressed
    /// </summary>
    public bool Shift => !ControlInclusive && !AltInclusive && ShiftInclusive;

    /// <summary>
    ///     True if Alt and Shift are the only modifiers pressed
    /// </summary>
    public bool AltShift => !ControlInclusive && AltInclusive && ShiftInclusive;

    /// <summary>
    ///     True if Control and Shift are the only modifier pressed
    /// </summary>
    public bool ControlShift => ControlInclusive && !AltInclusive && ShiftInclusive;

    /// <summary>
    ///     True if Control and Alt are the only modifier pressed
    /// </summary>
    public bool ControlAlt => ControlInclusive && AltInclusive && !ShiftInclusive;

    /// <summary>
    ///     True if Control, Alt, and Shift are all pressed
    /// </summary>
    public bool ControlAltShift => ControlInclusive && AltInclusive && ShiftInclusive;

    /// <summary>
    ///     True if no modifiers are pressed
    /// </summary>
    public bool None => !ControlInclusive && !AltInclusive && !ShiftInclusive;

    public override string ToString()
    {
        if (None)
        {
            return "None";
        }

        var ctrl = ControlInclusive ? "Control" : "";
        var alt = AltInclusive ? "Alt" : "";
        var shift = ShiftInclusive ? "Shift" : "";

        return $"{ctrl}{alt}{shift}";
    }
}
