using System;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Data;

public class Clickable
{
    private readonly MouseButton _targetButton;
    private bool _primed;

    public Clickable(MouseButton targetButton = MouseButton.Left)
    {
        _targetButton = targetButton;
    }

    public bool Poll(ConsumableInput.ConsumableMouse inputMouse, HoverState hovered)
    {
        var result = false;
        if (inputMouse.GetButton(_targetButton).WasPressed)
        {
            if (hovered)
            {
                ClickInitiated?.Invoke();
                _primed = true;
            }
        }

        if (inputMouse.GetButton(_targetButton).WasReleased)
        {
            if (hovered && _primed)
            {
                ClickedFully?.Invoke();
                result = true;
            }

            _primed = false;
        }

        return result;
    }

    public event Action? ClickedFully;
    public event Action? ClickInitiated;
}
