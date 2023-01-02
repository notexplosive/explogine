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
    public bool Enabled { get; set; } = true;

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        if (!Enabled)
        {
            return;
        }

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

    public void DynamicLabel(RectangleF rectangle, Depth depth, Action<Painter, IGuiTheme, RectangleF, Depth> action)
    {
        _widgets.Add(new DynamicLabel(rectangle, depth, action));
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

    public Gui AddSubGui(bool startEnabled = false)
    {
        var page = new SubGuiWidget
        {
            InnerGui =
            {
                Enabled = startEnabled
            }
        };
        _widgets.Add(page);
        return page.InnerGui;
    }

    public void Draw(Painter painter, IGuiTheme theme)
    {
        if (!Enabled)
        {
            return;
        }

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
                    theme.DrawButton(painter, button);
                    break;
                case Panel panel:
                    panel.Draw(painter, theme);
                    break;
                case Checkbox checkbox:
                    theme.DrawCheckbox(painter, checkbox);
                    break;
                case Slider slider:
                    theme.DrawSlider(painter, slider);
                    break;
                case RadialCheckbox radialCheckbox:
                    theme.DrawRadialCheckbox(painter, radialCheckbox);
                    break;
                case SubGuiWidget page:
                    page.Draw(painter, theme);
                    break;
                case DynamicLabel dynamicLabel:
                    dynamicLabel.Draw(painter, theme);
                    break;
                default:
                    throw new Exception($"Unknown UI Widget type: {widget}");
            }
        }

        _isReadyToDraw = false;
    }

    public void PrepareCanvases(Painter painter, IGuiTheme uiTheme)
    {
        if (!Enabled)
        {
            return;
        }

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
