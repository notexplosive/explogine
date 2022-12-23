namespace ExplogineMonoGame.Input;

public readonly struct ModifierKeys
{
    private readonly bool _control;
    private readonly bool _alt;
    private readonly bool _shift;

    public ModifierKeys(bool control, bool alt, bool shift)
    {
        _control = control;
        _alt = alt;
        _shift = shift;
    }

    /// <summary>
    /// True if left or right control are pressed
    /// </summary>
    public bool ControlInclusive => _control;
    
    /// <summary>
    /// True if left and right alt are pressed
    /// </summary>
    public bool AltInclusive => _alt;
    
    /// <summary>
    /// True if left and right shift are pressed
    /// </summary>
    public bool ShiftInclusive => _shift;
    
    /// <summary>
    /// True if Control is the only modifier pressed
    /// </summary>
    public bool Control => _control && !_alt && !_shift;
    
    /// <summary>
    /// True if Alt is the only modifier pressed
    /// </summary>
    public bool Alt => !_control && _alt && !_shift;
    
    /// <summary>
    /// True if Shift is the only modifier pressed
    /// </summary>
    public bool Shift => !_control && !_alt && _shift;
    
    /// <summary>
    /// True if Alt and Shift are the only modifiers pressed
    /// </summary>
    public bool AltShift => !_control && _alt && _shift;
    
    /// <summary>
    /// True if Control and Shift are the only modifier pressed
    /// </summary>
    public bool ControlShift => _control && !_alt && _shift;
    
    /// <summary>
    /// True if Control and Alt are the only modifier pressed
    /// </summary>
    public bool ControlAlt => _control && _alt && !_shift;
    
    /// <summary>
    /// True if Control, Alt, and Shift are all pressed
    /// </summary>
    public bool ControlAltShift => _control && _alt && _shift;
    
    /// <summary>
    /// True if no modifiers are pressed
    /// </summary>
    public bool None => !_control && !_alt && !_shift;

    public override string ToString()
    {
        if (None)
        {
            return "None";
        }

        var ctrl = _control ? "Control" : "";
        var alt = _alt ? "Alt" : "";
        var shift = _shift ? "Shift" : "";

        return $"{ctrl}{alt}{shift}";
    }
}
