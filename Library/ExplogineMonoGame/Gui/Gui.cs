using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Gui;

public class Gui : IUpdateInput
{
    private readonly List<IGuiWidget> _widgets = new();
    private bool _isReadyToDraw;

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        foreach (var element in _widgets)
        {
            element.UpdateInput(input, hitTestStack);
        }
    }

    public void Button(RectangleF rectangle, string label, Depth depth, Action? onPress)
    {
        _widgets.Add(new Button(rectangle, label, onPress, depth));
    }

    public void Checkbox(RectangleF totalRectangle, string label, Depth depth, Wrapped<bool> state)
    {
        _widgets.Add(new Checkbox(totalRectangle, label, depth, state));
    }

    public void Slider(RectangleF rectangle, Orientation orientation, int numberOfNotches, Depth depth,
        Wrapped<int> state)
    {
        _widgets.Add(new Slider(rectangle, orientation, numberOfNotches, depth, state));
    }

    /// <summary>
    ///     You can call this or Radial.Add, they do the same thing
    /// </summary>
    public void RadialCheckbox(Radial radial, int targetState, RectangleF rectangle, string label, Depth depth)
    {
        _widgets.Add(new RadialCheckbox(radial, targetState, rectangle, label, depth));
    }

    public Radial RadialState(Wrapped<int> state)
    {
        return new Radial(this, state);
    }

    public Gui Panel(RectangleF rectangle, Depth depth)
    {
        var panel = new Panel(rectangle, depth);
        _widgets.Add(panel);
        return panel.InnerGui;
    }

    public void Draw(Painter painter, IGuiTheme uiTheme)
    {
        if (!_isReadyToDraw)
        {
            throw new Exception(
                $"{nameof(Gui.PrepareCanvases)} was not called before drawing");
        }

        foreach (var widget in _widgets)
        {
            switch (widget)
            {
                case Button button:
                    uiTheme.DrawButton(painter, button);
                    break;
                case Panel panel:
                    uiTheme.DrawPanel(painter, panel);
                    break;
                case Checkbox checkbox:
                    uiTheme.DrawCheckbox(painter, checkbox);
                    break;
                case Slider slider:
                    uiTheme.DrawSlider(painter, slider);
                    break;
                case RadialCheckbox radialCheckbox:
                    uiTheme.DrawRadialCheckbox(painter, radialCheckbox);
                    break;
                default:
                    throw new Exception($"Unknown UI Widget type: {widget}");
            }
        }

        _isReadyToDraw = false;
    }

    public void PrepareCanvases(Painter painter, IGuiTheme uiTheme)
    {
        foreach (var widget in _widgets)
        {
            if (widget is IPreDrawWidget iWidgetThatDoesPreDraw)
            {
                iWidgetThatDoesPreDraw.PreDraw(painter, uiTheme);
            }
        }

        _isReadyToDraw = true;
    }
}
