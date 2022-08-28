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

    public bool Control => _control && !_alt && !_shift;
    public bool Alt => !_control && _alt && !_shift;
    public bool Shift => !_control && !_alt && _shift;
    public bool AltShift => !_control && _alt && _shift;
    public bool ControlShift => _control && !_alt && _shift;
    public bool ControlAlt => _control && _alt && !_shift;
    public bool ControlAltShift => _control && _alt && _shift;
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
