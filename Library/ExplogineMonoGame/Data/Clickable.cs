using System;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Data;

public class Clickable
{
    private readonly MouseButton _targetButton;
    public bool Primed { get; private set; }

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
                Primed = true;
            }
        }

        if (inputMouse.GetButton(_targetButton).WasReleased)
        {
            if (hovered && Primed)
            {
                ClickedFully?.Invoke();
                result = true;
            }

            Primed = false;
        }

        return result;
    }

    public event Action? ClickedFully;
    public event Action? ClickInitiated;
}
